using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemigoAI : MonoBehaviour
{
    enum PatrolMode { Waypoints, Random }
    enum AIState { Patrol, Chase, Returning, Idle }

    [Header("Detección y combate")]
    [SerializeField] private float detectionRadius = 10f;     // distancia para empezar a seguir
    [SerializeField] private float attackRadius = 2f;         // distancia para atacar
    [SerializeField] private float followSpeed = 3.5f;        // velocidad de seguimiento
    [SerializeField] private float rotationSpeed = 10f;       // velocidad de giro hacia el jugador
    [SerializeField] private float attackCooldown = 1.2f;     // tiempo entre ataques
    [SerializeField] private float returnDelay = 3f;          // tiempo que espera antes de volver a su posición inicial si pierde al jugador
    [SerializeField] private bool requireLineOfSight = false; // si true usa raycast para comprobar visión directa

    [Header("Patrulla")]
    [SerializeField] private PatrolMode patrolMode = PatrolMode.Waypoints;
    [SerializeField] private Transform[] patrolPoints;        // si usas Waypoints, asigna transforms en el inspector
    [SerializeField] private float patrolRadius = 5f;         // radio para generar puntos aleatorios si usas Random
    [SerializeField] private int randomPatrolCount = 4;       // cuantos puntos generar en modo Random
    [SerializeField] private float patrolSpeed = 2f;          // velocidad durante patrulla
    [SerializeField] private float patrolWaitTime = 1f;       // espera al llegar a un punto de patrulla
    [SerializeField] private float patrolPointReachThreshold = 0.35f;

    [Header("Evitación de obstáculos (sin NavMesh)")]
    [SerializeField] private float obstacleDetectDistance = 1.0f;   // distancia de detección para evitar obstaculos
    [SerializeField] private float obstacleSphereRadius = 0.35f;    // radio del sensor (SphereCast)
    [SerializeField] private float sensorHeight = 0.8f;             // altura desde la cual se hace el sensor
    [SerializeField] private LayerMask obstacleMask = ~0;           // capas a considerar como obstáculo

    [Header("Opcional")]
    [SerializeField] private Animator animator;               // asignar si tienes animaciones
    [SerializeField] private string playerTag = "Player";     // tag del jugador

    private Transform player;
    private Vector3 startPosition;
    private AIState state = AIState.Patrol;

    private Rigidbody rb;
    private CharacterController cc;

    private Vector3 currentPatrolTarget;
    private int currentPatrolIndex = 0;
    private Coroutine returnCoroutine;
    private Coroutine patrolWaitCoroutine;
    private bool canAttack = true;

    void Awake()
    {
        startPosition = transform.position;
        rb = GetComponent<Rigidbody>();
        cc = GetComponent<CharacterController>();
    }

    void Start()
    {
        var playerObj = GameObject.FindGameObjectWithTag(playerTag);
        if (playerObj != null) player = playerObj.transform;
        else Debug.LogWarning($"[EnemigoAI] No se encontró ningún GameObject con tag '{playerTag}'.");

        // Preparar puntos de patrulla
        if ((patrolPoints == null || patrolPoints.Length == 0) && patrolMode == PatrolMode.Waypoints)
        {
            // si no hay waypoints asignados, usar modo Random
            patrolMode = PatrolMode.Random;
        }

        if (patrolMode == PatrolMode.Random)
        {
            GenerateRandomPatrolPoints();
        }

        if (patrolPoints != null && patrolPoints.Length > 0)
        {
            currentPatrolIndex = 0;
            currentPatrolTarget = patrolPoints[currentPatrolIndex].position;
        }
        else
        {
            currentPatrolTarget = startPosition;
        }
    }

    void Update()
    {
        // Detección del jugador
        bool playerVisible = true;
        float distanceToPlayer = Mathf.Infinity;
        if (player != null)
        {
            distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (requireLineOfSight)
            {
                Vector3 dir = (player.position - transform.position).normalized;
                Vector3 origin = transform.position + Vector3.up * 0.5f;
                if (Physics.Raycast(origin, dir, out RaycastHit hit, detectionRadius))
                    playerVisible = hit.transform == player;
            }
        }

        // Transiciones de estado
        if (player != null && distanceToPlayer <= detectionRadius && playerVisible)
        {
            // empieza persecución
            if (returnCoroutine != null)
            {
                StopCoroutine(returnCoroutine);
                returnCoroutine = null;
            }

            if (patrolWaitCoroutine != null)
            {
                StopCoroutine(patrolWaitCoroutine);
                patrolWaitCoroutine = null;
            }

            state = AIState.Chase;
        }
        else
        {
            // si estaba persiguiendo y perdió al jugador, iniciar retorno después de delay
            if (state == AIState.Chase && (player == null || distanceToPlayer > detectionRadius || !playerVisible))
            {
                if (returnCoroutine != null) StopCoroutine(returnCoroutine);
                returnCoroutine = StartCoroutine(DelayedReturnToStart());
            }
        }

        // Ejecutar comportamiento según estado
        switch (state)
        {
            case AIState.Patrol:
                PatrolUpdate();
                break;
            case AIState.Chase:
                ChaseUpdate(distanceToPlayer);
                break;
            case AIState.Returning:
                ReturnUpdate();
                break;
            case AIState.Idle:
            default:
                if (animator != null) animator.SetBool("isMoving", false);
                break;
        }
    }

                #region Comportamientos

    private void PatrolUpdate()
    {
        if (patrolPoints == null || patrolPoints.Length == 0)
        {
            // no hay puntos de patrulla asignados: queda en la posición de inicio
            MoveTowardsWithAvoidance(startPosition, patrolSpeed);
            return;
        }

        float dist = Vector3.Distance(transform.position.WithY(0), currentPatrolTarget.WithY(0));
        if (dist <= patrolPointReachThreshold)
        {
            if (patrolWaitCoroutine == null)
                patrolWaitCoroutine = StartCoroutine(WaitAtPatrolPoint());
            if (animator != null) animator.SetBool("isMoving", false);
            return;
        }

        MoveTowardsWithAvoidance(currentPatrolTarget, patrolSpeed);
    }

    private void ChaseUpdate(float distanceToPlayer)
    {
        if (player == null) return;

        if (distanceToPlayer > attackRadius)
        {
            // perseguir
            MoveTowardsWithAvoidance(player.position.WithY(transform.position.y), followSpeed);
        }
        else
        {
            // atacar
            if (animator != null) animator.SetBool("isMoving", false);
            TryAttack();
        }
    }

    private void ReturnUpdate()
    {
        float dist = Vector3.Distance(transform.position.WithY(0), startPosition.WithY(0));
        if (dist <= 0.15f)
        {
            // llegó a la posición inicial
            state = AIState.Patrol;
            if (patrolPoints != null && patrolPoints.Length > 0)
            {
                // seleccionar el punto de patrulla más cercano como siguiente destino
                currentPatrolIndex = FindClosestPatrolIndex();
                currentPatrolTarget = patrolPoints[currentPatrolIndex].position;
            }
            else
            {
                currentPatrolTarget = startPosition;
            }

            if (animator != null) animator.SetBool("isMoving", false);
            return;
        }

        MoveTowardsWithAvoidance(startPosition, followSpeed);
    }

    #endregion

    #region Movimiento y evitación

    // Movimiento hacia target con detección y evasion de obstáculos (no usa NavMesh).
    private void MoveTowardsWithAvoidance(Vector3 target, float speed)
    {
        // mantener altura actual del enemigo
        target.y = transform.position.y;
        Vector3 toTarget = target - transform.position;
        toTarget.y = 0f;
        if (toTarget.sqrMagnitude < 0.0001f)
        {
            if (animator != null) animator.SetBool("isMoving", false);
            return;
        }

        Vector3 desiredDir = toTarget.normalized;
        Vector3 origin = transform.position + Vector3.up * sensorHeight + desiredDir * 0.15f; // pequeño offset hacia adelante
        RaycastHit hit;
        Vector3 moveDir = desiredDir;

        // Si se detecta obstáculo frente a la dirección deseada, buscamos una dirección despejada
        if (Physics.SphereCast(origin, obstacleSphereRadius, desiredDir, out hit, obstacleDetectDistance, obstacleMask, QueryTriggerInteraction.Ignore))
        {
            if (hit.collider != null && hit.collider.gameObject != gameObject)
            {
                moveDir = FindClearDirection(desiredDir, origin);
            }
        }

        // Rotación suave hacia la dirección de movimiento
        if (moveDir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }

        Vector3 movement = moveDir * speed * Time.deltaTime;

        // Preferir Rigidbody o CharacterController si existen (mejor manejo de colisiones)
        if (rb != null && !rb.isKinematic)
        {
            rb.MovePosition(rb.position + movement);
        }
        else if (cc != null)
        {
            cc.Move(movement);
        }
        else
        {
            transform.position += movement;
        }

        if (animator != null) animator.SetBool("isMoving", true);
    }

    // Busca una dirección libre rotando la dirección deseada en varios ángulos.
    private Vector3 FindClearDirection(Vector3 desiredDir, Vector3 origin)
    {
        // comprueba primero rotaciones pequeñas y luego más grandes
        float[] angles = { 25f, -25f, 50f, -50f, 90f, -90f, 120f, -120f };
        foreach (float ang in angles)
        {
            Vector3 candidate = Quaternion.Euler(0f, ang, 0f) * desiredDir;
            if (!Physics.SphereCast(origin, obstacleSphereRadius, candidate, out RaycastHit h2, obstacleDetectDistance, obstacleMask, QueryTriggerInteraction.Ignore))
                return candidate;
        }

        // fallback: deslizarse siguiendo la normal del impacto
        if (Physics.SphereCast(origin, obstacleSphereRadius, desiredDir, out RaycastHit fallbackHit, obstacleDetectDistance, obstacleMask, QueryTriggerInteraction.Ignore))
        {
            Vector3 slide = Vector3.Cross(fallbackHit.normal, Vector3.up).normalized;
            if (Vector3.Dot(slide, desiredDir) < 0f) slide = -slide;
            return slide;
        }

        // ultimo recurso: devolver la dirección deseada
        return desiredDir;
    }

    #endregion

    #region Coroutines y utilidades

    private IEnumerator WaitAtPatrolPoint()
    {
        if (animator != null) animator.SetBool("isMoving", false);
        yield return new WaitForSeconds(patrolWaitTime);

        // elegir siguiente punto
        patrolWaitCoroutine = null;
        if (patrolPoints != null && patrolPoints.Length > 0)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
            currentPatrolTarget = patrolPoints[currentPatrolIndex].position;
        }
        else
        {
            currentPatrolTarget = GetRandomPointInRadius();
        }
    }

    private IEnumerator DelayedReturnToStart()
    {
        yield return new WaitForSeconds(returnDelay);

        // si durante la espera el jugador volvió a entrar, no iniciar retorno
        if (player != null && Vector3.Distance(transform.position, player.position) <= detectionRadius) yield break;

        state = AIState.Returning;
        returnCoroutine = null;
    }

    private int FindClosestPatrolIndex()
    {
        if (patrolPoints == null || patrolPoints.Length == 0) return 0;
        int best = 0;
        float bestDist = Vector3.Distance(transform.position.WithY(0), patrolPoints[0].position.WithY(0));
        for (int i = 1; i < patrolPoints.Length; i++)
        {
            float d = Vector3.Distance(transform.position.WithY(0), patrolPoints[i].position.WithY(0));
            if (d < bestDist)
            {
                bestDist = d;
                best = i;
            }
        }
        return best;
    }

    private void GenerateRandomPatrolPoints()
    {
        List<Transform> generated = new List<Transform>();
        // generamos puntos temporales como objetos hijos para poder usarlos en inspector y moverlos si hace falta
        for (int i = 0; i < randomPatrolCount; i++)
        {
            Vector3 p = GetRandomPointInRadius();
            GameObject go = new GameObject($"PatrolPoint_{i}");
            go.transform.position = p;
            go.transform.parent = this.transform; // opcional: como hijo del enemigo
            generated.Add(go.transform);
        }
        patrolPoints = generated.ToArray();
    }

    private Vector3 GetRandomPointInRadius()
    {
        for (int i = 0; i < 12; i++)
        {
            Vector2 r = Random.insideUnitCircle * patrolRadius;
            Vector3 sample = startPosition + new Vector3(r.x, 0f, r.y);

            // intentar proyectar al suelo para obtener la Y correcta
            if (Physics.Raycast(sample + Vector3.up * 5f, Vector3.down, out RaycastHit hit, 10f, obstacleMask, QueryTriggerInteraction.Ignore))
            {
                Vector3 candidate = new Vector3(sample.x, hit.point.y, sample.z);

                // opcional: verificar que el punto no esté dentro de una pared (haciendo un overlap)
                if (!Physics.CheckSphere(candidate + Vector3.up * 0.2f, 0.2f, obstacleMask, QueryTriggerInteraction.Ignore))
                    return candidate;
            }
            else
            {
                // si no hay suelo detectado, devolver sample simple con la Y del start
                Vector3 candidate = new Vector3(sample.x, startPosition.y, sample.z);
                if (!Physics.CheckSphere(candidate + Vector3.up * 0.2f, 0.2f, obstacleMask, QueryTriggerInteraction.Ignore))
                    return candidate;
            }
        }

        // si falla, devolver la posición inicial
        return startPosition;
    }

    private void TryAttack()
    {
        if (!canAttack) return;

        canAttack = false;
        if (animator != null) animator.SetTrigger("attack");
        Debug.Log("[EnemigoAI] Atacando al jugador.");

        StartCoroutine(ResetAttackCooldown());
    }

    private IEnumerator ResetAttackCooldown()
    {
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    #endregion

    // Dibuja radios de detección y ataque en el editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);

        // gizmos para patrulla
        Gizmos.color = Color.cyan;
        if (patrolPoints != null)
        {
            foreach (var p in patrolPoints)
            {
                if (p == null) continue;
                Gizmos.DrawSphere(p.position, 0.15f);
                Gizmos.DrawLine(transform.position, p.position);
            }
        }
        else
        {
            Gizmos.DrawWireSphere(startPosition, patrolRadius);
        }

        // sensor de obstáculos (frente)
        Gizmos.color = Color.magenta;
        Vector3 origin = transform.position + Vector3.up * sensorHeight;
        Gizmos.DrawWireSphere(origin + transform.forward * obstacleDetectDistance, obstacleSphereRadius);
    }
}

// Helper extension para ignorar componente Y en vectores de dirección (mantener altura actual)
static class Vector3Extensions
{
    public static Vector3 WithY(this Vector3 v, float y)
    {
        return new Vector3(v.x, y, v.z);
    }

    public static Vector3 WithY(this Vector3 v, Vector3 other)
    {
        return new Vector3(v.x, other.y, v.z);
    }
}

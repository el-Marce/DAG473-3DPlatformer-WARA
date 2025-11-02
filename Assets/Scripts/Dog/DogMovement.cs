using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;

public class DogMovement : MonoBehaviour
{
    public Transform player; // El jugador a seguir
    private NavMeshAgent agent;

    [Header("Movimiento")]
    public float SeguirDistancia = 2f;      // Distancia mínima para dejar de moverse
    public float VelocidadMover = 3.5f;     // Velocidad de seguimiento
    public bool EstadoSeguir = true;        // Alterna seguimiento con tecla E

    [Header("Salto")]
    public float AlturaSalto = 1.5f;        // Altura del salto del perro
    public float DuracionSalto = 0.6f;      // Duración del salto
    public float UmbralDeteccionSalto = 0.6f; // Diferencia vertical mínima para imitar salto

    private bool isJumping = false;
    private Vector3 ultimoPosJugador;
    private float alturaInicialJugador;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("El objeto " + gameObject.name + " no tiene NavMeshAgent adjunto.");
            enabled = false;
            return;
        }

        agent.speed = VelocidadMover;
        if (player != null)
        {
            ultimoPosJugador = player.position;
            alturaInicialJugador = player.position.y;
        }
    }

    void Update()
    {
        if (player == null) return;

        // Alternar seguir con tecla E
        if (Input.GetKeyDown(KeyCode.E))
            EstadoSeguir = !EstadoSeguir;

        // Si no está saltando y seguir está activo
        if (EstadoSeguir && !isJumping)
        {
            float distancia = Vector3.Distance(transform.position, player.position);

            // Detectar si el jugador ha saltado (cambio de altura repentino)
            float deltaAltura = player.position.y - alturaInicialJugador;
            if (deltaAltura > UmbralDeteccionSalto && !isJumping)
            {
                StartCoroutine(SaltarDetrasJugador(player.position, DuracionSalto, AlturaSalto));
            }

            // Seguir al jugador mientras no esté demasiado cerca
            if (distancia > SeguirDistancia)
                agent.SetDestination(player.position);
            else
                agent.ResetPath();
        }

        // Actualizar referencia de altura del jugador
        if (Mathf.Abs(player.position.y - alturaInicialJugador) < 0.1f)
            alturaInicialJugador = player.position.y;

        ultimoPosJugador = player.position;
    }

    private IEnumerator SaltarDetrasJugador(Vector3 destinoJugador, float duracion, float altura)
    {
        isJumping = true;
        agent.enabled = false;

        Vector3 inicio = transform.position;
        Vector3 fin = new Vector3(destinoJugador.x, destinoJugador.y, destinoJugador.z);

        float tiempo = 0f;
        while (tiempo < duracion)
        {
            float t = tiempo / duracion;
            Vector3 pos = Vector3.Lerp(inicio, fin, t);
            pos.y += Mathf.Sin(t * Mathf.PI) * altura;
            transform.position = pos;

            tiempo += Time.deltaTime;
            yield return null;
        }

        transform.position = fin;
        agent.enabled = true;
        isJumping = false;
    }

    private void OnDrawGizmosSelected()
    {
        // Visualiza el rango de seguimiento en el editor
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, SeguirDistancia);
    }
}

    //Implementar mediante navmeshsurface, para poder soportar saltos, y otros obstaculos



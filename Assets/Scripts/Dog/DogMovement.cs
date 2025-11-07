using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class DogFollower : MonoBehaviour
{
    
    public Transform player;// detecta jugador
    private NavMeshAgent agent;// ai de seguimiento

    //Movimiento valores
    public float SeguirDistancia = 2f;
    public float VelocidadMover = 3.5f;
    public bool EstadoSeguir = true;

    //Salto
    public float AlturaSalto = 1.5f;
    public float DuracionSalto = 0.6f;

    private bool isJumping = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            enabled = false;
            return;
        }

        agent.speed = VelocidadMover;
    }

    void Update()
    {
        if (player == null) return;

        if (Input.GetKeyDown(KeyCode.Q))
            EstadoSeguir = !EstadoSeguir;

        if (EstadoSeguir && !isJumping)
        {
            float distancia = Vector3.Distance(transform.position, player.position);

            if (distancia > SeguirDistancia)
                agent.SetDestination(player.position);
            else
                agent.ResetPath();
        }
    }

    // 🐕 Este método lo llama el jugador cuando salta
    public void SaltarConJugador(Vector3 destinoJugador)
    {
        if (!isJumping)
            StartCoroutine(SaltoSincronizado(destinoJugador));
    }

    private IEnumerator SaltoSincronizado(Vector3 destinoJugador)
    {
        isJumping = true;
        agent.enabled = false;

        Vector3 inicio = transform.position;
        Vector3 fin = new Vector3(destinoJugador.x, destinoJugador.y, destinoJugador.z);

        float tiempo = 0f;
        while (tiempo < DuracionSalto)
        {
            float t = tiempo / DuracionSalto;
            Vector3 pos = Vector3.Lerp(inicio, fin, t);
            pos.y += Mathf.Sin(t * Mathf.PI) * AlturaSalto;
            transform.position = pos;

            tiempo += Time.deltaTime;
            yield return null;
        }

        transform.position = fin;
        agent.enabled = true;
        isJumping = false;
    }
}

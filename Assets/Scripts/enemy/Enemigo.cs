using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("Referencias")]
    public NavMeshAgent agente;
    public Transform jugador;

    [Header("Rangos")]
    public float rangoDeteccion = 10f;   // Detecta al jugador
    public float rangoAtaque = 2f;       // Comienza a atacar
    public float distanciaExtra = 2f;    // Margen para dejar de perseguir

    [Header("Velocidades")]
    public float velocidadPatrulla = 2f;
    public float velocidadPersecucion = 4f;

    [Header("Patrullaje")]
    public Transform[] puntosPatrulla;
    private Transform puntoActual;
    private int ultimoIndice = -1;

    [Header("Ataque")]
    public float tiempoEntreAtaques = 1.5f;
    private float contadorAtaque = 0f;

    [Header("Espera antes de patrullar")]
    public float tiempoEsperaParaPatrulla = 2f;
    private float contadorEspera = 0f;

    private bool persiguiendo = false;

    void Start()
    {
        ElegirNuevoPunto();
    }

    void Update()
    {
        float distanciaJugador = Vector3.Distance(transform.position, jugador.position);

        // Determinar si debe perseguir
        if (distanciaJugador < rangoDeteccion)
        {
            persiguiendo = true;
            contadorEspera = 0f; // reinicia contador de espera
        }
        else if (persiguiendo)
        {
            // Espera antes de volver a patrullar
            contadorEspera += Time.deltaTime;
            if (contadorEspera >= tiempoEsperaParaPatrulla)
            {
                persiguiendo = false;
                contadorEspera = 0f;
            }
        }

        // Lógica de estados
        if (!persiguiendo)
        {
            Patrullar();
        }
        else
        {
            if (distanciaJugador > rangoAtaque)
            {
                // Persigue al jugador
                agente.speed = velocidadPersecucion;
                agente.SetDestination(jugador.position);
            }
            else
            {
                // Ataca al jugador
                agente.SetDestination(transform.position); // se queda quieto
                contadorAtaque += Time.deltaTime;
                if (contadorAtaque >= tiempoEntreAtaques)
                {
                    Ataque();
                    contadorAtaque = 0f;
                }
            }
        }
    }

    private void Patrullar()
    {
        if (puntosPatrulla.Length == 0) return;

        agente.speed = velocidadPatrulla;

        if (puntoActual == null || Vector3.Distance(transform.position, puntoActual.position) < 1f)
        {
            ElegirNuevoPunto();
        }

        agente.SetDestination(puntoActual.position);
    }

    private void ElegirNuevoPunto()
    {
        if (puntosPatrulla.Length == 0) return;

        int nuevoIndice;
        do
        {
            nuevoIndice = Random.Range(0, puntosPatrulla.Length);
        } while (nuevoIndice == ultimoIndice && puntosPatrulla.Length > 1);

        ultimoIndice = nuevoIndice;
        puntoActual = puntosPatrulla[nuevoIndice];
    }

    private void Ataque()
    {
        Debug.Log("¡Atacando al jugador!");
        // Aquí puedes restar vida al jugador:
        // jugador.GetComponent<Salud>().RecibirDaño(10);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rangoDeteccion);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, rangoAtaque);
    }
}

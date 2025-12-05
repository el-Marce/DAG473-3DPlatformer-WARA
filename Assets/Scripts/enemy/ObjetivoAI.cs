using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ObjetivoAI : MonoBehaviour
{
    public NavMeshAgent Enemigo;

    [Header("Movimiento")]
    public float Velocidad;
    public bool Persiguiendo;
    public float Rango;
    public float Distancia;
    public float DistanciaExtra = 2;


    public Transform objetivo;

    [Header("Objetivos")]
    public float velocidadPatrullaje;
    public Transform[] Objetivos;
    Transform ObjetivoEspecifico;
    float DistanciaObjetivoEspecifico;


    [Header("Ataque")]
    public float DistanciaAtaque = 1.5f; // rango para atacar
    public float tiempoEntreAtaques = 1.2f; // segundos entre ataques
    private float temporizadorAtaque = 0f;
    public bool atacando;

    //header

    private void Start()
    {
        ObjetivoEspecifico = Objetivos[Random.Range(0, Objetivos.Length)];
    }


    private void Update()
    {
        Distancia = Vector3.Distance(Enemigo.transform.position, objetivo.position);

        if (Distancia < Rango)
        {
            Persiguiendo = true;
        }
        else if (Distancia > Rango + DistanciaExtra)
        {
            Persiguiendo = false;
        }


        if(Persiguiendo == false)
        {
            Enemigo.speed = 0;
            patrullaje();
        }
        else if (Persiguiendo == true)
           {
            Enemigo.speed = Velocidad;
            Enemigo.SetDestination(objetivo.position);

            // Verificar si está en rango de ataque
            if (Distancia <= DistanciaAtaque)
            {
                // Mira hacia el jugador
                //Enemigo.transform.LookAt(objetivo);

                // Detiene el movimiento para atacar
                Enemigo.speed = 0;

                // Contador para evitar ataques continuos
                temporizadorAtaque -= Time.deltaTime;
                if (temporizadorAtaque <= 0f)
                {
                    Ataque();
                    temporizadorAtaque = tiempoEntreAtaques;
                }
            }
        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(Enemigo.transform.position, Rango);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(Enemigo.transform.position, DistanciaAtaque);
    }

    private void patrullaje()
    {
        DistanciaObjetivoEspecifico = Vector3.Distance(transform.position, ObjetivoEspecifico.position);

        if (DistanciaObjetivoEspecifico < 2)
        {
            ObjetivoEspecifico = Objetivos[Random.Range(0, Objetivos.Length)];
        }

        Enemigo.destination = ObjetivoEspecifico.position;
        Enemigo.speed = velocidadPatrullaje;

    }

    public void Ataque()
    {
        Debug.Log("Atacando");
        //lógica para atacar
        //encontrar codigo de salud del jugador y restar vida
        //objetivo.GetComponent<Salud>().RecibirDaño(10);

    }

}

//detectar enemigo
//acciones enemigo patrullar enemigo
//para el pretullaje usar puntos de referencia o puntos aleatorios, toamr en cuenta que no salga de un area o que quiera ir a partes del mapa que no deberia ir
//implementar un area para que el enemigo detecte si el jugador esta cerca o salio del rango del enemigo
//el nav mesh debe ser paa todo el mapa no solo una area
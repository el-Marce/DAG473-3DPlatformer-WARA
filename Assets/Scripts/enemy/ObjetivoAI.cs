using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ObjetivoAI : MonoBehaviour
{
    public Transform Target;
    public float DistanciaAtaque;
    private NavMeshAgent m_Agent;
    private float m_Distancia;


    // Start is called before the first frame update
    void Start()
    {
        m_Agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        m_Distancia = Vector3.Distance(m_Agent.transform.position, Target.position);
        if (m_Distancia < DistanciaAtaque)
        {
            m_Agent.isStopped = true;
            Debug.Log("EnemigoAtaco.");
        }
        else
        {
            m_Agent.isStopped = false;
            m_Agent.destination = Target.position;
        }
    }
}

//detectar enemigo
//acciones enemigo patrullar enemigo
//para el pretullaje usar puntos de referencia o puntos aleatorios, toamr en cuenta que no salga de un area o que quiera ir a partes del mapa que no deberia ir
//implementar un area para que el enemigo detecte si el jugador esta cerca o salio del rango del enemigo
//el nav mesh debe ser paa todo el mapa no solo una area
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
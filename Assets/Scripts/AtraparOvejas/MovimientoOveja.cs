using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MovimientoOveja : MonoBehaviour
{
    [SerializeField] NavMeshAgent AI;
    [SerializeField] public float velocidad;
    [SerializeField] public Transform[] Objetivos;
    Transform Objetivo;
    [SerializeField] public float Distancia;

    void Start()
    {
      Objetivo = Objetivos[Random.Range(0, Objetivos.Length)];
    }

    void Update()
    {
        Distancia = Vector3.Distance(transform.position, Objetivo.position);

        if (Distancia < 2f)
        {
            Objetivo = Objetivos[Random.Range(0, Objetivos.Length)];
        }

        AI.destination = Objetivo.position;
        AI.speed = velocidad;
    }
}

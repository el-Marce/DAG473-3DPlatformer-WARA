using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ControladorOveja : MonoBehaviour
{
    private NavMeshAgent agenteOveja;

    void Start()
    {
        agenteOveja = GetComponent<NavMeshAgent>();

        // 1. Evita la rotación vertical/volteo (Eje X-Arriba)
        agenteOveja.updateUpAxis = false;

        // 2. EVITA TODA ROTACIÓN HORIZONTAL (Mirar al destino)
        // Esto mantendrá tu objeto mirando en la misma dirección inicial.
        agenteOveja.updateRotation = false;
    }
}

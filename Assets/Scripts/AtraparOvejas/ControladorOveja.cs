using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class ControladorOveja : MonoBehaviour
{
    private NavMeshAgent agenteOveja;

    void Start()
    {
        // 1. Obtener el componente NavMeshAgent
        agenteOveja = GetComponent<NavMeshAgent>();

        // 2. Desactivar la alineación automática del eje "Arriba"
        agenteOveja.updateUpAxis = false;
        agenteOveja.updateRotation = false;
    }

    void Update()
    {
        // 1. Obtener la velocidad deseada que el NavMeshAgent está calculando.
        Vector3 direccionDeseada = agenteOveja.desiredVelocity;

        // 2. Solo rotar si hay movimiento (si la dirección no es cero).
        if (direccionDeseada.sqrMagnitude > 0.01f)
        {
            // 3. Crear una rotación basada en la dirección, ASUMIENDO Y-UP (estándar).
            // Usamos Quaternion.LookRotation para obtener la rotación que miraría hacia adelante.
            Quaternion rotacionEstandar = Quaternion.LookRotation(direccionDeseada);

            // 4. Aplicar la compensación de 90 grados.
            // Esto "traduce" la rotación Y-Up (estándar) a tu rotación X-Up.
            // (La compensación exacta puede ser +90 o -90 en Z o Y, ajústala si la oveja mira a un lado).
            Quaternion compensacion = Quaternion.Euler(0, 180, +90);

            // 5. Aplicar la rotación final combinada con suavidad (Slerp)
            // Slerp suaviza el giro para que no sea instantáneo.
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                rotacionEstandar * compensacion,
                Time.deltaTime * 10f // El '10f' controla la velocidad del giro
            );
        }
    }


}


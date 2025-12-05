using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZonaSegura : MonoBehaviour
{
    [Header("Configuración del Corral")]
    [SerializeField] private Transform puntoDeEntrega;
    [SerializeField] private float radioDeDistribucion = 2f;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.instance.EntregarOvejas();
        }
        else if (other.CompareTag("Oveja"))
        {
            EntregarOvejaEnPosicion(other.gameObject);
        }
    }

    public void EntregarOvejaEnPosicion(GameObject oveja)
    {
        // Primero actualizar estado de la oveja
        Ovejas scriptOveja = oveja.GetComponent<Ovejas>();
        if (scriptOveja != null)
            scriptOveja.Entregar();

        // Posicionar en el corral
        if (puntoDeEntrega != null)
        {
            Vector3 posicionAleatoria = CalcularPosicionEnCorral();
            oveja.transform.position = posicionAleatoria;
        }
    }

    private Vector3 CalcularPosicionEnCorral()
    {
        Vector3 posicionBase = puntoDeEntrega != null ? puntoDeEntrega.position : transform.position;

        float angulo = Random.Range(0f, 360f);
        float distancia = Random.Range(0f, radioDeDistribucion);

        float x = posicionBase.x + distancia * Mathf.Cos(angulo * Mathf.Deg2Rad);
        float z = posicionBase.z + distancia * Mathf.Sin(angulo * Mathf.Deg2Rad);

        return new Vector3(x, posicionBase.y, z);
    }

    private void OnDrawGizmosSelected()
    {
        if (puntoDeEntrega != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(puntoDeEntrega.position, radioDeDistribucion);
        }
    }
}

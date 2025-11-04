using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemigoController : MonoBehaviour
{
    [Header("Daño que causa al jugador")]
    public int dano = 20;

    //[Header("Fuerza de empuje")]
    //public float fuerzaEmpuje = 5f;

    private void OnTriggerEnter(Collider other)
    {
        // Verifica si el objeto que entra tiene el sistema de salud
        SistemaDeSalud salud = other.GetComponent<SistemaDeSalud>();
        if (salud != null)
        {
            salud.RecibirDanio(dano);
            StartCoroutine(EsperarAntesDePoderAtacar());
            Debug.Log("Jugador recibió daño: " + dano);

            /*Rigidbody rbJugador = other.GetComponent<Rigidbody>();
            if (rbJugador != null)
            {
                // Dirección del empuje = desde el enemigo hacia el jugador
                Vector3 direccion = (other.transform.position - transform.position).normalized;

                // Aplicar fuerza instantánea (impulso)
                rbJugador.AddForce(direccion * fuerzaEmpuje, ForceMode.Impulse);
            }*/
        }
    }

    private IEnumerator EsperarAntesDePoderAtacar()
    {
        GetComponent<Collider>().enabled = false;
        yield return new WaitForSeconds(1f); // 1 segundo de espera
        GetComponent<Collider>().enabled = true;
    }
}

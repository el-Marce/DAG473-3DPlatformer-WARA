using System.Collections;
using UnityEngine;

public class EnemyAplastador : MonoBehaviour
{
    [Header("Referencias")]
    public Transform jugador;

    [Header("Aplastamiento")]
    public GameObject cuboAplastador;   // Prefab del cubo
    public float alturaSpawn = 5f;       // Altura desde donde aparece
    public float tiempoPreparacion = 1f; // Pausa antes de soltar el cubo

    [Header("Frecuencia de ataque")]
    public float tiempoEntreAtaques = 2f;

    private bool atacando = false;

    [SerializeField] ObjetivoAI enemigo;
    void Update()
    {
        if (!atacando && enemigo.Persiguiendo)
        {
            StartCoroutine(AtaqueAplastamiento());
        }
    }

    IEnumerator AtaqueAplastamiento()
    {
        atacando = true;

        // Mira al jugador
        transform.LookAt(jugador);

        // Pausa antes de crear el cubo
        yield return new WaitForSeconds(tiempoPreparacion);

        // Crear cubo encima del jugador
        Vector3 posicion = jugador.position + Vector3.up * alturaSpawn;
        Instantiate(cuboAplastador, posicion, Quaternion.identity);

        // Espera el tiempo entre ataques
        yield return new WaitForSeconds(tiempoEntreAtaques);

        atacando = false;
    }
}

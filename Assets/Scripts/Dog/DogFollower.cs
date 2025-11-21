using System.Collections;
using UnityEngine;
using UnityEngine.AI;
public class DogFollower : MonoBehaviour
{
    public Transform jugador;
    public NavMeshAgent navAgent;
    public float distanciaSeguir = 2f;
    public float VelocidadCaminar = 3.5f;
    public bool EstadoSeguir = true;

    public float AlturaSalto = 1.5f;
    public float DuracionSalto = 0.6f;
    private bool isJumping = false;
    // Start is called before the first frame update
    void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        if (navAgent == null)
        {
            enabled = false;
            return;
        }
        navAgent.speed = VelocidadCaminar;
    }

    // Update is called once per frame
    void Update()
    {
        if (jugador == null || isJumping) return;
            if (EstadoSeguir)
          {
            float distancia = Vector3.Distance(transform.position, jugador.position);
            if (distancia > distanciaSeguir)
            {
                navAgent.SetDestination(jugador.position);
            }
            else
            {
                navAgent.ResetPath();
            }  
          }
    }
    public void SaltarConJugador(Vector3 destinoJugador)
    { 
      if (!isJumping)
        {
            StartCoroutine(SaltoSincronizado(destinoJugador));
        }
    }
    private IEnumerator SaltoSincronizado(Vector3 destinoJugador)
    {
        isJumping = true;
        navAgent.enabled = false;

        Vector3 inicio = transform.position;
        Vector3 fin = destinoJugador;

        float tiempoTranscurrido = 0f;
        while (tiempoTranscurrido < DuracionSalto)
        { float t = tiempoTranscurrido / DuracionSalto;
            Vector3 pos = Vector3.Lerp(inicio, fin, t);
            pos.y += Mathf.Sin(t * Mathf.PI) * AlturaSalto;
            transform.position = pos;
            tiempoTranscurrido += Time.deltaTime;
            yield return null;
        }
        transform.position = fin;
        navAgent.enabled = true;
        isJumping = false;
    }
}

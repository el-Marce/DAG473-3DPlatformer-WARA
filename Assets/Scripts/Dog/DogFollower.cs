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

    private float jugadorAlturaAnterior = 0f;
    private float umbralDeteccionSalto = 0.12f;
    public float RetrasoSalto = 0.10f;
    public float fuerzaEmpuje = 4f;
    public DogCombat combatePerro;
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
        if (jugador != null)
        {
            jugadorAlturaAnterior = jugador.position.y;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (jugador == null) return;
        DetectarSaltoDelJugador ();
        if (isJumping) return;

        if (EstadoSeguir)
        {
            float distancia = Vector3.Distance(transform.position, jugador.position);
            if (distancia > distanciaSeguir)
            navAgent.SetDestination(jugador.position);
            else
            {
                navAgent.ResetPath();
            }
            if (combatePerro != null)
            {
               combatePerro.Atacar();
            }
        }
    }
    void DetectarSaltoDelJugador()
    {
        float alturaActual = jugador.position.y;
        float velocidadVertical = alturaActual - jugadorAlturaAnterior;

        if (!isJumping && velocidadVertical > umbralDeteccionSalto)
        {
            StartCoroutine(SaltarConRetraso(jugador.position)); 
        }
        jugadorAlturaAnterior = alturaActual;
    }
    private IEnumerator SaltarConRetraso(Vector3 destino)
    {
        yield return new WaitForSeconds (RetrasoSalto);
        if (!isJumping)
        SaltarConJugador(destino);
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
        Vector3 fin = new Vector3(destinoJugador.x, transform.position.y, destinoJugador.z);

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
    private void OnTriggerStay (Collider other)
    {
        if (!other.CompareTag("Player")) return;
        CharacterController cc = other.GetComponent<CharacterController>();
        if (cc == null)
        cc = other.GetComponentInParent<CharacterController>();
        if (cc == null) return;
        Vector3 dir = other.transform.position - transform.position;
        dir.y = 0;
        dir.Normalize();
        cc.Move(dir * fuerzaEmpuje * Time.deltaTime);
    }
}
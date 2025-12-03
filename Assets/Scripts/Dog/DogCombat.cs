using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DogCombat : MonoBehaviour
{
    [Header("Referencias")] public DogFollower follower; 
    private UnityEngine.AI.NavMeshAgent navAgent; 
    private Transform jugador; 
    [Header("Ataque")] public float rangoAtaque = 2f; 
    public float rangoBusqueda = 12f; 
    public int daño = 10; 
    public float tiempoRecarga = 1f; 
    private bool puedeAtacar = true; 
    private Transform objetivo;
    // Start is called before the first frame update
    void Start()
    {
        navAgent = follower.navAgent; jugador = follower.jugador;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Atacar() 
    { 
        if (!puedeAtacar) return; 
        objetivo = BuscarEnemigoCercano(); 
        if (objetivo == null) 
        { 
            Debug.Log ("No hay enemigos para atacar."); return; 
        } 
        follower.EstadoSeguir = false; 
        navAgent.isStopped = false; 
        navAgent.SetDestination(objetivo.position); 
        StartCoroutine(PerseguirYAtacar()); 
    } 
    private Transform BuscarEnemigoCercano() 
    { 
    Collider[] hits = Physics.OverlapSphere(transform.position,rangoBusqueda,LayerMask.GetMask("Enemy"));
    if (hits.Length == 0) return null; 
    Transform mejor = hits[0].transform; 
    float mejorDist = Vector3.Distance(transform.position, mejor.position); 
    foreach (Collider c in hits) 
    { float d = Vector3.Distance(transform.position, c.transform.position); 
     if (d < mejorDist) 
     { 
        mejor = c.transform; mejorDist = d; 
     } 
    } 
      return mejor; 
    } 
    private IEnumerator PerseguirYAtacar() 
    { 
        if (!puedeAtacar) yield break; 
        while (objetivo != null && Vector3.Distance(transform.position, objetivo.position) > rangoAtaque) 
        { 
            navAgent.SetDestination(objetivo.position); yield return null; 
        } 
        navAgent.isStopped = true; 
        if(objetivo != null) 
        { 
            Debug.Log($" ¡El perro ataca a {objetivo.name}! Daño: {daño}"); 
        } 
        StartCoroutine(CooldownAtaque()); 
        yield return new WaitForSeconds(0.25f); 
        follower.EstadoSeguir = true; 
    } 
    private IEnumerator CooldownAtaque() 
    { 
        puedeAtacar = false; 
        yield return new WaitForSeconds(tiempoRecarga); 
        puedeAtacar = true; 
    }
}
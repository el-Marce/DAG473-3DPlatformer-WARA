using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DogCombat : MonoBehaviour
{
    [Header("Ataque")] public float rangoAtaque = 2f;
    public int daño = 10;
    public float tiempoRecarga = 1f;
    private bool puedeAtacar = true; 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Atacar()
    {
        if (!puedeAtacar) return;
        Collider[] enemigos = Physics.OverlapSphere(transform.position, rangoAtaque, LayerMask.GetMask("Enemy"));
        if (enemigos.Length == 0)
        {
            Debug.Log("No hay enemigos cerca para atacar");
        }
        else
        {
            foreach (Collider enemigo  in enemigos)
            {
                Debug.Log ($"¡El perro ataca a {enemigo.name}! Daño {daño} ");
            }
        }
        StartCoroutine(CooldownAtaque());
    }
    private IEnumerator CooldownAtaque()
    {
        puedeAtacar = false;
        yield return new WaitForSeconds(tiempoRecarga);
        puedeAtacar = true;
    }
}
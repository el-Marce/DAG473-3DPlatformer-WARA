using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonedaScript : MonoBehaviour

{

    [SerializeField] private float velocidadRotacion = 100f;
    [SerializeField] private int valor = 1;

    void Start()
    {

    }
    void Update()
    {
        transform.Rotate(Vector3.up * velocidadRotacion * Time.deltaTime, Space.World);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Ejemplo de lógica básica
            GameManager.instance.SumarMonedas(valor);
            //FMODUnity.RuntimeManager.PlayOneShot("event:/Moneda/Recoleccion");
            Destroy(gameObject);
        }
    }
}

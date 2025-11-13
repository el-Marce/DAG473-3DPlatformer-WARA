using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecolectarOvejas : MonoBehaviour
{
    [SerializeField] public float rangoRecoleccion = 5f;
    [SerializeField] KeyCode teclaRecolectar = KeyCode.E;
    [SerializeField] public int totalOvejasRecolectadas = 0;
    //[SerializeField] public bool llevandoOvejas = false;
    
    Ovejas ovejasActuales = null;
   
    // Update is called once per frame
    void Update()
    {
        if(ovejasActuales != null && Input.GetKeyDown(teclaRecolectar))
        {
            ovejasActuales.Recoger();
            //llevandoOvejas = true;
            totalOvejasRecolectadas++;

            GameManager.instance.RecogerOvejas();
            SoundManager.instance.reproducirRecogerOveja();
            //Debug.Log("Ovejas recolectadas: " + totalOvejasRecolectadas);
            ovejasActuales = null;
        }
    }

    private void OnTriggerEnter(Collider other) 
    {
        if(other.CompareTag("Oveja"))
        {
            ovejasActuales = other.GetComponent<Ovejas>();
            ovejasActuales.MostrarIndicador(true);
            //Debug.Log("Precione E para recoger Oveja.");
        }
    }

    private void OnTriggerExit(Collider other) 
    {
        if (other.CompareTag("Oveja"))
        {
            Ovejas oveja = other.GetComponent<Ovejas>();
            if (oveja != null)
                oveja.MostrarIndicador(false);

            if (ovejasActuales != null && other.gameObject == ovejasActuales.gameObject)
            {
                //Debug.Log("Saliste del rango de recolección de la oveja.");
                ovejasActuales = null;
            }
        }
    }
}

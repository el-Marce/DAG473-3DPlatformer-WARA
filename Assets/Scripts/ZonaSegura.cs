using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZonaSegura : MonoBehaviour
{
     [Header("Numero de ovejas para ganar")]
    public int OvejasRequeridas = 3; 
    private int OvejaEnZona = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Oveja"))
        {
            OvejaEnZona++;
            Debug.Log("Oveja dentro. Total en zona: " + OvejaEnZona);

            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
                rb.isKinematic = true;
            other.transform.position = new Vector3(
                transform.position.x, 
                other.transform.position.y, 
                transform.position.z
            );

            CheckWin();    
        }
    }
    void CheckWin()
    {
        if (OvejaEnZona >= OvejasRequeridas)
        {
            Debug.Log("Ganaste el juego");
        }
    }                           
}
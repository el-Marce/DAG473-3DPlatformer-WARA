using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolverMenuSimple : MonoBehaviour
{
    GameObject canvasMenu;
    GameObject instrucciones;

    void Start()
    {
        canvasMenu = GameObject.Find("Canvas");
        instrucciones = GameObject.Find("Instrucciones");
    }

    void Update()
    {
        if (Input.anyKeyDown)
        {
            if (canvasMenu != null) canvasMenu.SetActive(true);
            if (instrucciones != null) instrucciones.SetActive(false);
        }
    }
}



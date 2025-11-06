using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ovejas : MonoBehaviour
{
    [SerializeField] public bool atrapada = false;
    
    public void Recoger ()
    {
        if(!atrapada)
        {
            atrapada = true;
            gameObject.SetActive(false);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour    
{
    public static GameManager instance;

    [Header("Contador de monedas")]
    public int monedasTotales = 0;


    [SerializeField] public TMP_Text textoMonedas;

    private void Awake()
    {
        // Patrón Singleton
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);

        Debug.Log("iniciado. Monedas: " + monedasTotales);
    }
    public void Update()
    {
        ActualizarUI();
    }
    public void SumarMonedas(int cantidad)
    {
        monedasTotales += cantidad;
        Debug.Log("Monedas totales: " + monedasTotales);
        
    }

    private void ActualizarUI()
    {
        if (textoMonedas != null)
            textoMonedas.text = "Monedas: " + monedasTotales.ToString();
    }
}

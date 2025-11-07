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

    [Header("Contador de ovejas")]
    public int ovejasTotales = 0;   
    [SerializeField] public TMP_Text textoOvejas;

    [SerializeField] public TMP_Text textoTodasLasOvejas;
    private int totalOvejasEnEscena;


    public bool ovejasFaltantes
    {
        get { return ovejasTotales < totalOvejasEnEscena; }
    }
    private void Awake()
    {
        // Patrón Singleton
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);

        textoTodasLasOvejas.gameObject.SetActive(false);
        CalcularTotalOvejas();
        ActualizarUI();
    }
    public void Update()
    {
        ActualizarUI();
    }
    private void CalcularTotalOvejas()
    {
        // Busca todos los objetos con el tag "Oveja"
        totalOvejasEnEscena = GameObject.FindGameObjectsWithTag("Oveja").Length;
        Debug.Log("Total de ovejas en escena: " + totalOvejasEnEscena);
    }
    public void SumarMonedas(int cantidad)
    {
        monedasTotales += cantidad;
        //Debug.Log("Monedas totales: " + monedasTotales);
    }
    public void SumarOvejas()
    {
        ovejasTotales++;
        //Debug.Log("Ovejas totales: " + ovejasTotales);
        if (ovejasTotales >= totalOvejasEnEscena)
        {
            StartCoroutine(MostrarTextoTemporal(textoTodasLasOvejas, 5f));
        }
    }
    private IEnumerator MostrarTextoTemporal(TMP_Text texto, float duracion)
    {
        texto.gameObject.SetActive(true);
        yield return new WaitForSeconds(duracion);
        texto.gameObject.SetActive(false);
    }
    private void ActualizarUI()
    {
        if (textoMonedas != null)
            textoMonedas.text = $"Monedas: {monedasTotales}";

        if (textoOvejas != null)
            textoOvejas.text = $"Ovejas: {ovejasTotales}/{totalOvejasEnEscena}";
    }
}

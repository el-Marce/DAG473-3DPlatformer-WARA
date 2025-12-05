using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public List<GameObject> ovejasRecolectadas = new List<GameObject>();
    //private List<Ovejas> ovejasEntregadas = new List<Ovejas>();


    public ZonaSegura zonaSegura;
    public static GameManager instance;

    [Header("Contador de monedas")]
    public int monedasTotales = 0;
    [SerializeField] public TMP_Text textoMonedas;

    [Header("Contador de ovejas")]
    public int ovejasRecogidas = 0;
    private int totalOvejasEnEscena;
    public int ovejasEnZonaSegura;
    [SerializeField] public TMP_Text textoOvejas;
    [SerializeField] public TMP_Text textoTodasLasOvejas;

    public bool jugadorConOvejas
    {
        get { return ovejasRecogidas > 0; }
    }

    public bool ovejasFaltantes
    {
        get { return ovejasRecogidas < totalOvejasEnEscena; }
    }

    private void Awake()
    {
        instance = this;

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
        totalOvejasEnEscena = GameObject.FindGameObjectsWithTag("Oveja").Length;
        Debug.Log("Total de ovejas en escena: " + totalOvejasEnEscena);
    }

    public void SumarMonedas(int cantidad)
    {
        monedasTotales += cantidad;
    }

    public void RegistrarOveja(GameObject oveja)
    {
        if (!ovejasRecolectadas.Contains(oveja))
            ovejasRecolectadas.Add(oveja);
    }

    public void RecogerOvejas()
    {
        ovejasRecogidas++;
    }

    public void PerderOvejas()
    {
        ovejasRecogidas = 0;

        foreach (var o in ovejasRecolectadas)
        {
            o.SetActive(true);
            o.GetComponent<Ovejas>().atrapada = false; 
        }
        ovejasRecolectadas.Clear();
    }

    public int EntregarOvejas()
    {
        int entregadas = ovejasRecogidas;
        if (jugadorConOvejas)
        {
            ovejasEnZonaSegura += ovejasRecogidas;
            ovejasRecogidas = 0;

            foreach (var o in ovejasRecolectadas)
            {
                o.transform.position = zonaSegura.transform.position;
                o.SetActive(true);
            }
            ovejasRecolectadas.Clear();


            Debug.Log($"Ovejas en zona segura: {ovejasEnZonaSegura}/{totalOvejasEnEscena}");

            if (ovejasEnZonaSegura >= totalOvejasEnEscena)
            {
                GanarJuego();
            }
        }
        return entregadas;
    }

    void GanarJuego()
    {
        StartCoroutine(MostrarTextoTemporal(textoTodasLasOvejas, 5f));
        Debug.Log("¡Ganaste el juego!");
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
            textoOvejas.text = $"Ovejas: {ovejasEnZonaSegura}/{totalOvejasEnEscena}";
    }
}

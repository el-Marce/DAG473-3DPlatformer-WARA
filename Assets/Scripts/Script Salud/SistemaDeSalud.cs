using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using StarterAssets;
using TMPro;

public class SistemaDeSalud : MonoBehaviour
{
    [Header("Parámetros de salud")]
    public int saludMaxima = 200;
    public int saludActual;

    [Header("Vidas")]
    public int vidas = 3;
    public Image[] iconosVidas; // Íconos de ovejas

    [Header("Barra de vida")]
    public Slider barraDeVida;

    [Header("Reaparición")]
    public Vector3 posicionInicial = new Vector3(-7.117675f, 0.22f, 9.667f);
    public float tiempoReaparicion = 3f;

    [Header("UI Game Over")]
    public TMP_Text textoGameOver;
    private bool estaMuerto = false;
    private ThirdPersonController controlador;

    void Start()
    {
        saludActual = saludMaxima;
        barraDeVida.maxValue = saludMaxima;
        barraDeVida.value = saludActual;

        controlador = GetComponent<ThirdPersonController>();

        if(textoGameOver != null)
            textoGameOver.gameObject.SetActive(false);
    }

    void Update()
    {
        barraDeVida.value = saludActual;

        // Solo para testeo: presiona H para recibir daño
        if (Input.GetKeyDown(KeyCode.H))
        {
            RecibirDanio(20);
        }
    }

    // Método para aplicar daño al jugador
    public void RecibirDanio(int cantidad)
    {
        if (estaMuerto) return;

        saludActual -= cantidad;

        if (saludActual <= 0)
        {
            saludActual = 0;
            vidas--;
            ActualizarVidasUI();

            if (vidas > 0)
            {
                StartCoroutine(MorirYReaparecer());
            }
            else
            {
                // Última vida perdida: bloquear al jugador
                GameOver();
            }
        }
    }

    // Corrutina para simular la muerte y reaparición
    private IEnumerator MorirYReaparecer()
    {
        estaMuerto = true;

        if (controlador != null)
            controlador.enabled = false; // Desactivar movimiento

        // Desactivar el renderizado del personaje
        Renderer[] renderizadores = GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderizadores)
            r.enabled = false;

        yield return new WaitForSeconds(tiempoReaparicion);

        // Reaparecer en la posición inicial
        transform.position = posicionInicial;
        saludActual = saludMaxima;
        barraDeVida.value = saludActual;

        // Reactivar renderizado y movimiento
        foreach (Renderer r in renderizadores)
            r.enabled = true;

        if (controlador != null)
            controlador.enabled = true;

        estaMuerto = false;
    }

    // Actualiza los íconos de las vidas (ovejas)
    private void ActualizarVidasUI()
    {
        for (int i = 0; i < iconosVidas.Length; i++)
        {
            iconosVidas[i].enabled = i < vidas;
        }
    }

    // Maneja la situación de Game Over
    private void GameOver()
    {
        estaMuerto = true;

        // Bloquea movimiento
        if (controlador != null)
            controlador.enabled = false;

        // Mostrar texto de Game Over
        if (textoGameOver != null)
            textoGameOver.gameObject.SetActive(true);

        Debug.Log("¡Juego terminado!");
    }
}


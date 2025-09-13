using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CanvasUIController : MonoBehaviour
{
    public CanvasGroup panel;   // Asigna aquí el CanvasGroup de Buttons
    public float waitDuration = 2f;    // Tiempo en negro antes de mostrar botones
    public float fadeDuration = 5f;    // Duración del fade in de los botones

    private Coroutine currentFade;

    void Start()
    {
        panel.alpha = 0f; // botones ocultos al inicio
        currentFade = StartCoroutine(FadeSequence());
    }

    void Update()
    {
        // Si el jugador presiona espacio, saltamos todo y mostramos botones de una
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (currentFade != null)
            {
                StopCoroutine(currentFade);
                currentFade = null;
            }
            panel.alpha = 1f; // mostrar botones inmediatamente
        }
    }

    private IEnumerator FadeSequence()
    {
        // 1. Espera en negro (botones ocultos)
        float t = 0f;
        while (t < waitDuration)
        {
            t += Time.deltaTime;
            yield return null;

            if (Input.GetKey(KeyCode.Space))
                yield break;
        }

        // 2. Fade in de botones
        t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            panel.alpha = Mathf.Lerp(0f, 1f, t / fadeDuration);
            yield return null;

            if (Input.GetKey(KeyCode.Space))
            {
                panel.alpha = 1f;
                yield break;
            }
        }

        panel.alpha = 1f; // asegúrate de que quede completamente visible
    }
}

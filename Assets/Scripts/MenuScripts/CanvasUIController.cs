using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro; // si usas TextMeshPro

public class CanvasUIController : MonoBehaviour
{
    [Header("Fade del panel principal")]
    public CanvasGroup panel;
    public float waitDuration = 2f;    // Tiempo en negro antes del fade
    public float fadeDuration = 5f;    // Duración del fade in

    [Header("Texto de 'Presiona Espacio'")]
    public TMP_Text textoEspacio;       // Arrastra aquí tu texto
    public float blinkSpeed = 1f;       // Velocidad del parpadeo

    private Coroutine currentFade;
    private Coroutine blinkCoroutine;

    void Start()
    {
        panel.alpha = 0f;

        if (textoEspacio != null)
        {
            textoEspacio.gameObject.SetActive(true);
            textoEspacio.alpha = 0f; // invisible al inicio
        }

        currentFade = StartCoroutine(FadeSequence());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Detiene cualquier animación y muestra todo al instante
            if (currentFade != null)
                StopCoroutine(currentFade);

            if (blinkCoroutine != null)
                StopCoroutine(blinkCoroutine);

            panel.alpha = 1f;
            if (textoEspacio != null)
                textoEspacio.gameObject.SetActive(false);
        }
    }

    private IEnumerator FadeSequence()
    {
        // 1️⃣ Espera inicial con texto parpadeando
        if (textoEspacio != null)
            blinkCoroutine = StartCoroutine(BlinkText());

        float t = 0f;
        while (t < waitDuration)
        {
            t += Time.deltaTime;
            yield return null;

            if (Input.GetKey(KeyCode.Space))
                yield break;
        }

        // 2️⃣ Fade in del panel
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

        // 3️⃣ Oculta el texto de “Presiona Espacio” al finalizar
        if (blinkCoroutine != null)
            StopCoroutine(blinkCoroutine);
        if (textoEspacio != null)
            textoEspacio.gameObject.SetActive(false);
    }

    private IEnumerator BlinkText()
    {
        textoEspacio.alpha = 0f;
        while (true)
        {
            // Parpadeo con seno suave (0 → 1 → 0)
            textoEspacio.alpha = Mathf.Abs(Mathf.Sin(Time.time * blinkSpeed));
            yield return null;
        }
    }
}

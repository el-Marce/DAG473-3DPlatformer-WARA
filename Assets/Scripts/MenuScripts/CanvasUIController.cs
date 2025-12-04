using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CanvasUIController : MonoBehaviour
{
    [Header("Fade del panel principal")]
    public CanvasGroup panel;
    public float waitDuration = 2f;    // Tiempo antes del fade
    public float fadeDuration = 5f;    // Duración del fade in

    [Header("Animación inicial de la imagen")]
    public RectTransform imagen;       // Aquí arrastras tu Image (la ilustración)
    public float animDuration = 1.5f;  // Duración de la animación
    public float zoomAmount = 1.05f;   // Pequeño zoom (1.0 = sin zoom)
    public Vector2 moveOffset = new Vector2(10f, 10f); // Movimiento sutil

    private Coroutine currentFade;

    void Start()
    {
        panel.alpha = 0f;

        // Comienza la animación de imagen + fade
        currentFade = StartCoroutine(FullSequence());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (currentFade != null)
                StopCoroutine(currentFade);

            panel.alpha = 1f;

            if (imagen != null)
            {
                imagen.localScale = Vector3.one;
                imagen.anchoredPosition = Vector2.zero;
            }
        }
    }

    private IEnumerator FullSequence()
    {
        // 1️⃣ ANIMACIÓN INICIAL DE MOVIMIENTO / ZOOM
        if (imagen != null)
            yield return StartCoroutine(AnimateImage());

        // 2️⃣ Espera inicial antes del fade
        float t = 0f;
        while (t < waitDuration)
        {
            t += Time.deltaTime;
            yield return null;

            if (Input.GetKey(KeyCode.Space))
                yield break;
        }

        // 3️⃣ Fade in del panel
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
    }


    private IEnumerator AnimateImage()
    {
        Vector3 startScale = Vector3.one;
        Vector3 endScale = Vector3.one * zoomAmount;

        Vector2 startPos = Vector2.zero;
        Vector2 endPos = moveOffset;

        float t = 0f;

        while (t < animDuration)
        {
            t += Time.deltaTime;
            float lerp = t / animDuration;

            imagen.localScale = Vector3.Lerp(startScale, endScale, lerp);
            imagen.anchoredPosition = Vector2.Lerp(startPos, endPos, lerp);

            yield return null;
        }
    }
}

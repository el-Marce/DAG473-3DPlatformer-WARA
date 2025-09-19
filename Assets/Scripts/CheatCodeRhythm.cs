using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System.Collections;

public class CheatCodeRhythm : MonoBehaviour
{
    public float bpm = 90f;
    public int requiredRepeats = 2;
    public float tolerance = 0.15f; // margen de error en segundos
    public float maxInterval = 1f; // tiempo máximo entre clics

    [Header("UI Debug")]
    public TextMeshProUGUI debugText; // asigna tu TMP Text aquí
    public float debugDuration = 1f; // duración en segundos

    private List<float> clickTimes = new List<float>();
    private float beatDuration;
    private float lastClickTime = 0f;

    // Patrón en beats: negra(1), negra(1), corchea(0.5), corchea(0.5), negra(1)
    private float[] pattern = { 1f, 1f, 0.5f, 0.5f, 1f };

    void Start()
    {
        debugText.enabled = false;
        beatDuration = 60f / bpm;
    }
    void Update()
    {
        /*if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Click detectado por Input");
            RegisterClick();
        }*/
    }


    public void RegisterClick()
    {
        float now = Time.time;

        // si pasó demasiado tiempo desde el último clic, reset
        if (lastClickTime > 0 && now - lastClickTime > maxInterval)
        {
            clickTimes.Clear();
        }

        clickTimes.Add(now);
        lastClickTime = now;

        // Debug para ver los tiempos de clic
        Debug.Log("Clic registrado en: " + now + "s");

        if (CheckPattern())
        {
            Debug.Log("¡Cheat code activado!");
            ShowDebugMessage("¡Cheat code activado!");
            // acción del cheat code
            clickTimes.Clear(); // resetear para siguiente intento
            lastClickTime = 0f;
        }
    }

    private bool CheckPattern()
    {
        for (int start = 0; start <= clickTimes.Count - (pattern.Length + 1); start++)
        {
            bool match = true;
            for (int i = 0; i < pattern.Length; i++)
            {
                float expected = pattern[i] * beatDuration;
                float actual = clickTimes[start + i + 1] - clickTimes[start + i];

                if (Mathf.Abs(actual - expected) > tolerance)
                {
                    match = false;
                    break;
                }
            }
            if (match) return true;
        }
        return false;
    }

    private void ShowDebugMessage(string message)
    {
        if (debugText != null)
            StartCoroutine(DisplayMessageCoroutine(message));
    }

    private IEnumerator DisplayMessageCoroutine(string message)
    {
        debugText.text = message;
        debugText.enabled = true;
        yield return new WaitForSeconds(debugDuration);
        debugText.enabled = false;
    }
}

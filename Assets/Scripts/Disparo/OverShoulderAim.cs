using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverShoulderAim : MonoBehaviour
{
    [Header("Referencias")]
    public Transform camara;
    public Transform hombroDerecho;
    public Transform hombroIzquierdo;
    public Transform puntoMira;

    [Header("Configuración")]
    public float velocidadRotacion = 10f;
    public float suavizadoCamara = 10f;
    public float distanciaMira = 8f;

    [Header("Zoom")]
    public float zoomNormal = 60f;
    public float zoomApuntando = 40f;
    public float velocidadZoom = 10f;

    private bool apuntando = false;
    private bool ladoDerecho = true;

    void Update()
    {
        DetectarEntrada();
        MoverCamara();
        AjustarZoom();
        ActualizarPuntoMira();
    }

    void DetectarEntrada()
    {
        // Mantener click derecho para apuntar
        apuntando = Input.GetMouseButton(1);

        // Cambiar de hombro
        if (Input.GetKeyDown(KeyCode.V))
            ladoDerecho = !ladoDerecho;
    }

    void MoverCamara()
    {
        if (!apuntando)
            return;

        Transform hombroActual = ladoDerecho ? hombroDerecho : hombroIzquierdo;

        Vector3 objetivo = hombroActual.position;
        camara.position = Vector3.Lerp(camara.position, objetivo, Time.deltaTime * suavizadoCamara);
        Debug.DrawLine(camara.position, objetivo, Color.red);
        Debug.Log("Posición cámara: " + camara.position + " | Objetivo: " + objetivo);

        RotacionLibre();
    }

    void RotacionLibre()
    {
        float mouseX = Input.GetAxis("Mouse X") * velocidadRotacion;
        transform.Rotate(Vector3.up * mouseX);
    }

    void AjustarZoom()
    {
        float zoomObjetivo = apuntando ? zoomApuntando : zoomNormal;
        Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, zoomObjetivo, Time.deltaTime * velocidadZoom);
    }

    void ActualizarPuntoMira()
    {
        Ray rayo = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        puntoMira.position = rayo.origin + rayo.direction * distanciaMira;
    }

    // ------------------------------
    // MÉTODOS ÚTILES PARA OTROS SCRIPTS
    // ------------------------------

    public bool EstaApuntando()
    {
        return apuntando;
    }

    public Vector3 ObtenerDireccionDisparo()
    {
        return (puntoMira.position - camara.position).normalized;
    }
}

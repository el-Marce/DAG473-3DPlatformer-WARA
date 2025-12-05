using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlataformaMovil : MonoBehaviour
{
   
    public float distanciaMovimiento = 5f; // Distancia total de izquierda a derecha
    public float velocidadMovimiento = 1f; // Velocidad del movimiento horizontal
    private Vector3 puntoInicial;

  
    public float amplitudFlotacion = 0.1f; // Altura máxima de la flotación
    public float frecuenciaFlotacion = 1f; // Velocidad del movimiento de flotación
    private float tiempoBase;

    [Tooltip("Tiempo máximo de retardo antes de que esta plataforma inicie su movimiento.")]
    public float retardoInicioMaximo = 3f; // Variable para controlar el retardo

    void Start()
    {
        // 1. Guardar la posición inicial para el movimiento horizontal
        puntoInicial = transform.position;

        // 2. Inicializar el tiempo base para el movimiento de flotación
        // Se añade un valor aleatorio para que las plataformas no floten en perfecta sincronía
        //tiempoBase = Time.time + Random.Range(0f, 10f);

        // Asignar un retardo inicial aleatorio (entre 0 y retardoInicioMaximo)
        // para que cada plataforma inicie su movimiento en un tiempo diferente.
        tiempoBase = Time.time + Random.Range(0f, retardoInicioMaximo);
    }

    void Update()
    {
        MoverHorizontalmente();
        AplicarFlotacion();
    }

    void MoverHorizontalmente()
    {
        // Usa la función Seno para crear un movimiento suave de ida y vuelta
        // El valor oscilará entre -1 y 1
        float offsetX = Mathf.Sin(Time.time * velocidadMovimiento) * (distanciaMovimiento / 2f);

        // La nueva posición X es la posición inicial más el offset
        Vector3 nuevaPosicion = puntoInicial;
        nuevaPosicion.x += offsetX;

        // Mover la plataforma a la nueva posición, manteniendo Y y Z sin cambios.
        transform.position = nuevaPosicion;
    }

    void AplicarFlotacion()
    {
        // Usa la función Seno para el movimiento vertical (flotación)
        // Se usa 'tiempoBase' para desfasar la flotación de cada plataforma
        float offsetY = Mathf.Sin((Time.time + tiempoBase) * frecuenciaFlotacion) * amplitudFlotacion;

        // Añadir el offset de flotación a la posición Y actual (que ya fue actualizada por MoverHorizontalmente)
        Vector3 posicionFlotante = transform.position;
        posicionFlotante.y += offsetY;

        transform.position = posicionFlotante;
    }

}

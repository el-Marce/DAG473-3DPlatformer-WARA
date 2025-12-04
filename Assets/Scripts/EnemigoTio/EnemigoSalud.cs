using UnityEngine;

public class EnemigoSalud : MonoBehaviour
{
    [Header("Salud")]
    public float vidaInicial = 100f;
    private float vidaActual;

    void Start()
    {
        vidaActual = vidaInicial;
    }

    public void RecibirDaño(float daño)
    {
        vidaActual -= daño;
        Debug.Log($"Enemigo recibió {daño} daño. Vida restante: {vidaActual}");

        if (vidaActual <= 0)
        {
            Morir();
        }
    }

    void Morir()
    {
        Debug.Log("Enemigo muerto");
        Destroy(gameObject);
    }
}

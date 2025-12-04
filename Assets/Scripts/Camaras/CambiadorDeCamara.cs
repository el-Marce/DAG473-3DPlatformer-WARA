using UnityEngine;
using Cinemachine;

public class CambiadorDeCamara : MonoBehaviour
{
    public CinemachineVirtualCamera camaraTPS;
    public CinemachineVirtualCamera camaraApuntado;

    public int prioridadTPS = 10;
    public int prioridadApuntado = 20;

    public GameObject crosshair;

    public bool aiming => Input.GetMouseButton(1);

    public StarterAssets.ThirdPersonController player;

    float velocidadOriginal;

    void Start()
    {
        if (player != null)
        {
            velocidadOriginal = player.MoveSpeed;
        }
    }

    void Update()
    {
        if (aiming)
        {
            camaraTPS.Priority = prioridadTPS;
            camaraApuntado.Priority = prioridadApuntado;

            if (crosshair != null) crosshair.SetActive(true);

            if (player != null)
                player.MoveSpeed = velocidadOriginal * 0.5f;
        }
        else
        {
            camaraTPS.Priority = prioridadApuntado;
            camaraApuntado.Priority = prioridadTPS;

            if (crosshair != null) crosshair.SetActive(false);

            if (player != null)
                player.MoveSpeed = velocidadOriginal;
        }
    }
}

using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public CambiadorDeCamara cambiadorDeCamara;

    public Transform shootSpawm;
    public GameObject bulletPrefab;

    private Camera cam;
    public Transform playerBody;  // ? Añadido: objeto que gira (el root del jugador)

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        Cursor.lockState = CursorLockMode.Locked;

        if (cambiadorDeCamara.aiming)
        {
            ForzarMirarAlObjetivo();  // ? Añadido

            if (Input.GetKeyDown(KeyCode.F) || Input.GetMouseButtonDown(0))
                Shoot();
        }
    }

    void ForzarMirarAlObjetivo()
    {
        // Ray al centro de la pantalla
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        Vector3 objetivo;

        if (Physics.Raycast(ray, out RaycastHit hit, 999f))
            objetivo = hit.point;
        else
            objetivo = ray.GetPoint(999f);

        // Dirección hacia el objetivo
        Vector3 dir = objetivo - playerBody.position;
        dir.y = 0;

        if (dir.sqrMagnitude < 0.01f)
            return;

        // Rotación objetivo
        Quaternion rotObjetivo = Quaternion.LookRotation(dir);

        // ?? Suavizado (evita vibración)
        playerBody.rotation = Quaternion.Slerp(
            playerBody.rotation,
            rotObjetivo,
            Time.deltaTime * 12f   // Velocidad de suavizado (ajustable)
        );
    }


    void Shoot()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));

        Vector3 targetPoint;

        if (Physics.Raycast(ray, out RaycastHit hit, 999f))
            targetPoint = hit.point;
        else
            targetPoint = ray.GetPoint(999f);

        Vector3 dir = (targetPoint - shootSpawm.position).normalized;

        shootSpawm.forward = dir;

        Instantiate(bulletPrefab, shootSpawm.position, shootSpawm.rotation);
    }
}

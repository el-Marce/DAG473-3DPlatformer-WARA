using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float power = 10f;
    public float lifeTime = 4f;
    public float daño = 10f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * power;
        Destroy(gameObject, lifeTime);
    }

    public void OnTriggerEnter(Collider collision)
    {
        // Verificar el tag del objeto golpeado
        if (collision.CompareTag("enemigo"))
        {
            // Obtener salud en el objeto padre del enemigo
            EnemigoSalud enemigo = collision.GetComponent<EnemigoSalud>();

            if (enemigo != null)
            {
                enemigo.RecibirDaño(daño);
            }
            Destroy(gameObject);
        }

    }
}



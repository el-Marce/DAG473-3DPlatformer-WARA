using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float power = 100f;
    public float lifeTime = 4f;

    private Rigidbody bulletRb;

    void Start()
    {
        bulletRb = GetComponent<Rigidbody>();
        bulletRb.velocity = transform.forward * power;

        // Destruye la bala automáticamente después de "lifeTime" segundos
        Destroy(gameObject, lifeTime);
    }
}
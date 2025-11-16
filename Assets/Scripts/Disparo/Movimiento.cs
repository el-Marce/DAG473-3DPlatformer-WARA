using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movimiento : MonoBehaviour
{
    public float power = 10f;
    public float lifeTime = 4f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        rb.velocity = Vector3.zero;
        rb.AddForce(transform.forward * power, ForceMode.VelocityChange);
        Destroy(gameObject, lifeTime);
    }
}

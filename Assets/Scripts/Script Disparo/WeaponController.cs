using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public Transform shootSpawm;
    public bool shooting;
    public GameObject bulletPrefab;



    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Cursor.lockState = CursorLockMode.Locked;

        shooting = Input.GetKeyDown(KeyCode.F);

        if(shooting)
        {
            InstantiateBullet();
        }
    }

    public void InstantiateBullet()
    {
        Instantiate(bulletPrefab, shootSpawm.position, shootSpawm.rotation);
    }
}

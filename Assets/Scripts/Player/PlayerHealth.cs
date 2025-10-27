using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] int maxHealth = 100;
    [SerializeField] int currentHealth;



    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damageAmount)
    {
        if(currentHealth <= 0)
        {
            currentHealth = 0;

            Die();
        }

    }

    void Die()
    {
        Debug.Log("Player Died");

       // Destroy(gameObject);
    }

    void RestarLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); 
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DeathZone"))
        {
            Debug.Log("Tocado por una Zona Muerta");

            Die();


        }
    }
    private void Update()
    {
        if (transform.position.y < -50)
        {
            Die();
        }
    }
}

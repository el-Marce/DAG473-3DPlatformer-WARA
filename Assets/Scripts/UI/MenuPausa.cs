using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPausa : MonoBehaviour
{
    public GameObject menuPausaUI;
    public MonoBehaviour scriptMovimiento;

    private bool estaPausado = false;

    void Update()
    {
        // Toggle pausa con ESC
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (estaPausado)
                ReanudarJuego();
            else
                PausarJuego();
        }

        // ----------- Atajos del menú pausado -----------
        if (estaPausado)
        {
            if (Input.GetKeyDown(KeyCode.N))
            {
                IrAlMenuPrincipal();
            }

            if (Input.GetKeyDown(KeyCode.M))
            {
                // Reiniciar nivel
                ReiniciarNivel();
            }
        }
    }
    public void ReanudarJuego()
    {
        menuPausaUI.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        scriptMovimiento.enabled = true;

        Time.timeScale = 1f;
        estaPausado = false;
    }

    public void PausarJuego()
    {
        menuPausaUI.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        scriptMovimiento.enabled = false;

        Time.timeScale = 0f;
        estaPausado = true;

    }
    public void IrAlMenuPrincipal()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
    public void ReiniciarNivel()
    {
        Time.timeScale = 1f;

        // Evita que siga visible durante el cambio de escena
        if (menuPausaUI != null)
            menuPausaUI.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}



using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;

public class MenuPausa : MonoBehaviour
{
    [Header("UI")]
    public GameObject menuPausaUI;      // Canvas del menú de pausa

    [Header("Cinemachine")]
    public CinemachineBrain cineBrain;  // Arrastra tu CinemachineBrain de la Main Camera

    private bool estaPausado = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (estaPausado)
                ReanudarJuego();
            else
                PausarJuego();
        }
    }

    public void PausarJuego()
    {
        // Mostrar menú
        menuPausaUI.SetActive(true);

        // Cursor visible y desbloqueado
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // Congelar tiempo del juego
        Time.timeScale = 0f;

        // Desactivar Cinemachine
        if (cineBrain != null)
            cineBrain.enabled = false;

        estaPausado = true;
    }

    public void ReanudarJuego()
    {
        // Ocultar menú
        menuPausaUI.SetActive(false);

        // Cursor oculto y bloqueado
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // Reactivar tiempo del juego
        Time.timeScale = 1f;

        // Reactivar Cinemachine
        if (cineBrain != null)
            cineBrain.enabled = true;

        estaPausado = false;
    }

    public void ReiniciarNivel()
    {
        // Asegurarse de que el tiempo esté normal antes de recargar
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void VolverMenuPrincipal()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu"); // Cambia el nombre por el de tu escena
    }
}


using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPausa : MonoBehaviour
{
    public GameObject menuPausaUI;
    public MonoBehaviour scriptMovimiento;

    private bool estaPausado = false;

    void Start()
    {
        // Asegurar que todo inicia normal al cargar la escena
        Time.timeScale = 1f;
        estaPausado = false;

        if (menuPausaUI != null)
            menuPausaUI.SetActive(false);
    }

    void Update()
    {
        // Referencias necesarias
        if (menuPausaUI == null || scriptMovimiento == null)
            return;

        // Toggle con ESC
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (estaPausado)
                ReanudarJuego();
            else
                PausarJuego();
        }
    }

    public void ReanudarJuego()
    {
        menuPausaUI.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;

        scriptMovimiento.enabled = true;

        Time.timeScale = 1f;
        estaPausado = false;
    }

    public void PausarJuego()
    {
        menuPausaUI.SetActive(true);
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

        // No destruir nada, simplemente ocultamos el menú
        if (menuPausaUI != null)
            menuPausaUI.SetActive(false);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}








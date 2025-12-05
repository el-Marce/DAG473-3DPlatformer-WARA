using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPausa : MonoBehaviour
{
    public GameObject menuPausaUI;
    public MonoBehaviour scriptMovimiento;

    private bool estaPausado = false;

    void Start()
    {
        if (menuPausaUI != null)
            menuPausaUI.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (estaPausado)
                ReanudarJuego();
            else
                PausarJuego();
        }

        // 🔹 N → Recargar la escena actual
        if (Input.GetKeyDown(KeyCode.N))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        // 🔹 M → Cargar MainMenu
        if (Input.GetKeyDown(KeyCode.M))
        {
            SceneManager.LoadScene("MainMenu");
        }
    }

    public void ReanudarJuego()
    {
        if (menuPausaUI != null)
            menuPausaUI.SetActive(false);

        scriptMovimiento.enabled = true;
        estaPausado = false;
    }

    public void PausarJuego()
    {
        if (menuPausaUI != null)
            menuPausaUI.SetActive(true);

        scriptMovimiento.enabled = false;
        estaPausado = true;
    }
}

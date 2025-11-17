using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPausa : MonoBehaviour
{
    public GameObject menuPausaUI;

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
        menuPausaUI.SetActive(true);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None; // 🔓 Para usar el mouse

        Time.timeScale = 0f;
        estaPausado = true;
    }

    public void ReanudarJuego()
    {
        menuPausaUI.SetActive(false);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked; // 🔒 Regresa al control 3D

        Time.timeScale = 1f;
        estaPausado = false;
    }

    public void ReiniciarNivel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}


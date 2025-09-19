using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public void NewGame()
    {
        SceneManager.LoadScene("Playground");
    }

    public void Options()
    {
        SceneManager.LoadScene("OptionsScene");
    }

    /*public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Salir del juego");
    }*/
}

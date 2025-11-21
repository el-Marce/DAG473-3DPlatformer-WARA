using UnityEngine;
using UnityEngine.SceneManagement;

public static class SaveSystemSlots
{
    public static void SaveGame(Transform player, int slot)
    {
        PlayerPrefs.SetFloat($"Slot{slot}_PlayerX", player.position.x);
        PlayerPrefs.SetFloat($"Slot{slot}_PlayerY", player.position.y);
        PlayerPrefs.SetFloat($"Slot{slot}_PlayerZ", player.position.z);

        PlayerPrefs.SetString($"Slot{slot}_Scene", SceneManager.GetActiveScene().name);
        PlayerPrefs.SetInt($"Slot{slot}_Used", 1);

        PlayerPrefs.Save();
        Debug.Log($"Guardado en slot {slot}");
    }

    public static bool SlotExists(int slot)
    {
        return PlayerPrefs.HasKey($"Slot{slot}_Used");
    }

    public static string GetSlotScene(int slot)
    {
        return PlayerPrefs.GetString($"Slot{slot}_Scene", "Ninguna");
    }

    public static void LoadGame(int slot)
    {
        if (!SlotExists(slot))
        {
            Debug.LogWarning($"El slot {slot} está vacío");
            return;
        }

        string sceneName = PlayerPrefs.GetString($"Slot{slot}_Scene");

        SceneManager.sceneLoaded += (Scene s, LoadSceneMode mode) =>
        {
            Vector3 pos = new Vector3(
                PlayerPrefs.GetFloat($"Slot{slot}_PlayerX"),
                PlayerPrefs.GetFloat($"Slot{slot}_PlayerY"),
                PlayerPrefs.GetFloat($"Slot{slot}_PlayerZ")
            );

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.transform.position = pos;

            Debug.Log($"Partida del slot {slot} cargada");
        };

        SceneManager.LoadScene(sceneName);
    }
}

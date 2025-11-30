using UnityEngine;
using UnityEngine.SceneManagement;

public static class SaveSystemSlots
{
    // Guarda posición + escena en slot (1..3)
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

    public static int GetFirstEmptySlot()
    {
        for (int s = 1; s <= 3; s++)
        {
            if (!SlotExists(s)) return s;
        }
        return -1; // ninguno vacío
    }

    // Cargar: guarda el slot a cargar y suscribe OnSceneLoaded
    public static void LoadGame(int slot)
    {
        if (!SlotExists(slot))
        {
            Debug.LogWarning($"El slot {slot} está vacío");
            return;
        }

        PlayerPrefs.SetInt("SlotToLoad", slot);

        // Asegurarse de no suscribir varias veces
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;

        string sceneName = PlayerPrefs.GetString($"Slot{slot}_Scene");
        Debug.Log("Cargando escena: " + sceneName);
        SceneManager.LoadScene(sceneName);
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        int slot = PlayerPrefs.GetInt("SlotToLoad", -1);
        if (slot == -1)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            return;
        }

        Vector3 pos = new Vector3(
            PlayerPrefs.GetFloat($"Slot{slot}_PlayerX"),
            PlayerPrefs.GetFloat($"Slot{slot}_PlayerY"),
            PlayerPrefs.GetFloat($"Slot{slot}_PlayerZ")
        );

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.transform.position = pos;
            Debug.Log($"Partida del slot {slot} cargada correctamente");
        }
        else
        {
            Debug.LogWarning("No se encontró GameObject con tag 'Player' al cargar partida.");
        }

        PlayerPrefs.DeleteKey("SlotToLoad"); // opcional: limpiar
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    public static void DeleteLastUsedSlot()
    {
        int lastUsed = -1;

        // Recorre del 3 al 1 para encontrar el último slot guardado
        for (int slot = 3; slot >= 1; slot--)
        {
            if (SlotExists(slot))
            {
                lastUsed = slot;
                break;
            }
        }

        if (lastUsed == -1)
        {
            Debug.Log("No hay partidas para borrar.");
            return;
        }

        // Eliminar claves del slot
        PlayerPrefs.DeleteKey($"Slot{lastUsed}_PlayerX");
        PlayerPrefs.DeleteKey($"Slot{lastUsed}_PlayerY");
        PlayerPrefs.DeleteKey($"Slot{lastUsed}_PlayerZ");
        PlayerPrefs.DeleteKey($"Slot{lastUsed}_Scene");
        PlayerPrefs.DeleteKey($"Slot{lastUsed}_Used");

        PlayerPrefs.Save();

        Debug.Log($"Partida borrada del slot {lastUsed}");
    }

}
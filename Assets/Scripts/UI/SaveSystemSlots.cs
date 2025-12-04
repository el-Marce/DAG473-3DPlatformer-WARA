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

        PlayerPrefs.SetInt($"Slot{slot}_Monedas", GameManager.instance.monedasTotales);
        PlayerPrefs.SetInt($"Slot{slot}_OvejasEnZonaSegura", GameManager.instance.ovejasEnZonaSegura);

        SistemaDeSalud salud = player.GetComponent<SistemaDeSalud>();
        PlayerPrefs.SetInt($"Slot{slot}_Salud", salud != null ? salud.saludActual : 100);

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
        return -1;
    }

    public static void DeleteLastUsedSlot()
    {
        int lastUsed = -1;
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

        PlayerPrefs.DeleteKey($"Slot{lastUsed}_PlayerX");
        PlayerPrefs.DeleteKey($"Slot{lastUsed}_PlayerY");
        PlayerPrefs.DeleteKey($"Slot{lastUsed}_PlayerZ");
        PlayerPrefs.DeleteKey($"Slot{lastUsed}_Scene");
        PlayerPrefs.DeleteKey($"Slot{lastUsed}_Monedas");
        PlayerPrefs.DeleteKey($"Slot{lastUsed}_OvejasEnZonaSegura");
        PlayerPrefs.DeleteKey($"Slot{lastUsed}_Salud");
        PlayerPrefs.DeleteKey($"Slot{lastUsed}_Used");

        PlayerPrefs.Save();
        Debug.Log($"Partida borrada del slot {lastUsed}");
    }

    public static void LoadGame(int slot)
    {
        if (!SlotExists(slot))
        {
            Debug.LogWarning($"El slot {slot} está vacío");
            return;
        }

        PlayerPrefs.SetInt("SlotToLoad", slot);

        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;

        string sceneName = PlayerPrefs.GetString($"Slot{slot}_Scene");
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

            SistemaDeSalud salud = player.GetComponent<SistemaDeSalud>();
            if (salud != null)
                salud.saludActual = PlayerPrefs.GetInt($"Slot{slot}_Salud", salud.saludMaxima);

            Debug.Log($"Partida del slot {slot} cargada correctamente");
        }
        else
        {
            Debug.LogWarning("No se encontró GameObject con tag 'Player' al cargar partida.");
        }

        if (GameManager.instance != null)
        {
            GameManager.instance.monedasTotales = PlayerPrefs.GetInt($"Slot{slot}_Monedas", 0);
            GameManager.instance.ovejasEnZonaSegura = PlayerPrefs.GetInt($"Slot{slot}_OvejasEnZonaSegura", 0);
        }

        PlayerPrefs.DeleteKey("SlotToLoad");
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}


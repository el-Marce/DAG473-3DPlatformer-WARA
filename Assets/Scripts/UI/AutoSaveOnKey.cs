using UnityEngine;

public class AutoSaveOnKey : MonoBehaviour
{
    public Transform player; // arrastra tu Player aquí en el Inspector
    public KeyCode saveKey = KeyCode.V;

    void Update()
    {
        if (Input.GetKeyDown(saveKey))
        {
            SaveToFirstEmptySlot();
        }
    }

    void SaveToFirstEmptySlot()
    {
        int slot = SaveSystemSlots.GetFirstEmptySlot();

        if (slot == -1)
        {
            Debug.Log("No hay slots vacíos para guardar.");
            return;
        }

        SaveSystemSlots.SaveGame(player, slot);
        Debug.Log("Juego guardado en el slot: " + slot);
    }
}


using UnityEngine;

public class AutoSaveOnKey : MonoBehaviour
{
    public Transform player; // arrastra tu Player aquí
    public KeyCode saveKey = KeyCode.V; // tecla para guardar

    void Update()
    {
        if (Input.GetKeyDown(saveKey))
        {
            SaveToFirstEmptySlot();
        }
    }

    void SaveToFirstEmptySlot()
    {
        // buscar primer slot vacío
        for (int slot = 1; slot <= 3; slot++)
        {
            if (!SaveSystemSlots.SlotExists(slot))
            {
                SaveSystemSlots.SaveGame(player, slot);
                Debug.Log("Juego guardado en slot: " + slot);
                return;
            }
        }

        Debug.Log("No hay slots vacíos para guardar.");
    }
}


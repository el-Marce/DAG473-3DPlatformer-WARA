using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SaveSlotsMenu : MonoBehaviour
{
    public Button[] slotButtons;   // Size = 3
    public TMP_Text[] slotTexts;   // Size = 3 (TextMeshPro)

    void Start()
    {
        RefreshSlots();
    }

    public void RefreshSlots()
    {
        for (int i = 0; i < 3; i++)
        {
            int slot = i + 1;
            if (SaveSystemSlots.SlotExists(slot))
            {
                string scene = SaveSystemSlots.GetSlotScene(slot);
                slotTexts[i].text = $"Slot {slot} - Escena: {scene}";
            }
            else
            {
                slotTexts[i].text = $"Slot {slot}: Vacío";
            }
        }
    }

    // Llamar desde el OnClick de cada botón (con parámetro)
    public void LoadSlot(int slot)
    {
        SaveSystemSlots.LoadGame(slot);
    }
}


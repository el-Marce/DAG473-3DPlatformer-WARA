using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SaveSlotsMenu : MonoBehaviour
{
    public Button[] slotButtons;
    public TMP_Text[] slotTexts;

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

    public void LoadSlot(int slot)
    {
        SaveSystemSlots.LoadGame(slot);
    }
}


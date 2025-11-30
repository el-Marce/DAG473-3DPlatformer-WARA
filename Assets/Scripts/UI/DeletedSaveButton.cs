using UnityEngine;

public class DeletedSaveButton : MonoBehaviour
{
    public SaveSlotsMenu slotsMenu; // para refrescar la UI

    public void DeleteLastSave()
    {
        SaveSystemSlots.DeleteLastUsedSlot();

        if (slotsMenu != null)
            slotsMenu.RefreshSlots();
    }
}


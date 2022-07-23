using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Subclass of slot. Same behavior except for some 
/// additional logic for communicating with the currently selected tower
/// </summary>

public class TowerSlot : Slot
{
    /// <summary>
    /// - Method is called when an object is dragged onto it.
    /// - EventData holds information about the incoming object that was dropped.
    /// </summary>
    /// <param name="eventData"></param>
    public override void OnDrop(PointerEventData eventData)
    {
        var incomingChipInstance = eventData.pointerDrag.GetComponent<UI_ChipInstance>();

        // -- If the chip was placed onto itself, locked, or is trying to interact with a chip from a locked tower -- \\
        if (DoesChipMatch(incomingChipInstance) || Locked || incomingChipInstance.SlotRef.Locked)
        {
            return;
        }

        // -- If the chip was placed onto another chip -- \\
        if (IsSlotFull)
        {
            this.Merge(incomingChipInstance, out bool mergeSuccess);
            if (mergeSuccess)
            {
                EventManager.Instance.RaiseGameEvent(EventConstants.MERGE);
            }
        }
        else
        {
            base.UpdateChipData(incomingChipInstance);
            SlottedChip.gameObject.SetActive(true);

            SelectedTower.Instance.AddChip(SlottedChip.ThisChip, SlotID);
            // -- If the dropped chip is from a tower slot, we must remove it from the source weapon slot -- \\
            if (incomingChipInstance.GetComponentInParent<Slot>().CompareTag("TowerSlot"))
            {
                SelectedTower.Instance.RemoveChip(incomingChipInstance.ThisChip, incomingChipInstance.SlotID);
            }
            base.ResetChipData(incomingChipInstance);
        }
    }

    /// <summary>
    /// Overrides Item slots handling of the merging of chips.
    /// Main difference is that it doesn't need to check if the incoming 
    /// chip came from the inventory. See class summary for more info on merging
    /// </summary>
    /// <param name="incomingChipInstance">The chip instance that is being dropped</param>
    /// <returns>Whether or not the merge was a success or not</returns>
    public override void Merge(UI_ChipInstance incomingChipInstance, out bool success)
    {
        if (InventoryData.CanMerge(this.SlottedChip.ThisChip, incomingChipInstance.ThisChip))
        {
            SelectedTower.Instance.RemoveChip(incomingChipInstance.ThisChip, incomingChipInstance.SlotID);
            base.ResetChipData(incomingChipInstance);
            this.SlottedChip.ThisChip.UpgradeTier();
            base.UpdateChipData(this.SlottedChip);
            success = true;

            // Passing null ONLY updates the tower stats.
            // We do this because we aren't adding anything, just increasing the tier
            SelectedTower.Instance.AddChip(null, SlotID);
        }
        else
        {
            success = false;
            base.SwapChips(incomingChipInstance);
        }
    }
}

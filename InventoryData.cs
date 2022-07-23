using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Tracks variables for inventory size and state
/// </summary>

[CreateAssetMenu(menuName = "ScriptableObjects/InventoryData")]
public class InventoryData : SingletonScriptableObject<InventoryData>
{
    [SerializeField]
    [Tooltip("The set of item slots in the inventory")]
    public List<Slot> Slots = new List<Slot>();
    [SerializeField]
    [Tooltip("The maximum number of item slots")]
    public int MaxItemSlots;

    public float RewardChipInterval = 10;
    public List<ChipInstance> OverflowQueue;

    /// <summary>
    /// Wipe the ItemSlots list
    /// </summary>
    void OnEnable() => Slots.Clear();
    
    /// <summary>
    /// Resets this scriptable objects data back to defualt (you choose what "default" is)
    /// </summary>
    public void ResetData() => OverflowQueue.Clear();

    /// <summary>
    /// Get all ItemSlot scripts from the children of the provided GameObject
    /// </summary>
    /// <param name="obj">The object to search the children of</param>
    public void GetAllSlots(GameObject obj)
    {
        Slots.Clear();
        foreach (Slot slotScript in obj.GetComponentsInChildren<Slot>())
        {
            if (!Slots.Contains(slotScript))
            {
                Slots.Add(slotScript);
            }
        }
    }

    /// <summary>
    /// Adds a Chip to the first available item slot, if possible
    /// </summary>
    /// <param name="chip">The new Chip data</param>
    /// <returns>Whether the Chip could be added</returns>
    public bool AddNewChip(ChipInstance chip, BoxState boxState = BoxState.Box)
    {
        Slot slot = FirstEmptySlot();
        // if inventory full, add to queue
        if (slot == null && OverflowQueue.Count < 15f)
        {
            OverflowQueue.Add(chip);
            EventManager.Instance.RaiseGameEvent(EventConstants.ADD_TO_OVERFLOW);
            return false;
        }
        // otherwise, add new Chip (and store it in a box if necessary)
        else
        {
            
            if (PlayerStats.Instance.TutorialComplete)
            {
                slot.UpdateChipInstance(chip, boxState);
            }
            else
            {
                slot.UpdateChipInstance(chip, BoxState.None);
            }
            GameObject slotChip = slot.SlottedChip.gameObject;
            slotChip.SetActive(true);
            slotChip.GetComponent<UI_ChipInstance>().FadeIn();
            return true;
        }
    }

    /// <summary>
    /// Return whether the inventory is full
    /// </summary>
    /// <returns>Whether it is full</returns>
    public bool IsFull()
    {
        foreach (Slot slot in Slots)
        {
            if (slot != null && !slot.IsSlotFull)
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Upgrades all inventory chips to the chip tier rewarded for economy
    /// </summary>
    public void UpgradeAllInventoryChips(GameObject upgradeEffect)
    {
        foreach (Slot slot in Slots)
        {
            int tier = slot.SlottedChip.ThisChip.Tier;
            if (tier < PlayerStats.Instance.ChipEconomy && tier > 0 && (slot.BoxStatus == BoxState.None || slot.BoxStatus == BoxState.Opening))
            {
                slot.SlottedChip.ThisChip.Tier = PlayerStats.Instance.ChipEconomy;
                slot.SlottedChip.UpdateVisualData();
                // only display upgrade effect on visible chips
                if (slot.SlottedChip.gameObject.activeSelf)
                {
                    GameObject effect = Instantiate(upgradeEffect, slot.transform);
                    effect.transform.localScale = Vector3.one*100f;
                    effect.GetComponent<ParticleDecay>().SetLayer("UI");
                }
            }
        }
    }

    /// <summary>
    /// Call this when the economy upgrades.
    /// Increases every chip in the overflow queue to the new chip tier
    /// to be rolled (ChipTier variable in playerstats)
    /// </summary>
    public void UpgradeAllOverflowChips()
    {
        foreach (ChipInstance chip in OverflowQueue)
        {
            if (chip.Tier < PlayerStats.Instance.ChipEconomy)
            {
                chip.Tier = PlayerStats.Instance.ChipEconomy;
            }
        }
    }

    /// <summary>
    /// Get a reference to the first item slot
    /// </summary>
    /// <returns>The item slot found, or null if none are available</returns>
    public Slot FirstEmptySlot()
    {
        foreach (Slot slot in Slots)
        {
            if (slot != null && !slot.IsSlotFull)
            {
                return slot;
            }
        }
        return null;
    }

    /// <summary>
    /// Checks if two chip instances are of the same tier and type
    /// </summary>
    /// <returns>True if can merge or false if cannot</returns>
    public static bool CanMerge(ChipInstance instance, ChipInstance instance2)
    {
        return instance.Tier == instance2.Tier
            && instance.ChipBaseRef == instance2.ChipBaseRef;
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

/// <summary>
/// Sends all inventory slots to the InventoryData SO and fills a list
/// </summary>

public class InventoryUIHandler : MonoBehaviour
{
    
    public string note = "Sends all inventory slots to the InventoryData SO & handles overflow";
    
    private const string REWARD_BOX_AD = "AcceptedAdBox";

    // Rewards a chip every once and a while 
    private float rewardChipTimer = 0;
    private float rewardChipInterval;


    void Awake()
    {
        // Make the system aware of the inventory slots
        InventoryData.Instance.GetAllSlots(gameObject);
        rewardChipInterval = InventoryData.Instance.RewardChipInterval;
    }

    private void Update()
    {
        // Adds a chip to the inventory every (RewardChipInterval) seconds 
        if (PlayerStats.Instance.TutorialComplete)
        {
            rewardChipTimer += Time.deltaTime;
            if (rewardChipTimer > rewardChipInterval)
            {
                // Get Type
                var randChip = Random.Range(0, PlayerStats.Instance.NumChipTypesAvailable);
                ChipBase chipType = PlayerStats.Instance.AvailableChipTypes[randChip];
                // Get tier
                int tier = PlayerStats.Instance.ChipEconomy;
                // Create and add new chip
                InventoryData.Instance.AddNewChip(new ChipInstance { ChipBaseRef = chipType, Tier = tier });
                rewardChipTimer = 0;
            }
        }

        // ---- AD BOXES MOVED TO AdHandler.cs! ----- \\
    }

    /// <summary>
    /// Attempt to add a chip from the overflow queue
    /// </summary>
    public void TryAddFromOverflow()
    {
        // Base case
        if (InventoryData.Instance.OverflowQueue.Count == 0 || InventoryData.Instance.IsFull()) { return; }

        // Add chip to inventory after a slight delay
        StartCoroutine(RemoveFromOverflow());
    }

    /// <summary>
    /// Attempt to remove a chip from the overflow queue
    /// </summary>
    private IEnumerator RemoveFromOverflow()
    {
        yield return new WaitForSeconds(1f);
        if (!InventoryData.Instance.IsFull() && InventoryData.Instance.OverflowQueue.Count != 0)
        {
            InventoryData.Instance.AddNewChip(InventoryData.Instance.OverflowQueue[0]);
            InventoryData.Instance.OverflowQueue.RemoveAt(0);
            EventManager.Instance.RaiseGameEvent(EventConstants.REMOVE_FROM_OVERFLOW);
        }
        yield return 0;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the tutorial's behavior
/// </summary>

public class TutorialManagerBehavior : MonoBehaviour
{
    [Tooltip("Conversation to be launched at the start of a new game")]
    public ConversationData IntroConversation;

    [Tooltip("Tutorial arrow reference")]
    public GameObject ArrowRef;
    private TutorialArrowBehavior tutorialArrow;
    
    [Tooltip("Tracks the number of merges completed")]
    private int mergeCount = 0;
    [Tooltip("Tracks the number of chips purchased")]
    private int purchaseCount = 0;
    [Tooltip("Whether the intro conversation is completed")]
    private bool dialogueDone;
    
    [Tooltip("The set of Item Slots to monitor (at least 4)")]
    public GameObject[] ItemSlots;
    private ItemSlot[] itemSlots;

    [Tooltip("The set of Tower Slots to monitor (at least 2)")]
    public GameObject[] TowerSlots;
    private TowerSlot[] towerSlots;

    [Tooltip("Buy Button reference")]
    public GameObject BuyButtonRef;


    /// <summary>
    /// Populate references
    /// </summary>
    void Awake()
    {
        tutorialArrow = ArrowRef.GetComponent<TutorialArrowBehavior>();
        
        itemSlots = new ItemSlot[ItemSlots.Length];
        for(int i = 0; i < ItemSlots.Length; i++)
        {
            itemSlots[i] = ItemSlots[i].GetComponent<ItemSlot>();
        }
        towerSlots = new TowerSlot[TowerSlots.Length];
        for(int i = 0; i < TowerSlots.Length; i++)
        {
            towerSlots[i] = TowerSlots[i].GetComponent<TowerSlot>();
        }
    }

    void Start()
    {
        if (!PlayerStats.Instance.TutorialComplete)
        {
            StartCoroutine(TutorialSequence());       
        }
    }

    /// <summary>
    /// This coroutine handles the initial tutorial sequence
    /// </summary>
    private IEnumerator TutorialSequence()
    {
        Vector3 yOffset = 0.6f*Vector3.up;

        // Wait until dialogue is finished
        // TODO yield return new WaitUntil(() => dialogueDone);

        Debug.Log("Tutorial Step 1");
        // Step 1: Spawn 4 chips
        // Completion Criteria:
        // - 4 chips spawned
        Vector3 buttonPos = BuyButtonRef.transform.position;
        PlayerStats.Instance.NumChipTypesAvailable = 1; // lock chips to only producing first type (plasma)
        LockAllItemSlots(true); // disable use of item slots
        LockAllTowers(true); // disable use of towers
        LockDiscard(true); // disable use of discard bin
        for (int i = 0; i < 4; i++)
        {
            yield return new WaitForSeconds(0.2f);
            InventoryData.Instance.AddNewChip(PlayerStats.Instance.GetRandomChip());           
        }


        Debug.Log("Tutorial Step 2");
        // Step 2: Merge chips to make first tier 2
        // Completion Criteria:
        // - itemSlot 0 or itemSlot 1 is empty
        // - At least 1 merge has been completed
        LockAllItemsExcept(new List<int>() {0,1});
        ArrowRef.SetActive(true);
        tutorialArrow.StartAnimation(ItemSlots[0].transform.position+yOffset, ItemSlots[1].transform.position+yOffset, 0.8f);
        yield return new WaitUntil(() => mergeCount >= 1 && !(itemSlots[0].IsSlotFull && itemSlots[1].IsSlotFull));

        Debug.Log("Tutorial Step 3");
        // Step 3: Merge chips to make second tier 2
        // Completion Criteria:
        // - itemSlot 2 or itemSlot 3 is empty
        // - At least 2 merges have been completed
        LockAllItemsExcept(new List<int>() {2,3});
        tutorialArrow.StartAnimation(ItemSlots[3].transform.position+yOffset, ItemSlots[2].transform.position+yOffset, 1.1f);
        yield return new WaitUntil(() => mergeCount >= 2 && !(itemSlots[2].IsSlotFull && itemSlots[3].IsSlotFull));
        
        Debug.Log("Tutorial Step 4");
        // Step 4: Merge chips to make tier 3
        // Completion Criteria:
        // - itemSlot 1 or itemSlot 2 is empty
        // - At least 3 merges have been completed
        LockAllItemsExcept(new List<int>() {0,1,2,3});
        tutorialArrow.StartAnimation(ItemSlots[2].transform.position+yOffset, ItemSlots[1].transform.position+yOffset, 0.8f);
        yield return new WaitUntil(() => mergeCount >= 3);

        Debug.Log("Tutorial Step 5");
        // Step 5: Equip chip
        // Completion Criteria:
        // - Chip equipped to a designated tower slot
        LockAllTowers(false);
        LockAllItemSlots(false);
        tutorialArrow.transform.Rotate(new Vector3(0,0,180));
        Vector3 newOffset = Vector3.up*1.5f;
        Vector3 centerPos = ItemSlots[1].transform.position + newOffset;
        tutorialArrow.StartAnimation(centerPos-newOffset, centerPos, 0.8f);
        //tutorialArrow.StartAnimation(buttonPos+180*Vector3.up, buttonPos+240*Vector3.up, 0.8f);
        yield return new WaitUntil(() => towerSlots[0].IsSlotFull || towerSlots[1].IsSlotFull);

        Debug.Log("Tutorial Step 6");
        // Step 6: Begin game
        LockDiscard(false);
        ArrowRef.SetActive(false);
        //PlayerStats.Instance.NumChipTypesAvailable = PlayerStats.Instance.AvailableChipTypes.Length; // allow any chip type to be produced
        PlayerStats.Instance.TutorialComplete = true;
        EventManager.Instance.RaiseGameEvent(EventConstants.ON_TUTORIAL_COMPLETE);
        yield return null;
    }

    /// <summary>
    /// Lock all chip slots besides the specified one
    /// </summary>
    /// <param name="unlocked">The indexes of the slots that remains unlocked</param>
    private void LockAllItemsExcept(List<int> unlocked)
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            itemSlots[i].Locked = !unlocked.Contains(i);
        }
    }

    /// <summary>
    /// Lock or Unlock all item slots
    /// </summary>
    /// <param name="locked">Whether to lock the slots</param>
    private void LockAllItemSlots(bool locked)
    {
         if (locked)
        {
            EventManager.Instance.RaiseGameEvent(EventConstants.LOCK_ITEMS);
        }
        else
        {
            EventManager.Instance.RaiseGameEvent(EventConstants.UNLOCK_ITEMS);
        }
    }

    /// <summary>
    /// Lock or Unlock all towers
    /// </summary>
    /// <param name="locked">Whether to lock the slots</param>
    private void LockAllTowers(bool locked)
    {
        if (locked)
        {
            EventManager.Instance.RaiseGameEvent(EventConstants.LOCK_TOWERS);
        }
        else
        {
           EventManager.Instance.RaiseGameEvent(EventConstants.UNLOCK_TOWERS);
        }
    }

    /// <summary>
    /// Lock or Unlock the chip discard bin
    /// </summary>
    /// <param name="locked">Whether to lock the discard bin</param>
    private void LockDiscard(bool locked)
    {
        if (locked)
        {
            EventManager.Instance.RaiseGameEvent(EventConstants.LOCK_DISCARD);
        }
        else
        {
           EventManager.Instance.RaiseGameEvent(EventConstants.UNLOCK_DISCARD);
        }
    }

    /// <summary>
    /// Records that a merge has occurred
    /// </summary>
    public void MergeReceived()
    {
        if (mergeCount < 3)
        {
            mergeCount++;
        }
    }

    /// <summary>
    /// Records that a chip purchase has occurred
    /// </summary>
    public void PurchaseReceived()
    {
        if (purchaseCount < 4)
        {
            purchaseCount++;
        }
    }
}

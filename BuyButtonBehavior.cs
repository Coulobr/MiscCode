using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Handles behavior for the Buy Chip button
/// </summary>

public class BuyButtonBehavior : MonoBehaviour, IPointerDownHandler
{
    [Header("References")]
    [Tooltip("The reference to the cost text component attached to a child of this button")]
    public TextMeshProUGUI CostText;
    [Tooltip("The reference to the tier value text component attached to a child of this button")]
    public TextMeshProUGUI TierText;
    private Image image;

    [Tooltip("Tracks whether the button is locked (refusing input)")]
    private bool locked;

    [Header("Appearance Settings")]
    [Tooltip("The color the button uses when a purchase can be made")]
    public Color ActiveColor;
    [Tooltip("The color the button uses when no purchase can be made")]
    public Color LockedColor;

    /// <summary>
    /// Initialize the cost and tier
    /// </summary>
    void Start()
    {
        image = GetComponent<Image>();
        UpdateCostTier();
        UpdateUsable();
    }

    /// <summary>
    /// Updates the cost and tier displayed on the button
    /// </summary>
    public void UpdateCostTier()
    {
        CostText.text = "-$" + PlayerStats.Instance.ChipCost.ToString();
        TierText.text = PlayerStats.Instance.ChipEconomy.ToString();
    }

    /// <summary>
    /// Queues update for the button appearance
    /// </summary>
    public void UpdateUsable()
    {
        StartCoroutine(ApplyUsableUpdate());
    }

    /// <summary>
    /// Updates the button appearance based on whether it can be used at the moment
    /// </summary>
    private IEnumerator ApplyUsableUpdate()
    {
        yield return new WaitForSeconds(0.1f);
        int cost = PlayerStats.Instance.ChipCost;
        if (PlayerStats.Instance.CanPurchase(cost) && !InventoryData.Instance.IsFull())
        {
            locked = false;
            image.color = ActiveColor;
            transform.localScale = new Vector3(1,1,1);
        }
        else
        {
            locked = true;
            image.color = LockedColor;
            transform.localScale = new Vector3(0.8f,0.8f,0.8f);
        }
        yield return null; 
    }

    /// <summary>
    /// Triggers on mouse down
    /// Purchases a chip if able
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerDown(PointerEventData eventData)
    {
        PurchaseChip();
    }

    /// <summary>
    /// Attempt to purchase a chip
    /// </summary>
    /// <returns>Whether the purchase could be completed</returns>
    private bool PurchaseChip()
    {
        int cost = PlayerStats.Instance.ChipCost;
        if (!locked && PlayerStats.Instance.CanPurchase(cost))
        {
            // Generate new chip
            ChipInstance newChip = PlayerStats.Instance.GetRandomChip();

            // attempt to create and purchase the new chip
            // fails if no chip generatable, no space available, or insufficient money
            if ((newChip != null) && InventoryData.Instance.AddNewChip(newChip) && PlayerStats.Instance.Purchase(cost))
            {
                EventManager.Instance.RaiseGameEvent(EventConstants.PURCHASE);
                return true;
            }
        }
        // otherwise, return false
        return false;
    }
}

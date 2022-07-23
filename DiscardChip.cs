using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// Drag and drop a chip onto here to discard it
/// </summary>

public class DiscardChip : MonoBehaviour, IDropHandler
{
    [Tooltip("The minumum alpha the chip discard bin icon")]
    public float MinAlpha = 0.05f;
    [Tooltip("Whether the slot is locked (will not accept chips)")]
    public bool Locked = false;
    private Image icon;

    void Awake()
    {
        icon = GetComponentInChildren<Image>();
        icon.DOFade(MinAlpha,0f);
    }

    #region Drag/Drop & Merging Methods
    /// <summary>
    /// Destory the chip dropped on it
    /// - Method is called when an object is dragged onto it.
    /// - EventData holds information about the incoming object that was dropped.
    /// </summary>
    /// <param name="eventData"></param>
    public virtual void OnDrop(PointerEventData eventData)
    {
        var incomingChipInstance = eventData.pointerDrag.GetComponent<UI_ChipInstance>();

        // -- If the chip was placed onto itself -- \\
        if (Locked || incomingChipInstance.SlotRef.Locked)
        {
            return;
        }
        else
        {
            // -- If the dropped chip is from a tower slot, we must remove it from that tower -- \\
            if (incomingChipInstance.transform.parent.CompareTag("TowerSlot"))
            {
                SelectedTower.Instance.RemoveChip(incomingChipInstance.ThisChip, incomingChipInstance.SlotID);
            }
            incomingChipInstance.gameObject.SetActive(false);
            incomingChipInstance.GetComponent<RectTransform>().localPosition = Vector2.zero;
        }
    }

    #endregion

    /// <summary>
    /// Locks the item slot
    /// </summary>
    public void LockTrash()
    {
        Locked = true;
    }

    /// <summary>
    /// Unlocks the item slot
    /// </summary>
    public void UnlockTrash()
    {
        Locked = false;
    }

    /// <summary>
    /// Fades out when not relevant
    /// </summary>
    public void FadeOut()
    {
        float fadeTime = 0.1f;
        icon.DOFade(MinAlpha, fadeTime);
        icon.rectTransform.DOScale(1f, fadeTime);
    }

    /// <summary>
    /// Fades in when draggable
    /// </summary>
    public void FadeIn()
    {
        if (!Locked)
        {
            float fadeTime = 0.2f;
            icon.DOFade(0.8f, fadeTime);
            icon.rectTransform.DOScale(1.2f, fadeTime);
        }
    }
}

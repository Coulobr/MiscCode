using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// This gameobject represents an invisible image
/// that locks it's location on its corresponding tower
/// at the start of the game.
/// 
/// This class also implements OnDrop(), which will handle
/// adding the dragged chip onto the tower. 
/// 
/// This is an alternative solution to raycasting to detect if 
/// the chip was dropped on the tower. We instead are using 
/// a UI object with the provided interface to implement the logic.
/// </summary>


public class TowerUIBody : MonoBehaviour, IDropHandler
{
    public GameObject TargetTower;
    public Transform CanvasContainer;
    private Canvas canvas;
    [Tooltip("Whether the tower is locked (will not accept chips)")]
    public bool Locked = false;

    #region Init & Update
    private void Start()
    {
        Vector3 scale = transform.localScale;
        SetNewParent(CanvasContainer);
        transform.localScale = scale;
        canvas = GetComponentInParent<Canvas>();
        if(TargetTower.GetComponent<TowerBehavior>().purchasable)
        {
            LockTowerBody();
        }
    }
    private void LateUpdate()
    {
        transform.position = WorldToUISpace(canvas, TargetTower, Vector2.zero);
    }
    #endregion

    /// <summary>
    /// See class summary 
    /// </summary>
    /// <param name="eventData"> Data of the dragged object </param>
    public void OnDrop(PointerEventData eventData)
    {
        // early exit if locked
        if (Locked)
        {
            return;
        }

        var incomingChipInstance = eventData.pointerDrag.GetComponent<UI_ChipInstance>();
        // -- Reset the incoming chip instance -- \\
        incomingChipInstance.GetComponent<RectTransform>().localPosition = Vector2.zero;

        // -- Only remove a chip from a tower if its selected -- \\
        if (incomingChipInstance.SlotRef.CompareTag("TowerSlot") && SelectedTower.Instance.CurrentTower != null)
        {
            SelectedTower.Instance.RemoveChip(incomingChipInstance.ThisChip, incomingChipInstance.SlotID);
        }
        SelectedTower.Instance.PreviousTower = SelectedTower.Instance.CurrentTower;
        SelectedTower.Instance.CurrentTower = TargetTower;

        // -- Consume or reactivate the incoming chip depending on if it could be placed in the tower -- \\
        if (SelectedTower.Instance.AddChip(incomingChipInstance.ThisChip, -1)) // -1 means it assigns to whatever slot is available
        {
            incomingChipInstance.GetComponent<RectTransform>().localPosition = Vector2.zero;
            incomingChipInstance.gameObject.SetActive(false);
        }
        else
        {
            // return the chip to where it came from
            bool chipExists = incomingChipInstance.ThisChip != null;
            if (!chipExists) {Debug.Log("Incoming chip null!");}
            bool prevTowerExists = SelectedTower.Instance.PreviousTower != null;
            if (!prevTowerExists) {Debug.Log("Previous tower null");}
            // make sure chip exists, previous tower exists, and the chip is coming from a tower slot
            if (chipExists && prevTowerExists && incomingChipInstance.GetComponentInParent<Slot>().CompareTag("TowerSlot"))
            {
                SelectedTower.Instance.AddChipSpecific(incomingChipInstance.ThisChip, incomingChipInstance.SlotID, SelectedTower.Instance.PreviousTower);
            }
        }

        // -- Invoke OnTowerSelect Event -- \\
        SelectedTower.Instance.InvokeTowerSelect(TargetTower);
    }

    #region Util Methods
    /// <summary>
    /// </summary>
    /// <param name="parentCanvas"></param>
    /// <param name="worldPos"></param>
    /// <param name="offset"></param>
    /// <returns></returns>
    public Vector3 WorldToUISpace(Canvas parentCanvas, GameObject worldPos, Vector2 offset)
    {
        _ = parentCanvas ?? throw new NullReferenceException("Assign the TowerBody Canvas container to this object");

        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos.transform.position);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentCanvas.transform as RectTransform, screenPos, parentCanvas.worldCamera, out Vector2 movePos);
        return parentCanvas.transform.TransformPoint(movePos + offset);
    }

    /// <summary>
    /// Sets the tower body object to the UI container
    /// and notifies if this field is null
    /// </summary>
    /// <param name="parent"></param>
    public void SetNewParent(Transform parent)
    {
        _ = parent ?? throw new NullReferenceException("Assign the TowerBody Canvas container to this object");
        transform.SetParent(parent);
    }
    #endregion

    /// <summary>
    /// Locks the tower body
    /// </summary>
    public void LockTowerBody()
    {
        Locked = true;
    }

    /// <summary>
    /// Unlocks the tower body
    /// </summary>
    public void UnlockTowerBody()
    {
        Locked = false;
    }
}

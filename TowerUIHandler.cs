using DG.Tweening;
using DG.Tweening.Core;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class handles the visuals of the tower
/// UI inventory. It sets the UI chip visuals accordingly
/// as well as animation tweening
/// </summary>

public class TowerUIHandler : MonoBehaviour
{
    [Header("References to the towers inventory slots")]
    public List<TowerSlot> TowerSlots;

    private Canvas canvas;
    private GameObject UI_Grid;
    private GameObject PurchaseUI;
    public Vector3 TowerOffset;

    private void Awake()
    {
        PurchaseUI = transform.GetChild(0).gameObject;
        UI_Grid = transform.GetChild(0).gameObject;
        canvas = GetComponentInParent<Canvas>();
        ZeroScale();
    }

    #region Start & EventSubs
    private void Start()
    {
        SelectedTower.Instance.OnTowerSelect += SetUIData;
    }
    private void OnDisable()
    {
        SelectedTower.Instance.OnTowerSelect -= SetUIData;
    }
    private void OnDestroy()
    {
        SelectedTower.Instance.OnTowerSelect -= SetUIData;
    }
    #endregion

    /// <summary>
    /// Returns a position in canvas space that is directly above
    /// the world position parameter
    /// </summary>
    /// <param name="parentCanvas"></param>
    /// <param name="worldPos"></param>
    /// <returns> Position in screen space </returns>
    public Vector3 WorldToUISpace(Canvas parentCanvas, Vector3 worldPos, Vector2 offset)
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentCanvas.transform as RectTransform, screenPos, parentCanvas.worldCamera, out Vector2 movePos);
        return parentCanvas.transform.TransformPoint(movePos + offset);
    }

    /// <summary>
    /// Called when the player clicks on a tower.
    /// It sets the UI chips to reflect the chips on the tower.
    /// </summary>
    /// <param name="tower"> The tower the event broadcast passed to this method </param>
    public void SetUIData(GameObject tower)
    {
        transform.GetChild(0).gameObject.SetActive(true);
        transform.position = WorldToUISpace(canvas, tower.transform.position, TowerOffset);

        TowerWeaponBehavior[] towerWeapons = tower.GetComponent<TowerBehavior>().TowerWeapons;
        for (int i = 0; i < TowerSlots.Count; i++)
        {
            TowerWeaponBehavior weapon = towerWeapons[i];
            if (!weapon.HasChip())
            {
                TowerSlots[i].SlottedChip.gameObject.SetActive(false);
            }
            else
            {
                TowerSlots[i].UpdateChipInstance(weapon.Chip);
                TowerSlots[i].SlottedChip.gameObject.SetActive(true);
            }
        }

        // pop in
        ZeroScale();
        TweenIn();
    }

    /// <summary>
    /// Sets the visual state of the tower UI
    /// </summary>
    /// <param name="state"></param>
    public void SetUIState(bool state)
    {
        SelectedTower.Instance.CurrentTower = null;
    }

    /// <summary>
    /// Activates either UI_Grid or PurchaseUI depending on the situation
    /// </summary>
    public void UIPurchasabilityCheck(bool purchasability)
    {
        Image[] gridImages = UI_Grid.GetComponentsInChildren<Image>();
        for (int i = 0; i < gridImages.Length; i++)
            gridImages[i].enabled = !purchasability;

        Image[] purchaseUIImages = PurchaseUI.GetComponentsInChildren<Image>();
        for (int i = 0; i < purchaseUIImages.Length; i++)
            purchaseUIImages[i].enabled = purchasability;
        Text[] purchaseUITexts = PurchaseUI.GetComponentsInChildren<Text>();
        for (int i = 0; i < purchaseUITexts.Length; i++)
            purchaseUITexts[i].enabled = purchasability;
    }
    public void UIPUrchasabilityCheck() => UIPurchasabilityCheck(SelectedTower.Instance.CurrentTower.GetComponent<TowerBehavior>().purchasable);

    /// <summary>
    /// Sets the scale of the tower UI to 0 for nice tweening
    /// </summary>
    public void ZeroScale()
    {
        UI_Grid.transform.localScale = Vector3.zero;
        PurchaseUI.transform.localScale = Vector3.zero;
    }

    /// <summary>
    /// Animates the UI in
    /// </summary>
    public void TweenIn()
    {
        UI_Grid.transform.DOScale(Vector3.one, 0.25f).From(UI_Grid.transform.localScale).SetEase(Ease.OutBack);
        PurchaseUI.transform.DOScale(Vector3.one, 0.25f).From(PurchaseUI.transform.localScale).SetEase(Ease.OutBack);
    }


    /// <summary>
    /// Animates the UI out
    /// </summary>
    public void TweenOut()
    {
        UI_Grid.transform.DOScale(Vector3.zero, 0.15f).From(UI_Grid.transform.localScale).SetEase(Ease.InBack);
        PurchaseUI.transform.DOScale(Vector3.zero, 0.15f).From(PurchaseUI.transform.localScale).SetEase(Ease.InBack);
    }
}

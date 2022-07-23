using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

/// <summary>
/// This class represents a visual representation of a chip in the UI.
/// Also uses Unity event systems interfaces to handles draging UI elements.
/// Pointer event data holds loads of information about object being interacted with.
/// </summary>

public class UI_ChipInstance : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [Header("Holds data for the chips type and tier")]
    [Tooltip("Reference to this chip's data")]
    public ChipInstance ThisChip;
    public Slot SlotRef;
    public TextMeshProUGUI TierText;
    public Image ThisSprite;
    [Tooltip("The glow sprite behind this chip, used to indicate valid merges")]
    public Image ThisGlow;
    [Tooltip("The numeric id of the slot - automatically obtained form parent slot")]
    public int SlotID = 0;

    #region Private Variables & Awake()
    private RectTransform rectTransform;
    private Vector3 startScale;
    private CanvasGroup canvasGroup;
    private Canvas draggingCanvas;
    private Canvas masterCanvas;
    private bool pickedUp = false;
    private bool validTarget = false;
    
    private Tween activeSpriteTween;
    private Tween activeTextTween;
    private Tween activeGlowTween;

    private void Awake()
    {
        masterCanvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();
        TierText = GetComponentInChildren<TextMeshProUGUI>();
        SlotRef = GetComponentInParent<Slot>();
        startScale = gameObject.transform.localScale;
        activeGlowTween = ThisGlow.DOFade(0,0);
    }

    void Start()
    {
        // retrieve slot id from parent slot
        SlotID = GetComponentInParent<Slot>().SlotID;
        if (ThisChip != null && ThisChip.ChipBaseRef != null)
        {
            ThisSprite.sprite = ThisChip.ChipBaseRef.ChipIcon;
        }
        draggingCanvas = GetComponentInParent<Slot>().OverlayCanvas;
    }

    void OnEnable()
    {
        ThisSprite.enabled = true;
        TierText.enabled = true;
    }

    void OnDisable()
    {
        ThisSprite.enabled = false;
        TierText.enabled = false;
    }

    #endregion

    // ===========================
    //  Interface Implementations
    // ===========================

    #region Interface Implementations

    /// <summary>
    /// Triggers at the beggining of drag
    /// </summary>
    /// <param name="eventData"></param>
    public void OnBeginDrag(PointerEventData eventData)
    {
        // prevent interaction if the chip is in a box
        if (SlotRef.BoxStatus != BoxState.None || SlotRef.Locked)
        {
            return;
        }

        // Record chip as picked up
        SelectedTower.Instance.PickupChip(ThisChip);
        pickedUp = true;
        EventManager.Instance.RaiseGameEvent(EventConstants.CHIP_PICKUP);

        // Set the chip parent to the overlay canvas
        eventData.pointerDrag.transform.SetParent(draggingCanvas.transform);
        canvasGroup.alpha = .7f;
        canvasGroup.blocksRaycasts = false;

        // Play pickup sound effect
        AudioManager.Instance.PlayAudio(AudioIdentifiers.ChipPickup);
    }

    /// <summary>
    /// Triggers every frame during drag
    /// moves the object with the mouse
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrag(PointerEventData eventData)
    {
        if (SlotRef.BoxStatus != BoxState.None || SlotRef.Locked)
        {
            return;
        }
        rectTransform.anchoredPosition += eventData.delta/masterCanvas.scaleFactor;
    }

    /// <summary>
    /// Triggers at end of drag
    /// </summary>
    /// <param name="eventData"></param>
    public void OnEndDrag(PointerEventData eventData)
    {
       //rectTransform.localPosition = eventData.pointerDrag.transform.position;
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
    }

    /// <summary>
    /// Triggers on mouse down
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerDown(PointerEventData eventData)
    {   
        #region Editor Hacks

        #if UNITY_EDITOR
            if (Input.GetKey(KeyCode.Space))
            {
                UI_ChipInstance uic = eventData.pointerEnter.GetComponentInParent<UI_ChipInstance>();
                uic.ThisChip.UpgradeTier();
                uic.TierText.text = uic.ThisChip.Tier.ToString();
                
                TowerSlot parentTowerSlot = GetComponentInParent<TowerSlot>();
                if (parentTowerSlot != null)
                {
                    GameObject targetTower = SelectedTower.Instance.CurrentTower;
                    if (targetTower != null)
                    {
                        TowerBehavior tb = targetTower.GetComponent<TowerBehavior>();
                        tb.ReplaceChip(tb.TowerWeapons[SlotID], ThisChip);
                        tb.UpdateStats();
                    }
                }
            }
        #endif

        #endregion
    }

    /// <summary>
    /// Handles releasing chips onto towers in the playing field
    /// </summary>
    /// <param name="eventData"> Event data holds data for the object being dragged</param>
    public void OnPointerUp(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        eventData.pointerDrag.GetComponent<RectTransform>().SetParent(SlotRef.transform);
        eventData.pointerDrag.GetComponent<RectTransform>().localPosition = Vector2.zero;
        transform.SetSiblingIndex(1);
        pickedUp = false;
        
        // Record chip as dropped
        SelectedTower.Instance.DropCurrentChip();
        EventManager.Instance.RaiseGameEvent(EventConstants.CHIP_DROP);
    }

    #endregion

    /// <summary>
    /// Fade chip in when first created
    /// </summary>
    public void FadeIn()
    {
        float fadeTime = 0.2f;
        // animate
        activeGlowTween = ThisGlow.DOFade(0,0f); // hide glow
        activeSpriteTween = ThisSprite.DOFade(1, fadeTime);
        activeTextTween = TierText.DOFade(1, fadeTime);
        gameObject.transform.localScale = Vector3.zero;
        if (!SlotRef.CompareTag("TowerSlot")) 
        { 
            gameObject.transform.DOScale(startScale, fadeTime).SetEase(Ease.OutBack,1.5f);
        }
    }

    /// <summary>
    /// Partially fade chip if invalid target for merge, or glow if valid target
    /// </summary>
    public void CheckValid()
    {
        if (!InventoryData.CanMerge(SelectedTower.Instance.CurrentChip, ThisChip))
        {
            validTarget = false;
            KillTweens();
            float fadeTime = 0.2f;
            float fadeValue = 0.6f;
            // animate
            activeSpriteTween = ThisSprite.DOFade(fadeValue, fadeTime);
            activeTextTween = TierText.DOFade(0.8f, fadeTime);
            activeGlowTween = ThisGlow.DOFade(0,fadeTime);
        }
        else if (!pickedUp)
        {
            validTarget = true;
            StopAllCoroutines();
            StartCoroutine(GlowPulse());
        }
    }

    /// <summary>
    /// Restore chip appearance once a chip has been dropped
    /// </summary>
    public void RestoreChip()
    {
        validTarget = false;
        KillTweens();
        float fadeTime = 0.1f;
        float fadeValue = 1f;
        // animate
        activeSpriteTween = ThisSprite.DOFade(fadeValue, fadeTime);
        activeTextTween = TierText.DOFade(fadeValue, fadeTime);
        activeGlowTween = ThisGlow.DOFade(0, fadeTime);
    }

    /// <summary>
    /// Reset current sprite and text tweens
    /// </summary>
    public void KillTweens()
    {
        activeSpriteTween.Kill();
        activeTextTween.Kill();
        activeGlowTween.Kill();
        StopAllCoroutines();
    }

    /// <summary>
    /// Makes the glow around the chip pulse until no longer a valid merge target
    /// </summary>
    private IEnumerator GlowPulse()
    {
        float minAlpha = 0.2f;
        float maxAlpha = 0.8f;
        float pulseTime = 0.4f;

        while (validTarget)
        {
            activeGlowTween = ThisGlow.DOFade(maxAlpha,pulseTime).SetEase(Ease.InCirc);
            yield return new WaitForSeconds(pulseTime);
            if (!validTarget)
            {
                yield return null;
            }
            activeGlowTween = ThisGlow.DOFade(minAlpha,pulseTime).SetEase(Ease.OutCirc);
            yield return new WaitForSeconds(pulseTime);
        }
        yield return null;
    }

    /// <summary>
    /// Updates the chip appearance in response to automatic upgrading
    /// </summary>
    public void UpdateVisualData()
    {
        if (ThisChip != null && ThisChip.ChipBaseRef != null)
        {
            ThisSprite.sprite = ThisChip.ChipBaseRef.ChipIcon;
            TierText.text = ThisChip.Tier.ToString();
        }
    }
}

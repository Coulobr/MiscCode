using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Advertisements;

/// <summary>
/// Subclass of slot. Same basic behavior, but also allows chips to arrive in a box that must be opened to access the chip
/// </summary>

public class ItemSlot : Slot, IPointerDownHandler
{
    [Header("Cog Effect")]
    public GameObject NormalRewardCogs;
    public GameObject BossRewardCogs;

    [Header("- Full Sprite")]
    [Header("Chip Box Images and Sprites")]
    [Tooltip("The full regular box image, used for animation")]
    public Image BoxImageFull;
    [Tooltip("The full ad box image, used for animation")]
    public Image AdboxImageFull;
    private Image boxAnimation;

    [Header("- Box Top")]
    [Tooltip("The top part of the chip's box image")]
    public Image BoxImageTop;
    private Vector3 boxImageTopPosition;
    [Tooltip("The sprite to use for the top part of the regular chip box")]
    public Sprite BoxTopSprite;
    [Tooltip("The sprite to use for the top part of the ad chip box")]
    public Sprite AdboxTopSprite;

    [Header("- Box Bottom")]
    [Tooltip("The bottom part of the chip's box image")]
    public Image BoxImageBottom;
    private Vector3 boxImageBottomPosition;
    [Tooltip("The sprite to use for the bottom part of the regular chip box")]
    public Sprite BoxBottomSprite;
    [Tooltip("The sprite to use for the bottom part of the ad chip box")]
    public Sprite AdboxBottomSprite;


    #region Box Status
    [Tooltip("Whether to show the box opening prompt")]
    private bool boxPrompt = false;
    #endregion

    void Awake()
    {
        boxImageTopPosition = BoxImageTop.rectTransform.localPosition;
        boxImageBottomPosition = BoxImageBottom.rectTransform.localPosition;
        OverlayCanvas = FindObjectOfType<DraggingCanvasTag>().GetComponent<Canvas>();
    }

    void Start()
    {
        BoxImageFull.gameObject.SetActive(false);
        AdboxImageFull.gameObject.SetActive(false);
    }

    #region Box Opening Methods
    /// <summary>
    /// Triggers on mouse down
    /// Unlocks the chip box if able
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerDown(PointerEventData eventData)
    {
        if (BoxStatus == BoxState.Box || (BoxStatus == BoxState.Adbox && !boxPrompt))
        {
            StartCoroutine(OpenBox());
        }
    }

    /// <summary>
    /// Apply the designated box type to this slot
    /// </summary>
    private void SetupBox(BoxState newBox)
    {
        BoxStatus = newBox;
        switch (newBox)
        {
            case BoxState.Box:
                // set box to look like regular box
                BoxImageTop.sprite = BoxTopSprite;
                BoxImageBottom.sprite = BoxBottomSprite;
                boxAnimation = BoxImageFull;
                break;
            case BoxState.Adbox:
                // set box to look like lootbox
                BoxImageTop.sprite = AdboxTopSprite;
                BoxImageBottom.sprite = AdboxBottomSprite;
                boxAnimation = AdboxImageFull;
                break;
            default:
                // early exit if no box applied
                return;
        }

        // reveal the box
        GameObject boxTop = BoxImageTop.gameObject;
        GameObject boxBottom = BoxImageBottom.gameObject;
        float animTime = 0.1f;
        float startScale = 0.1f;
        // - reset position and alpha
        BoxImageTop.rectTransform.localPosition = boxImageTopPosition*startScale;
        BoxImageBottom.rectTransform.localPosition = boxImageBottomPosition*startScale;
        BoxImageTop.DOFade(1,0);
        BoxImageBottom.DOFade(1,0);
        // - enable and animate
        boxTop.SetActive(true);
        boxBottom.SetActive(true);
        boxAnimation.gameObject.SetActive(true);
        BoxImageTop.rectTransform.DOLocalMove(boxImageTopPosition, animTime);
        BoxImageBottom.rectTransform.DOLocalMove(boxImageBottomPosition, animTime);
        boxTop.transform.DOScale(startScale, animTime).From().SetEase(Ease.OutExpo);
        boxBottom.transform.DOScale(startScale, animTime).From().SetEase(Ease.OutExpo);
        boxAnimation.transform.DOScale(startScale, animTime).From().SetEase(Ease.OutExpo);
    }



    /// <summary>
    /// Handle behavior for opening a box
    /// </summary>
    private IEnumerator OpenBox()
    {
        //Event broadcast
        EventManager.Instance.RaiseGameEvent(EventConstants.OPEN_CHIP_BOX);

        //Visual bolts going to money area
        if (BoxStatus == BoxState.Adbox)
        {
            SpawnBossCogs();
        }
        else
        {
            SpawnNormalCogs();
        }

        // IF ADDS ARE ENABLED UNCOMMENT THIS
        //if (BoxStatus == BoxState.Adbox)
        //{
        //    boxPrompt = true;
        //    EventManager.Instance.RaiseGameEvent(EventConstants.PROMPT_REWARD_AD);
        //    // boxPrompt is set to false via external event once the prompt is closed
        //    yield return new WaitUntil(() => !boxPrompt);
        //}

        // Box unlock animation
        AudioManager.Instance.PlayAudio(AudioIdentifiers.BoxOpen);
        BoxStatus = BoxState.Opening;
        ImageAnimator anim = boxAnimation.gameObject.GetComponent<ImageAnimator>();
        anim.StartAnimation();
        yield return new WaitUntil(() => anim.AnimationDone);

        // Box open animation
        BoxStatus = BoxState.None;
        anim.ResetAnimation();
        boxAnimation.gameObject.SetActive(false);
        float animTime = 0.2f;
        Vector3 moveVector = Vector3.up*0.5f;
        BoxImageTop.transform.SetParent(OverlayCanvas.transform); // move to overlay
        BoxImageBottom.transform.SetParent(OverlayCanvas.transform);
        BoxImageTop.DOFade(0,animTime).SetEase(Ease.InQuad);
        BoxImageBottom.DOFade(0,animTime).SetEase(Ease.InQuad);
        BoxImageTop.transform.DOMove(transform.position+moveVector,animTime);
        BoxImageBottom.transform.DOMove(transform.position-moveVector,animTime);
        yield return new WaitForSeconds(animTime);
        
        // Complete opening
        BoxImageBottom.transform.SetParent(transform); // return from overlay
        BoxImageBottom.transform.SetSiblingIndex(2);
        BoxImageTop.transform.SetParent(transform);
        BoxImageTop.transform.SetSiblingIndex(3);
        BoxImageTop.gameObject.SetActive(false);
        BoxImageBottom.gameObject.SetActive(false);
        BoxStatus = BoxState.None;

        yield return null;
    }
    #endregion

    #region Drag/Drop & Merging Methods
    /// <summary>
    /// - Method is called when an object is dragged onto it.
    /// - EventData holds information about the incoming object that was dropped.
    /// </summary>
    /// <param name="eventData"></param>
    public override void OnDrop(PointerEventData eventData)
    {
        var incomingChipInstance = eventData.pointerDrag.GetComponent<UI_ChipInstance>();

        // -- If the chip was placed onto itself, the slot is locked, the chip's slot is locked, or the chip is in a box -- \\
        if (DoesChipMatch(incomingChipInstance) || Locked || incomingChipInstance.SlotRef.Locked || BoxStatus != BoxState.None)
        {
            return;
        }

        // -- If the chip was placed onto another chip -- \\
        if (IsSlotFull)
        {
            bool mergeSuccess;
            this.Merge(incomingChipInstance, out mergeSuccess);
            if (mergeSuccess)
            {
                EventManager.Instance.RaiseGameEvent(EventConstants.MERGE);
                AudioManager.Instance.PlayAudio(AudioIdentifiers.ChipMerge);
            }
            else
            {
                AudioManager.Instance.PlayAudio(AudioIdentifiers.ChipDrop);
            }
        }
        else
        {
            this.UpdateChipData(incomingChipInstance);
            SlottedChip.gameObject.SetActive(true);

            // -- If the dropped chip is from a tower slot, we must remove it from that tower weapon slot-- \\
            if (incomingChipInstance.transform.parent.CompareTag("TowerSlot"))
            {
                SelectedTower.Instance.RemoveChip(incomingChipInstance.ThisChip, incomingChipInstance.SlotID);
            }
            ResetChipData(incomingChipInstance);
            AudioManager.Instance.PlayAudio(AudioIdentifiers.ChipDrop);
        }
    }

    #endregion

    #region UtilityMethods

    public void SpawnNormalCogs()
    {
        NormalRewardCogs.SetActive(true);
    }
    public void SpawnBossCogs()
    {
        BossRewardCogs.SetActive(true);
    }

    /// <summary>
    /// Updates the local UI_ChipInstance's data to the passed chipInstance
    /// </summary>
    /// <param name="instance">The chip instance to create</param>
    /// <param name="isBoxed">Whether to spawn the chip in a box or lootbox (default: BoxState.None)</param>
    public override void UpdateChipInstance(ChipInstance instance, BoxState boxState=BoxState.None)
    {
        // update chip appearance
        SlottedChip.TierText.text = instance.Tier.ToString();
        if (instance.ChipBaseRef != null)
        {
            SlottedChip.ThisSprite.sprite = instance.ChipBaseRef.ChipIcon;
        }
        SlottedChip.ThisChip = instance;
        SlottedChip.gameObject.SetActive(true);

        SetupBox(boxState);
    }

    /// <summary>
    /// Updates the local chip to the passed UI_ChipInstance
    /// </summary>
    /// <param name="instance"> The local chip's data will be assinged to this parameter</param>
    public override void UpdateChipData(UI_ChipInstance instance, BoxState boxState=BoxState.None)
    {
        UpdateChipInstance(instance.ThisChip, boxState);
    }

    /// <summary>
    /// Sets the state of the boxPrompt bool so the OpenBox coroutine can finish
    /// This is called via event listener on the itemslot
    /// </summary>
    /// <param name="state"></param>
    public void BoxPromptState(bool state)
    {
        boxPrompt = state;
    }

    /// <summary>
    /// Sets the chip value in the box back to the default if they decline the advertisment
    /// </summary>
    public void RewardAdDecline()
    {
        if(BoxStatus == BoxState.Adbox)
        {
            SlottedChip.ThisChip.Tier = PlayerStats.Instance.ChipEconomy;
            UpdateChipInstance(SlottedChip.ThisChip);
            boxPrompt = false;
        }
    }
    #endregion
}

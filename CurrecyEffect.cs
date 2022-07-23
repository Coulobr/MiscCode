using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CurrecyEffect : MonoBehaviour
{
    public float PricePerCog;
    public List<GameObject> CogChildren;

    private Canvas gameplayCanvas;
    private RectTransform moneyText;
    private ItemSlot  parentSlot;
    private bool effectComplete;
    public RectTransform EndPos;
    public RectTransform ForegroundCanvas;
    public CurrencyIcon CurrencyIcon;

    public bool EffectComplete { 
        get { return effectComplete; }  
    }

    private void Awake()
    {

    }
    private void OnEnable()
    {
        Initialize();
    }

    public void Initialize()
    {
        parentSlot = GetComponentInParent<ItemSlot>();
        transform.SetParent(ForegroundCanvas);
        StartCoroutine(LaunchCogs());
    }

    /// <summary>
    /// Set the end position of the cogs and enable them.
    /// Movement handled in FlyingCog.cs
    /// </summary>
    public IEnumerator LaunchCogs()
    {
        yield return new WaitForSeconds(.25f); //Open box anim delay
        for (int i = 0; i < CogChildren.Count; i++)
        {
            CogChildren[i].GetComponent<FlyingCog>().EndPosition = EndPos.anchoredPosition;
            CogChildren[i].GetComponent<FlyingCog>().StartPosition = Vector2.zero;
            CogChildren[i].SetActive(true);
        }
        yield return 0;
    }

    /// <summary>
    /// Called via each cog child. Doesnt resolve unless all cogs have finished
    /// </summary>
    private int cogEffectCompleteCounter = 0;
    public void EffectCompleteCallback()
    {
        cogEffectCompleteCounter++;
        CurrencyIcon.Animate();
        PlayerStats.Instance.AddCurrency(PricePerCog);
        AudioManager.Instance.PlayAudio(AudioIdentifiers.ChipPickup);

        if (cogEffectCompleteCounter >= CogChildren.Count)
        {
            effectComplete = true;
            cogEffectCompleteCounter = 0;
            RepoolCogs();
        }
    }

    /// <summary>
    /// Reset cog position and reset parent
    /// </summary>
    public void RepoolCogs()
    {
        for (int i = 0; i < CogChildren.Count; i++)
        {
            CogChildren[i].GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        }

        effectComplete = false;
        transform.SetParent(parentSlot.transform);
        gameObject.SetActive(false);    
    }
}

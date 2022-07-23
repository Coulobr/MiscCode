using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Handles logic for updating the wave counter
/// </summary>

public class WaveCounter : Counter
{
    private TextMeshPro textComponent;
    private RectTransform rectTransform;
    private void Awake()
    {
        textComponent = GetComponent<TextMeshPro>();
        rectTransform = GetComponent<RectTransform>();
        textComponent.text = "1";
        baseText = textComponent.text;
    }

    public void SetWaveNum()
    {
        this.count = PlayerStats.Instance.CurrentWave;
        UpdateUI();
    }

    #region Overrides
    public override void Increment()
    {
        this.count++;
        UpdateUI();
    }

    public override void Decrement()
    {
        this.count--;
        UpdateUI();
    }

    public override void UpdateUI()
    {
        textComponent.text = (count).ToString();
        textComponent.text = PlayerStats.Instance.CurrentWave.ToString();
    }
    #endregion

}

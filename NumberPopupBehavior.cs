using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

/// <summary>
/// Floating text used to show changes in money
/// </summary>

public class NumberPopupBehavior : MonoBehaviour
{
    [Header("References")]
    private MoneyCounter mc;
    private TextMeshProUGUI DisplayedText;
    private RectTransform rectTransform;

    [Header("Internal State")]
    private float value;
    private float timer = 0;
    [Tooltip("How long the animation will last, in seconds")]
    public float TimerMax;
    [Tooltip("The X offset from the target position")]
    public float xOffset = 0;
    [Tooltip("The initial Y offset from the target position")]
    public float yOffset = 0;
    [Tooltip("The position to move towards")]
    private Vector3 targetPosition;
    private bool active = false;
    
    [Header("Colors")]
    [Tooltip("The how intensely positive and negative values affect the popup color")]
    public float ColorIntensity = 0.1f;
    private float redChannel = 1;
    private float greenChannel = 1;
    private float blueChannel = 1;

    /// <summary>
    /// Disable the object and return it to the pool
    /// </summary>
    private void RePool()
    {
        rectTransform.localPosition = rectTransform.localPosition + new Vector3(0, -yOffset, 0);
        DisplayedText.enabled = false;
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Handle initial setup
    /// </summary>
    void Awake()
    {
        mc = GetComponentInParent<MoneyCounter>();
        DisplayedText = GetComponent<TextMeshProUGUI>();
        rectTransform = GetComponent<RectTransform>();
        RePool();
    }

    /// <summary>
    /// Use to externally initialize the text displayed
    /// </summary>
    public void Setup(float newNumber, Vector3 newPosition)
    {
        value = newNumber;
        targetPosition = newPosition;
        
        // update displayed value
        string prefix = "";
        if (value >= 0)
        {
            prefix = "+";
        }
        DisplayedText.text = prefix + value;
        Activate();
    }

    /// <summary>
    /// Begin animation
    /// </summary>
    private void Activate()
    {
        active = true;
        timer = 0;
        // adjust tint based on magnitude and whether the value is positive or negative
        // Negative -> Yellow (this comes from purchasing a Chip)
        // Positive -> Cyan (this comes from defeating enemies)
        float colorShift = value*ColorIntensity;
        redChannel = Mathf.Max(0,1+Mathf.Min(-colorShift,0));
        blueChannel = Mathf.Max(0,1+Mathf.Min(colorShift,0));
        UpdateVisual();
        
        //transform.parent.SetAsLastSibling();
        DisplayedText.enabled = true;
    }

    /// <summary>
    /// Update animation if active
    /// </summary>
    void Update()
    {
        if (active)
        {
            timer += Time.deltaTime;
            // resolve at end of timer
            if (timer >= TimerMax)
            {
                active = false;

                // update the displayed money value
                mc.UpdateDisplayedMoney(value);
                // hide self
                RePool();
            }
        }
    }

    /// <summary>
    /// Update position and color
    /// </summary>
    private void UpdateVisual()
    {
        // update postion
        //transform.position = targetPosition + new Vector3(xOffset,yOffset*(timer/TimerMax),0);

        rectTransform.DOAnchorPosY(0, TimerMax);
        // update color
        //float opacity = Mathf.Sin(Mathf.PI*timer/TimerMax);
        DisplayedText.color = new Color(redChannel,greenChannel,blueChannel,1);
    }
}

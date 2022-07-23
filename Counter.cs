using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Generic Counter to be used via
/// game event listeners.
/// </summary>

public abstract class Counter : MonoBehaviour
{
    [TextArea]
    public string note = "Add an event listener to call methods of this class";

    [SerializeField]
    protected TextMeshProUGUI DisplayedText;
    protected string baseText;
    protected float count = 0;
    void Awake()
    { 

        DisplayedText = GetComponent<TextMeshProUGUI>();
        baseText = DisplayedText.text;
        UpdateUI();
    }

    #region Event Responses
    public abstract void Increment();
    public abstract void Decrement();
    public abstract void UpdateUI();
    #endregion

}

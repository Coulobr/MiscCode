using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// More complex variant of Counter for tracking the player's money
/// </summary>

public class MoneyCounter : Counter
{    
    [Tooltip("The actual amount of money the buy button knows the player has.")]
    private float trackedMoney = 0;

    // count (inherited int) is the amount of money displayed

    [Tooltip("The pool of number popups")]
    private List<GameObject> popupPool = new List<GameObject>();
    
    [Tooltip("Location the popups will be stored")]
    public GameObject ContentPool;

    [Tooltip("The size of the number popup pool")]
    public int PopupPoolSize;
    [Tooltip("The prefab to use as a template for number popups that appear under the counter")]
    public GameObject PopupTemplate;

    void Start() => StartCoroutine(InitializeClassData());

    private IEnumerator InitializeClassData()
    {
        // Delay to ensure that the savemanager has loaded the correct currency to the player stats
        yield return new WaitForSeconds(.2f);

        count = PlayerStats.Instance.Money;
        trackedMoney = PlayerStats.Instance.Money;

        UpdateUI();

        for (int i = 0; i < PopupPoolSize; i++)
        {
            AddNewPopup();
        }
        yield return 0;
    }

    #region Event Responses

    /// <summary>
    /// Update the internally tracked money value
    /// </summary>
    public void UpdateTrackedMoney()
    {
        float newValue = PlayerStats.Instance.Money;
        if (trackedMoney != newValue)
        {
            // Queue up mondey display change
            float moneyChange = newValue - trackedMoney;
            GameObject popup = GetPopup();
            popup.GetComponent<NumberPopupBehavior>().Setup(moneyChange, transform.position);   
            // Update money value
            trackedMoney = newValue;

        }
    }
    
    #endregion

    #region Updating Display

    /// <summary>
    /// Update the displayed money value by the specified amount
    /// </summary>
    /// <param name="value">The value to change the displayed money by</param>
    public void UpdateDisplayedMoney(float value)
    {
        count += value;
        UpdateUI();
    }

    /// <summary>
    /// Instantiate a new Number Popup and add it to the popup pool
    /// <return>Returns the new Popup</return>
    /// </summary>
    private GameObject AddNewPopup()
    {
        GameObject popupInstance;
        popupInstance = Instantiate(PopupTemplate, ContentPool.transform);
        popupPool.Add(popupInstance);
        return popupInstance;
    }

    /// <summary>
    /// Get an inactive number popup from the Popup pool, instantiating a new one if necessary, and enable it
    /// <return>The chosen Popup</return>
    /// </summary>
    private GameObject GetPopup()
    {
        GameObject chosenPopup = null;
        // attempt to get first inactive Popup
        for (int i = 0; i < PopupPoolSize; i++)
        {
            GameObject popup = popupPool[i];
            if (!popup.activeSelf)
            {
                chosenPopup = popup;
                break;
            }
        }
        // if no popups available, instantiate a new one and expand pool
        if (!chosenPopup)
        {
            chosenPopup = AddNewPopup();
            PopupPoolSize++;
        }
        // enable and return the chosen Popup
        chosenPopup.SetActive(true);
        return chosenPopup;
    }

    public override void Increment()
    {
    }

    public override void Decrement()
    {
    }

    public override void UpdateUI()
    {
        this.DisplayedText.text = trackedMoney.ToString();
    }


    #endregion

}

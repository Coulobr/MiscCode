using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;


[CreateAssetMenu(menuName = "ScriptableObjects/AreaProgressionData")]

public class AreaProgression : SingletonScriptableObject<AreaProgression>
{
    [Tooltip("If disabled there will be no event calls for progression")]
    public bool EnableProgression;

    /// <summary>
    /// Returns the current area based on where the cuurrent active scene is in the progressable areas list
    /// </summary>
    public int CurrentArea
    {
        get {
            for (int i = 0; i < AllProgressableAreas.Count; i++)
            {
                if(AllProgressableAreas[i].sceneAssetName == SceneManager.GetActiveScene().name)
                {
                    return i;
                }
            }
            return 0;
        }
    }

    
    public List<Areas> AllProgressableAreas;

    private bool promptedNotification = false;
    /// <summary>
    /// If the player has already propted the notiofication to progress to the next area
    /// </summary>
    public bool PromptedNotification
    {
        get { return promptedNotification; }
        set { promptedNotification = value; }
    }

    public float RemainingCostToProgress
    {
        get
        {
            if (AllProgressableAreas[CurrentArea].costToProgress - PlayerStats.Instance.Money > 0)
            {
                return AllProgressableAreas[CurrentArea].costToProgress - PlayerStats.Instance.Money;
            }
            else
            {
                return 0;
            }
        }
    }

    public float CurrentAreaProgCost
    {
        get
        {
            return AllProgressableAreas[CurrentArea].costToProgress;
        }
    }

    /// <summary>
    /// Resets this scriptable obejct back to defaults
    /// </summary>
    public void ResetData() => PromptedNotification = false;
    

    /// <summary>
    /// Handles progressing to the next xcene in the scene asset list & removing player currency
    /// </summary>
    public void ProgressArea()
    {
        if (PlayerStats.Instance.Purchase(CurrentAreaProgCost)) // Spends $ if true
        {
            // Reset notification prompt if theres a next area
            if (NextAreaAvailable())
            {
                Instance.PromptedNotification = false;
            }

            // Save the game before loading new scene
            if (EventManager.Instance.RaiseGameEvent(EventConstants.SAVE_GAME))
            {
                //Load the next scene in the list based on the new current area we just incremented
                LoadSceneAsset(AllProgressableAreas[CurrentArea + 1].sceneAssetName);
                Debug.Log("Loading scene asset: " + (AllProgressableAreas[CurrentArea + 1].sceneAssetName) + "| Current Area: " + CurrentArea);
            }
        }
    }

    /// <summary>
    /// Checks if the player has enough money to advance to the next Area;
    /// We do this via event call on the AreaProgression Object so we can popup a notification
    /// </summary>
    /// <returns></returns>
    public void HasMoneyToPorgress()
    {
        // Only run if progression is enabled in the inspector.
        if (!Instance.EnableProgression)
        {
            Debug.LogWarning("Progression Disabled in the AreaProgression asset");
            return;
        }

        // If true, player has enough
        if (!Instance.PromptedNotification && CanProgress())
        {
            Instance.PromptedNotification = true;
            EventManager.Instance.RaiseGameEvent(EventConstants.NEXT_AREA_PURCHASEABLE);
        }
    }

    /// <summary>
    /// Loads a new scene and doing generic functions
    /// </summary>
    /// <param name="asset"></param>
    public void LoadSceneAsset(string assetName)
    {
        SceneManager.LoadSceneAsync(assetName);
    }

    //If the player has enough money 
    public bool CanProgress()
    {
        return Instance.RemainingCostToProgress == 0   
                && NextAreaAvailable();
    }

    public bool NextAreaAvailable()
    {
        return Instance.CurrentArea < Instance.AllProgressableAreas.Count - 1;
    }

}

[System.Serializable]
public struct Areas
{
    public string sceneAssetName;
    public float costToProgress;
}

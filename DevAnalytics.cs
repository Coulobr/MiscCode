using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DevToDev;
using UnityEngine.SceneManagement;

public class DevAnalytics : MonoBehaviour
{
    #region DelagateSubscriptions

    // Used for total time played
    private float totalTimePlayed = 0;
    private float tick = 0;
    private void Update()
    {
        if (SceneManager.GetActiveScene().name != "MainMenu") 
        { 
            UpdateTotalTimePlayed(); 
        }
    }

    #endregion
    public void CustomEvent(string eventName, CustomEventParams eventParams)
    {
        Analytics.CustomEvent(eventName, eventParams);
    }

    /// <summary>
    /// Tracks what wave the player lost on
    /// </summary>
    public void PlayerLoseWaveEvent()
    {
        CustomEventParams customParams = new CustomEventParams();
        customParams.AddParam("int", PlayerStats.Instance.CurrentWave);

        CustomEvent("WaveLost", customParams);
    }

    /// <summary>
    /// Tracks what wave the player won on
    /// </summary>
    public void PlayerWinWaveEvent()
    {
        CustomEventParams customParams = new CustomEventParams();
        customParams.AddParam("int", PlayerStats.Instance.CurrentWave);

        CustomEvent("WaveWon", customParams);
    }

    /// <summary>
    /// (In seconds) Tracks total time played on a save.
    /// </summary>
    public void TrackTotalTimePlayed()
    {
        CustomEventParams eventParams = new CustomEventParams();
        eventParams.AddParam("SecondsPlayed", PlayerStats.Instance.TotalSecondsPlayed);
        CustomEvent("TotalSecondsPlayedEvent", eventParams);
    }

    private void UpdateTotalTimePlayed()
    {
        tick += Time.deltaTime;
        totalTimePlayed += Time.deltaTime;
        if (tick > 10f)
        {
            PlayerStats.Instance.TotalSecondsPlayed = Mathf.RoundToInt(totalTimePlayed);
            TrackTotalTimePlayed();
            tick = 0;
        }
    }

    ///// <summary>
    ///// Tracks the size of the overflow queue, if its very large, the player has been AFK for a while
    ///// </summary>
    //public void PlayerIdleTracker()
    //{
    //    CustomEventParams customParams = new CustomEventParams();
    //    customParams.AddParam("int", InventoryData.Instance.OverflowQueue.Count);
    //    CustomEvent("PlayerIdleTracking", customParams);
    //}


    /// <summary>
    /// Every 5 upgrade tiers the event is called and tracks at what level they got his milestone.
    /// This will allow us to tell how fast players are progressing
    /// </summary>
    public void PlayerChipMilestoneEvent()
    {
        Debug.Log("Sending Analytic Event (ChipMilestone) ");
        CustomEventParams customParams = new CustomEventParams();
        customParams.AddParam("int", PlayerStats.Instance.HighestTierObtained);
        CustomEvent("ChipMilestone", customParams);
    }

    public void AdBoxReward(int watchedAdd)
    {
        Debug.Log("Sending Analytic Event (AdBoxReward) ");
        CustomEventParams customParams = new CustomEventParams();
        customParams.AddParam("int", watchedAdd);
        CustomEvent("WatchedAdBool", customParams);
    }

    public void TrackTutorialComplete()
    {
        CustomEventParams customParams = new CustomEventParams();
        customParams.AddParam("PassedTutorial", 1);
        CustomEvent("PassedTutorialEvent", customParams);
    }

    public void TrackAreaProgression()
    {
        CustomEventParams customParams = new CustomEventParams();
        customParams.AddParam("CurrentArea", AreaProgression.Instance.CurrentArea + 1);
        CustomEvent("AreaProgressionEvent", customParams);
    }

    /// <summary>
    /// Begins a Progression Event
    /// </summary>
    public void StartLevelProgressionEvent()
    {
        ProgressionEventParams parm = new ProgressionEventParams();
        parm.SetSource("Wave " + (PlayerStats.Instance.CurrentWave - 1).ToString());
        Analytics.StartProgressionEvent("Wave " + PlayerStats.Instance.CurrentWave.ToString(),parm);
    }

    /// <summary>
    /// Ends progression event after losing a wave
    /// </summary>
    public void EndUnsuccessfulLevelProgressionEvent()
    {
        ProgressionEventParams parm = GetProgressionEventStats(false);
        Analytics.EndProgressionEvent("Wave " + PlayerStats.Instance.CurrentWave.ToString(), parm);
    }

    /// <summary>
    /// Ends progression event after beating a wave
    /// </summary>
    public void EndSuccessfulLevelProgressionEvent()
    {
        ProgressionEventParams parm = GetProgressionEventStats(true);
        Analytics.EndProgressionEvent("Wave " + PlayerStats.Instance.CurrentWave.ToString(), parm);
    }

    /// <summary>
    /// Returns the stats from a level
    /// </summary>
    /// <param name="success"></param>
    /// <returns></returns>
    public ProgressionEventParams GetProgressionEventStats(bool success)
    {
        Dictionary<string, int> spent = new Dictionary<string, int>();
        Dictionary<string, int> earned = new Dictionary<string, int>();

        ProgressionEventParams parm = new ProgressionEventParams();
        parm.SetEarned(earned);
        parm.SetSpent(spent);
        //parm.SetDuration(Convert.ToInt64(Time.fixedTime - startTime));
        parm.SetSuccessfulCompletion(success);
        return parm;
    }


}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.Monetization;

public class AdHandler : MonoBehaviour, IUnityAdsListener
{
#if UNITY_IOS
    private string gameID = "3762858";
#elif UNITY_ANDROID
    private string gameID = "3762859";
#elif UNITY_STANDALONE_WIN	
        private string gameID = "3762860";
#endif

    private const string REWARD_BOX_AD = "AcceptedAdBox";

    // Rewards a chip every once and a while 
    //private float adChipTimer = 0;
    //private float adChipInterval = 360;

    #region EventSubs

    #endregion

    private void Start()
    {
        Advertisement.AddListener(this);
        Advertisement.Initialize(gameID, true);
    }
    private void Update()
    {
        //adChipTimer += Time.deltaTime;
        //if (adChipTimer > adChipInterval)
        //{
        //    var randChip = Random.Range(0, PlayerStats.Instance.NumChipTypesAvailable);
        //    ChipBase chipType = PlayerStats.Instance.AvailableChipTypes[randChip];
        //    // Bonus 2 tiers for watching ad
        //    int tier = PlayerStats.Instance.ChipEconomy + 2;
        //    // Create and add new chip
        //    if (!InventoryData.Instance.IsFull() && Advertisement.GetPlacementState(REWARD_BOX_AD) == PlacementState.Ready)
        //    {
        //        InventoryData.Instance.AddNewChip(new ChipInstance { ChipBaseRef = chipType, Tier = tier }, BoxState.Adbox);
        //        adChipTimer = 0;
        //    }
        //    else
        //    {
        //    }
        //}
    }
    private void OnDestroy()
    {
        Advertisement.RemoveListener(this); 
    }

    public void RewardAd(bool watchedAdd)
    {
        Debug.Log("Called reward add, player selected: " + watchedAdd);
        if (watchedAdd) //if true
        {
            PlacementState adState = Advertisement.GetPlacementState(REWARD_BOX_AD);
            Debug.Log("Ad State: " + adState.ToString());

            if (adState == PlacementState.Ready)
            {
                Advertisement.Show(REWARD_BOX_AD);
            }
        }
        else
        {
            Debug.Log("Player passed on advertisment");
        }
    }

    public void OnUnityAdsDidFinish(string placementId, UnityEngine.Advertisements.ShowResult showResult)
    {
        if (showResult == UnityEngine.Advertisements.ShowResult.Finished)
        {
            EventManager.Instance.RaiseGameEvent(EventConstants.REWARD_AD_FINISH);
        }
    }

    public void PauseForAd()
    {
        Time.timeScale = 0;
    }
    public void Unpause()
    {
        Time.timeScale = 1;
    }

    public void OnUnityAdsReady(string placementId)
    {

    }

    public void OnUnityAdsDidError(string message)
    {
        message = "Error";
    }

    public void OnUnityAdsDidStart(string placementId)
    {
    }

}

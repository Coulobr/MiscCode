using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains string representations of each event name.  
/// This class was constructed in order to collect all 
/// events into the same place.  
///
/// When adding a new event, you MUST also add it to this script.
/// 
/// A future optimization to this may be setting the value
/// of each event name via passing in the events to the inspector, as it
/// would save manual inputting of names.  However, this way also 
/// forces us to confirm spelling + purpose with each event name!
/// </summary>

public class EventConstants : MonoBehaviour
{
    #region Constants
    public const string SPAWNING_START = "OnSpawningStart";
    public const string WAVE_RESET = "OnWaveReset";
    public const string WAVE_COMPLETE = "OnWaveComplete";
    public const string WAVE_DEFEAT = "OnWaveDefeat";
    public const string WAVE_REVERT = "OnWaveRevert";
    public const string ENEMY_DEATH = "OnEnemyDeath";
    public const string DESELECT_TOWERS = "OnDeselectTower";
    public const string SELECT_TOWER = "OnSelectTower";
    public const string UPDATE_MONEY = "OnUpdateMoney";
    public const string UPDATE_INVENTORY = "OnUpdateInventory";
    public const string LEVEL_UP = "OnLevelUp";
    public const string MERGE = "OnMerge";
    public const string PURCHASE = "OnPurchase";
    public const string LOCK_ITEMS = "OnLockItems";
    public const string UNLOCK_ITEMS = "OnUnlockItems";
    public const string LOCK_TOWERS = "OnLockTowers";
    public const string UNLOCK_TOWERS = "OnUnlockTowers";
    public const string LOCK_DISCARD = "OnLockDiscard";
    public const string UNLOCK_DISCARD = "OnUnlockDiscard";
    public const string CHIP_PROGRESSION = "OnChipProgression";
    public const string CHIP_PICKUP = "OnChipPickup";
    public const string CHIP_DROP = "OnChipDrop";
    public const string LOADING_SAVE = "OnLoadingSave";
    public const string PAUSE = "OnPause";
    public const string UNPAUSE = "OnUnpause";
    public const string ADD_TO_OVERFLOW = "OnAddOverflow";
    public const string REMOVE_FROM_OVERFLOW = "OnRemoveOverflow";
    public const string ON_TUTORIAL_COMPLETE = "OnTutorialComplete";
    public const string ON_TUTORIAL_START = "OnTutorialStart";
    public const string START_REWARD_AD = "OnRewardAd";
    public const string REWARD_AD_FINISH = "OnRewardAdFinish";
    public const string PROMPT_REWARD_AD = "OnOfferRewardAd";
    public const string DECLINE_REWARD_AD = "OnRewardAdDecline";
    public const string AREA_PROGRESS = "OnAreaProgress";
    public const string NEXT_AREA_PURCHASEABLE = "OnNextAreaPurchasable";
    public const string SAVE_GAME = "OnSaveGame";
    public const string OPEN_CHIP_BOX = "OnOpenChipBox";  
    public const string SPAWN_BOSS = "OnBossSpawn";  
    public const string BOSS_INCOMING = "OnBossIncoming";  
    public const string LOAD_FROM_MAIN_MENU = "OnLoadFromMainMenu";  
    #endregion
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EditorInvokable;
using UnityEditor;

/// <summary>
/// This scriptable object holds all the stats and
/// scaling for the three bullet types
/// </summary>

[CreateAssetMenu(menuName = "ScriptableObjects/BulletStatsScaling")]
public class BulletStats : SingletonScriptableObject<BulletStats>
{
    [System.Serializable]
    public struct TierLevel
    {
        public int Level;
        public int Tier;
    }

    [Header("--- SCALING ---")]

    [Header("Ice Bullet Stats")]
    public AnimationCurve SlowIntensity;
    public AnimationCurve SlowDuration;

    [Header("Plasma Bullet Stats")]
    public AnimationCurve FireExplosionRadius;
    public AnimationCurve FireAoeDuration;
    public AnimationCurve FireAoeTickDamage;
    public AnimationCurve FireAoeTickRate;

    [Header("Lightning Bullet Stats")]
    public AnimationCurve LightningSpread;
    public AnimationCurve PercentLightningFalloffDamage;

    [Header("Upgrade Tiers")]
    [Tooltip("Maps chip tier to visual upgrade level. MUST BE IN LEVEL ORDER!")]
    public TierLevel[] UpgradeLevels;
    
    #region Methods
    
    /// <summary>
    /// Gets the upgrade level corresponding to the upgrade tier
    /// </summary>
    /// <param name="tier">The value of the upgrade tier</param>
    public int GetUpgradeLevel(int tier)
    {
        int highestLevel = 0;
        for (int  i = 0; i < UpgradeLevels.Length; i++)
        {
            TierLevel tl = UpgradeLevels[i];
            if (tl.Level > highestLevel && tl.Tier <= tier)
            {
                highestLevel = tl.Level;
            }
        }
        return highestLevel;
    }

    #endregion

    #region Editor Functions

    /// <summary>
    /// Reset to default values
    /// </summary>
    [ExposeMethodInEditor]
    public void MarkAsChanged()
    {
        Debug.Log("You've marked the BulletStatsScaling SO as changed!\nNow save it to include it on your next Plastic push.");
        #if UNITY_EDITOR
        EditorUtility.SetDirty(this);
        #endif
    }

    #endregion
}

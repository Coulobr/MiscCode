using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Describes the attribute, stats, and stat scaling of a type of Chip
/// <summary>

[CreateAssetMenu(fileName = "NewChipBase", menuName = "ScriptableObjects/ChipBase", order = 1)]
public class ChipBase : ScriptableObject
{
    [Tooltip("The Attribute of the Chip (its elemental type)")]
    public Attributes Attribute;
    [TextArea(3,3)]
    public string note = "";
    [Header("Scaling")]
    public AnimationCurve DamageScalingCurve;
    public AnimationCurve RateScalingCurve;

    [Header("Chip Icon")]
    public Sprite ChipIcon;

    [Tooltip("The sound effect to play when the chip's associated bullet fires")]
    [Header("Fire Sound Effect")]
    public AudioIdentifiers FireSound;


    /// <summary>
    /// Calculate the damage boost based on the Chip's tier    
    /// <param name="tier">The tier of the Chip (an integer)<param>
    /// <return>Return the damage boost for a Chip of this type and tier</return>
    /// </summary>
    public float GetDamage(int tier)
    {
        return (DamageScalingCurve.Evaluate(tier));
    }


    /// <summary>
    /// </summary>
    public float GetRate(int tier)
    {
        return  RateScalingCurve.Evaluate(tier);
    }

    /// <summary>
    /// Calculate the fire rate change based on the Chip's tier
    /// <param name="tier">The tier of the Chip (an integer)<param>
    /// <return>Return the fire rate change for a Chip of this type and tier</return>
    /// </summary>
    public float GetRateChange(int tier)
    {
        Debug.Log("Get Rate Change = " + RateScalingCurve.Evaluate(tier));
        return RateScalingCurve.Evaluate(tier);
    }

    // TODO: Any other stats should be calculated in the same manner
    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// This component is added to the enemies
/// to apply a movementspeed status effect OnStart()
/// and remove it when the component is removed
/// </summary>

public class IceSlow : StatusEffect
{
    [Header("Slowed By (Intensity)%")]
    public float Intensity;

    /// <summary>
    /// When the bullet collides with an enemy, it sets
    /// this components duration and intensity.
    /// </summary>
    /// <param name="duration"></param>
    /// <param name="intensity"></param>
    public void SetData(float duration, float intensity)
    {
        this.Duration = duration;
        this.Intensity = intensity;
    }

    /// <summary>
    /// Status effect that is applied on Start()
    /// </summary>
    public override void ApplyStatusEffect()
    {
        GetComponentInParent<BaseEnemy>().StatusEffect = BaseEnemy.StatusEffects.Slowed;
        // Reduce speed
        GetComponent<IStatusEffect>().ChangeMovespeedStatus(-Intensity);
        foreach (MeshRenderer renderer in gameObject.GetComponentsInChildren<MeshRenderer>())
        {
            if (!renderer.gameObject.GetComponent<StaticMaterialTag>())
            {
                renderer.material = GetComponentInParent<BaseEnemy>().BaseEnemySettings.FrozenEnemyMat;
            }
        }
    }

    /// <summary>
    /// Called when the script is removed from the enemy
    /// </summary>
    public override void RemoveStatusEffect()
    {
        if (gameObject.activeSelf)
        {
            // Add speed back
            float activeSlowIntensity = 0;
            IceSlow[] slows = GetComponents<IceSlow>();
            foreach(IceSlow slowEffect in slows)
            {
                activeSlowIntensity += slowEffect.Intensity;
            }

            GetComponent<IStatusEffect>().ResetMoveSpeed();
            GetComponent<IStatusEffect>().ChangeMovespeedStatus(-activeSlowIntensity);

            if (ShouldChangeMaterial(slows)) // If all slows on the target are less then .15sec about to expire
            {
                foreach (MeshRenderer renderer in gameObject.GetComponentsInChildren<MeshRenderer>())
                {
                    if (!renderer.gameObject.GetComponent<StaticMaterialTag>())
                    {
                        renderer.material = GetComponent<BaseEnemy>().BaseEnemySettings.NormalEnemyMat;
                    }
                }
                GetComponent<BaseEnemy>().StatusEffect = BaseEnemy.StatusEffects.None;
                GetComponent<IStatusEffect>().ResetMoveSpeed();
            }
        }
    }

    /// <summary>
    /// If an enemy has an active slow of more then a small buffer of .15sec, then change back to normal material
    /// </summary>
    /// <param name="slows"></param>
    /// <returns></returns>
    public bool ShouldChangeMaterial(IceSlow[] slows)
    {
        foreach (IceSlow s in slows)
        {
            if (s.Duration >= .15f)
            {
                return false;
            }
        }
        return true;
    }
}
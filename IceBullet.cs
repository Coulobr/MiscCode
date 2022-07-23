using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles behaviors with the ice bullet
/// </summary>
public class IceBullet : BulletBehavior
{
    [Tooltip("The ice effect object to spawn")]
    public GameObject IceEffect;

    /// <summary>
    /// Applies a slow to the enemy gameobject for duration & intensity (%)
    /// If a slow already exists then reset the duration
    /// Intensity and duration are controled via the Animation Curve in the 
    /// BulletScaling Scriptable Object
    /// </summary>
    /// <param name="enemy"></param>
    /// <param name="duration"> Slow time in seconds </param>
    /// <param name="intensity"> Slow amount as a % </param>
    public override void CollisionBehavior(GameObject CollisionTarget)
    {
        Instantiate(IceEffect, (transform.position + CollisionTarget.transform.position)/2, Quaternion.identity);

        CollisionTarget.GetComponentInParent<IDamageable>().TakeDamage(Damage);
        IceSlow[] slows = CollisionTarget.transform.parent.gameObject.GetComponents<IceSlow>();
        if (slows.Length < 3)
        {
            Debug.Log("add slow");
            IceSlow slow = CollisionTarget.transform.parent.gameObject.AddComponent(typeof(IceSlow)) as IceSlow;
            slow.SetData(BulletStats.Instance.SlowDuration.Evaluate(CurrentBulletTier), BulletStats.Instance.SlowIntensity.Evaluate(CurrentBulletTier));
            return;
        }
    }
}

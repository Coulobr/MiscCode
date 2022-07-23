using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Base class for all status effects.
/// Effects could include slows, stuns, or other 
/// types of crowd control. These unique status
/// effects are created in sub-components. See IceSlow.cs
/// for an example of this inheritance
/// </summary>

public abstract class StatusEffect : MonoBehaviour
{
    [Tooltip("The remaining duration of the status effect")]
    public float Duration;
    public abstract void ApplyStatusEffect();
    public abstract void RemoveStatusEffect();
    private void Start()
    {
        if (transform.parent != null)
        {
            ApplyStatusEffect();
        }
    }
    private void OnDisable()
    {
        StopAllCoroutines();
        if (transform.parent != null)
        {
            RemoveStatusEffect();
        }
    }
    private void OnDestroy()
    {
        StopAllCoroutines();

        if (transform.parent != null)
        {
            RemoveStatusEffect();
        }
    }

    private void Update()
    {
        Duration -= Time.deltaTime;
        if (Duration <= 0)
        {
            RemoveStatusEffect();
            Destroy(this);
        }
    }
}

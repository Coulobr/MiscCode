using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A simple script for a VFX object which destroys itself when its partle effect finishes
/// </summary>

public class ParticleDecay : MonoBehaviour
{
    private ParticleSystem ps;
    private float effectDuration;
   
    void Awake()
    {
        ps = GetComponent<ParticleSystem>();
        effectDuration = ps.main.duration;
    }

    /// <summary>
    /// Decay on a timer
    /// </summary>
    void Update()
    {
        effectDuration -= Time.deltaTime;
        if (effectDuration <= 0)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Set the layer of this and all children
    /// </summary>
    /// <param name="layerName">The name of the layer to set this effect to</param>
    public void SetLayer(string layerName)
    {
        var newLayer = LayerMask.NameToLayer(layerName);
        gameObject.layer = newLayer;
        foreach(Transform child in transform)
        {
            child.gameObject.layer = newLayer;
        }
    }
}

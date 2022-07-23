using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Normal Enemy type behavior
/// All data and virtual functons are found in the BaseEnemy script
/// </summary>

public class NormalEnemy : BaseEnemy
{
    public override void SetBaseStats()
    {
        base.SetBaseStats();
    }
    public override void TakeDamage(float amount)
    {
        base.TakeDamage(amount);
    }

    /// <summary>
    /// Applies the default material to all non-static tagged objects
    /// Add the StaticMaterial comonent to the gameobject that has a 
    /// material you dont want to change
    /// </summary>
    public override void ApplyDefaultMaterial()
    {
        foreach (MeshRenderer renderer in meshRenderers)
        {
            if (!renderer.gameObject.GetComponent<StaticMaterialTag>())
            {
                renderer.material = BaseEnemySettings.NormalEnemyMat;
            }
        }
    }
}

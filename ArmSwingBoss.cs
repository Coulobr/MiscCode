using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// All behavior relating to taking damage and movement logic is
/// in the BaseEnemy class. To change the stats for this enemy, search for
/// the TempBossData ScriptableObject.
/// </summary>
public class ArmSwingBoss : BaseEnemy
{
    // 50% slow resistance
    public const float SLOW_RESISTANCE = .75f; 
    public override void ChangeMovespeedStatus(float percentage)
    {
        float adjustedSlow = 0f;
        if(percentage < 0) // If a slow is applied
        {
            adjustedSlow = percentage * SLOW_RESISTANCE;
            base.ChangeMovespeedStatus(adjustedSlow);
        }
        else
        {
            base.ChangeMovespeedStatus(percentage);
        }
    }

    public override void ApplyDefaultMaterial()
    {
        foreach (MeshRenderer renderer in meshRenderers)
        {
            if (!renderer.gameObject.GetComponent<StaticMaterialTag>())
            {
                // Assigns this bosses material to the dault material in this enemies TempBossData that is inherited from BaseEnemySettings - 
                renderer.material = BaseEnemySettings.NormalEnemyMat;
            }
            renderer.material.SetFloat("_DissolveAmount", 0);
        }
    }
}

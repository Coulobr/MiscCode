using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.Collections;

[CreateAssetMenu(menuName = "ScriptableObjects/HintTracking")]
public class HintTracking : SingletonScriptableObject<HintTracking>
{
    [SerializeField]
    [ReadOnly] private bool mergeHintCompleted;
    public bool MergeHintCompleted
    {
        get { return mergeHintCompleted;  }
        set { mergeHintCompleted = value; }
    }

    [SerializeField]
    [ReadOnly] private bool socketTowersHintCompleted;
    public bool SocketTowersHintCompleted
    {
        get { return socketTowersHintCompleted; }
        set { socketTowersHintCompleted = value; }
    }

    [SerializeField]
    [ReadOnly] private bool openBoxHintCompleted;
    public bool OpenBoxHintCompleted
    {
        get { return openBoxHintCompleted; }
        set { openBoxHintCompleted = value; }
    }

    [SerializeField]
    [ReadOnly] private bool bossInfoHintCompleted;
    public bool BossInfoHintCompleted
    {
        get { return bossInfoHintCompleted; }
        set { bossInfoHintCompleted = value; }
    }

    [SerializeField]
    [ReadOnly] private bool previewSlotHintCompleted;
    public bool PreviewSlotHintCompleted
    {
        get { return previewSlotHintCompleted; }
        set { previewSlotHintCompleted = value; }
    }

    [SerializeField]
    [ReadOnly] private bool introHintCompleted;
    public bool IntroHintCompleted
    {
        get { return introHintCompleted; }
        set { introHintCompleted = value; }
    }


    /// <summary>
    /// Resets all bools back to false
    /// </summary>
    public void ResetData()
    {
        this.mergeHintCompleted = false;
        this.socketTowersHintCompleted = false;
        this.bossInfoHintCompleted = false;
        this.bossInfoHintCompleted = false;
        this.previewSlotHintCompleted = false;
        this.IntroHintCompleted = false;    
    }

}

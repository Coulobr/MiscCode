using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/BossSpawnData")]
public class BossSpawnData : SingletonScriptableObject<BossSpawnData>
{
    [Header("Should boss spawning be enabled for gameplay?")]
    public bool EnableSpawning;
    [Header("(ReadOnly) Spawning Stats - These are saved")]
    public  int bossIndexToSpawn = 0;
    public bool randomlySpawnBosses = false;

    //Testing saving issues
    public bool testBossVariable = false;

    public int BossIndexToSpawn
    {
        get { return bossIndexToSpawn; }
        set { bossIndexToSpawn = value; }
    }

    public bool RandomlySpawnBosses
    {
        get { return randomlySpawnBosses; }
        set { randomlySpawnBosses = value; }
    }

    public bool ShouldSpawnBoss
    {
        get { return CanSpawnBoss(); }
    }

    public int BossSpawnInterval
    {
        get { return 10; }
    }

    /// <summary>
    /// Resets this scriptable obejct back to defaults ( you choose the defaults here )
    /// </summary>
    public void ResetData()
    {
        bossIndexToSpawn = 0;
        randomlySpawnBosses = false;
    }

    /// <summary>
    /// Should a boss spawn
    /// </summary>
    /// <returns></returns>
    public bool CanSpawnBoss()
    {
        //If spawing is disabled
        if(!EnableSpawning) { return false; }

        // Round 5 we manually spawn a boss for design reasons
        if (PlayerStats.Instance.CurrentWave == 5)
        {
            //Spawn Boss event
            Debug.Log("Spawning boss this wave");
            return true;
        }

        // is the current wave a multiple of the interval
        if (PlayerStats.Instance.CurrentWave % BossSpawnInterval == 0) 
        {
            //Spawn Boss event
            Debug.Log("Spawning boss this wave");
            return true;
        }
        // is the next wave a multiple of the interval
        else if ( (PlayerStats.Instance.CurrentWave + 1) % BossSpawnInterval == 0)
        {
            //Incoming boss warning
            Debug.Log("Boss incoming next wave");
            return false;
        }
        else
        {
            return false;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The wave asset maps a self-authored WaveData asset to
/// a specific level in which it is spawned
/// </summary>
[System.Serializable]
public struct WaveAsset
{
    public int WaveNumber;
    public WaveData WaveData;
}

/// <summary>
/// holds a list of all self authored maps
/// if the current level exceeds this list size
/// the game will continue with procedurally
/// generate waves with weighted chunks
/// </summary>
[CreateAssetMenu(menuName = "ScriptableObjects/New Wave Map")]
public class WaveLevelMap : ScriptableObject
{
    [Header("Delay After Wave Completion & Failure")]
    public float Delay;

    [Header("Assign Level Numbers to Waves")]
    public List<WaveAsset> WaveMappingList;

    private Dictionary<int, WaveData> levelLookupMap;
    public Dictionary<int, WaveData> LevelLookupMap {
        get { return levelLookupMap; }
        private set { levelLookupMap = value; }
    }

    public List<WaveData> BossWaveList;

    private void OnEnable()
    {
        LevelLookupMap = new Dictionary<int, WaveData>();
        levelLookupMap.Clear();
        foreach (WaveAsset wave in WaveMappingList)
        {
            LevelLookupMap.Add(wave.WaveNumber, wave.WaveData);
        }
    }
    private void OnDisable()
    {
        LevelLookupMap.Clear();
    }

}

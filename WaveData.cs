using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Struct to hold the chunks
/// </summary>
[System.Serializable]
public struct Chunk
{
    public ChunkData ChunkData;
    public float DelayAfterChunk;
}

/// <summary>
/// Struct to hold a copy of the active 
/// waves chip drops, assigned in LaunchWave()
/// </summary>
[System.Serializable]
public struct ChipsToDropStruct
{
    public List<ChipBase> chips;
}

/// <summary>
/// Holds data that the spawning manager uses to spawn waves of enemies
/// </summary>

[CreateAssetMenu(menuName = "ScriptableObjects/New Wave")]
public class WaveData : ScriptableObject
{
    [Header("Which chips will drop from enemies this wave")]
    public ChipsToDropStruct chipsToDrop;
    public ChipsToDropStruct ChipsToDrop {
        get { return chipsToDrop; }
    }

    [Tooltip("The chunks that will spawn this wave")]
    public List<Chunk> Chunks;

}

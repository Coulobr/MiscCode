using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds a list of enemies to spawn and an interval 
/// at which to spawn them. 
/// Also contains a difficulty weight for procedural generation. Refer
/// to other waves to compare the assumed difficulty. The weight can always be changed later
/// </summary>

[CreateAssetMenu(menuName = "ScriptableObjects/New Chunk")]
public class ChunkData : ScriptableObject
{
    [Tooltip("Difficulty of the chunk represented by an int")]
    public int DifficultyWeight;
    [Tooltip("Delay between individual enemy spawns")]
    public float EnemySpawnInterval;
    [Header("List of enemies to spawn (in order)")]
    public List<BaseEnemyData> EnemiesToSpawn;
}

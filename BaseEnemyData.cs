using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds the base stats for enemies. This is a root script
/// for the various enemy types
/// </summary>

public class BaseEnemyData : ScriptableObject
{
    [Header("Is this a boss? Needed for death rewards")]
    public bool IsBossEnemy;
    [Header("Base stats the enemies spawn with")]
    [Tooltip("The starting HP for the enemies when they spawn")]
    public float BaseHealth;
    [Tooltip("The startng MS of the enemies when they spawn")]
    public float BaseMovespeed;
    [Header("Reference to this enemies prefab for pooling")]
    [Tooltip("Referencing the gameobject of the enemy (for spawning)")]
    public GameObject EnemyPrefab;
    [Header("Economy & Player Progression")]
    [Tooltip("Amount of currency to add to stats")]
    [SerializeField]
    private float baseMoneyOnKill;
    public float MoneyOnKill
    {
        get { return Mathf.RoundToInt(baseMoneyOnKill * (1 + moneyRewardScaling.Evaluate(PlayerStats.Instance.CurrentWave))); }
        set { baseMoneyOnKill = value; }
    }
    [Tooltip("Amount of EXP to add to stats")]
    public int ExpOnKill;

    [Header("Materials to reference and change enemies appearance")]
    public Material NormalEnemyMat;
    public Material FrozenEnemyMat;

    [Header("Scaling")]
    public AnimationCurve hpScaling;
    public AnimationCurve moneyRewardScaling;
}

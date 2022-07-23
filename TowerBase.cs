using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stores the basic data shared for all Towers
/// </summary>

[CreateAssetMenu(fileName = "NewTowerBase", menuName = "ScriptableObjects/TowerBase", order = 1)]
public class TowerBase : ScriptableObject
{
    [Tooltip("The base damage of all Towers")]
    public float BaseDamage;
    [Tooltip("The base range of all Towers")]
    public float BaseRange;
    [Tooltip("The base fire rate of all Towers")]
    public float BaseFireRate;
    [Tooltip("The base slot count of all Towers")]
    public int BaseSlotCount;
    [Tooltip("Maps turret attributes to bullet materials")]
    public Dictionary<Attributes, Sprite> BulletMaterial;
    [Tooltip("The set of attributes provided by the Tower's Chips")]
    public Dictionary<Attributes, Material> MaterialMap = new Dictionary<Attributes, Material>(7);

    [Header("Material Settings : Map Attributes to Colors")]
    public List<NewMaterial> MaterialSettings;

    private void OnEnable()
    {
        //Fill material map
        FillMaterialMap();
    }

    /// <summary>
    /// Maps mateirals to attribute types for colored glow
    /// SET IN INSPECTOR
    /// </summary>
    public void FillMaterialMap()
    {
        foreach(NewMaterial matStruct in MaterialSettings)
        {
            MaterialMap.Add(matStruct.Attribuute, matStruct.Material);
        }
    }
}

[System.Serializable]
public struct NewMaterial
{
    public string name;
    public Material Material;
    public Attributes Attribuute;
}
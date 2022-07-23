using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stores the data of a Tower instance and controls its behavior
/// </summary>

public class TowerBehavior : MonoBehaviour
{
    [TextArea(3, 3)]
    public string note = " Holds data for socketed chips, bullet pooling, and targeting";

    // References
    [Header("References")]
    [Tooltip("A reference to the shared Tower parameters")]
    public TowerBase TowerBaseRef;
    [Tooltip("The set of TowerWeapons assigned as children of this object")]
    public TowerWeaponBehavior[] TowerWeapons;

    [Header("Bullet Pooling")]
    public List<BulletPoolable> PoolingOptions;
    public Dictionary<Attributes, GameObject> PoolContainers = new Dictionary<Attributes, GameObject>();
    [Tooltip("The basic empty object that is instantiated to construct pools")]
    public GameObject EmptyObject;

    // Stats
    [Header("Generic Stats")]
    [Tooltip("The range at which enemies can be targeted")]
    public float Range;
    [Tooltip("Adjusts the speed to rotate to targets")]
    public float RotationSpeed;
    [Tooltip("If tower is purchasable or not")]
    public bool purchasable = false;

    // Targetting
    [Header("Targeting")]
    [Tooltip("Prioritize non-frozen targets")]
    public bool optimalIceTargeting;
    [Tooltip("The list of enemies currently targetted by the Tower")]
    public List<GameObject> Targets;
    [Tooltip("The enemy currently being targetted by the Tower")]
    public GameObject CurrentTarget;
    [Tooltip("The left arm")]
    public GameObject ArmL;
    [Tooltip("The right arm")]
    public GameObject ArmR;

    #region Private Vars
    [Tooltip("The list of Chips attached to the Tower")]
    private ChipInstance chipLeft = null;
    private ChipInstance chipRight = null;
    [Tooltip("The set of attributes provided by the Tower's Chips")]
    private List<Attributes> attributesList = new List<Attributes>();

    [Tooltip("The selected tower indicator")]
    private SpriteRenderer selectIndicator;
    [Tooltip("Empty GO for all the bullet pools")]
    private Transform bulletPool;
    [Tooltip("For Rotations")]
    private GameObject towerModel;
    private Quaternion targetRot;
    #endregion

    #region Properties
    /// <summary>
    /// Holds the data for the chipset list
    /// </summary>
    public List<ChipInstance> ChipSet { get { return new List<ChipInstance>() {chipLeft, chipRight}; } }
    #endregion

    // =======
    //  SETUP
    // =======
    #region Setup

    private void OnEnable()
    {
        SelectedTower.Instance.OnTowerSelect += ToggleTowerSelect;
    }
    private void OnDisable()
    {
        SelectedTower.Instance.OnTowerSelect -= ToggleTowerSelect;
    }
    private void Awake()
    {
        // Get select ring from Range object child
        selectIndicator = gameObject.GetComponentInChildren<SpriteRenderer>();
        // Get weapons from arm object children
        TowerWeapons = gameObject.GetComponentsInChildren<TowerWeaponBehavior>();
        
        // Set up pooling container
        bulletPool = GetComponentInChildren<BulletPoolTag>().gameObject.transform;

        // Set up target for rotations and other movements/animations
        towerModel = GetComponentInChildren<TowerModelTag>().gameObject;

        //disables model render if purchasable
        ChangeModelVisibility(!purchasable);
    }
    void Start()
    {
        CurrentTarget = null;
        // Start looking downwards
        targetRot = Quaternion.LookRotation(Vector3.back);
        towerModel.transform.rotation = Quaternion.RotateTowards(towerModel.transform.rotation, targetRot, Time.deltaTime * 35);

        // Set up Bullet pool
        StartCoroutine(FillPoolsRoutine());

        // Apply initial stats
        UpdateStats();

        //Apply material
        UpdateMaterial();
    }
    #endregion

    #region Pooling
    
    /// <summary>
    /// At the beginning of the game the pools are
    /// created and bullet types are mapped to their proper
    /// container
    /// </summary>
    public IEnumerator FillPoolsRoutine()
    {
        foreach (BulletPoolable pool in PoolingOptions)
        {
            GameObject ObjContainer = Instantiate(EmptyObject, bulletPool);
            ObjContainer.name = pool.BulletType.ToString() + "Pool";
            for (int i = 0; i < pool.PoolSize; i++)
            {
                GameObject bullet = Instantiate(pool.BulletPrefab, ObjContainer.transform);
                bullet.SetActive(false);
            }
            PoolContainers.Add(pool.BulletType, ObjContainer);
            yield return new WaitForEndOfFrame();
        }
        StopCoroutine(FillPoolsRoutine());
    }

    /// <summary>
    /// This method searches through the available pools for the 
    /// container that is equal to bulletToSpawn and activates 
    /// the first disabled bullet in that pool
    /// </summary>
    /// <param name="bulletToSpawn"></param>
    /// <returns>The bullet selected</returns>
    public GameObject SpawnBullet(Attributes bulletToSpawn)
    {
        foreach (BulletPoolable pool in PoolingOptions)
        {
            if (pool.BulletType == bulletToSpawn)
            {
                PoolContainers.TryGetValue(pool.BulletType, out GameObject container);
                for (int i = 0; i < container.transform.childCount; i++)
                {
                    if (!container.transform.GetChild(i).gameObject.activeSelf)
                    {
                        return container.transform.GetChild(i).gameObject;
                    }
                }
                // -- If all are active spawn more -- \\
                GameObject bullet = Instantiate(pool.BulletPrefab, container.transform);
                return bullet;
            }
        }
        return null;
    }

    #endregion


    #region Material Swapping

    public void UpdateMaterial()
    {
        Attributes attributeCombo = new Attributes();
        Material matToApply;
        switch (attributesList.Count)
        {
            case 0:
                TowerBaseRef.MaterialMap.TryGetValue(Attributes.Normal, out matToApply);
                ChangeMaterial(matToApply);
                break;
            case 1:
                TowerBaseRef.MaterialMap.TryGetValue(attributesList[0], out matToApply);
                ChangeMaterial(matToApply);
                break;
            case 2:
                attributeCombo = attributesList[0] | attributesList[1];
                TowerBaseRef.MaterialMap.TryGetValue(attributeCombo, out matToApply);
                ChangeMaterial(matToApply);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Change material of children that don't
    /// </summary>
    public void ChangeMaterial(Material matToApply)
    {
        MeshRenderer[] m_renderers = towerModel.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer m_renderer in m_renderers)
        {
            if (m_renderer.GetComponent<StaticMaterialTag>() == null)
            {
                m_renderer.material = matToApply;
            }
        }
    }

    #endregion
    // ===========
    //  ATTACKING
    // ===========
    #region Attacking

    /// <summary>
    /// Update targets and begin attacks
    /// </summary>
    void Update()
    {
        //Only Update targets if tower is not purchasable
        if(!purchasable)
        {
            UpdateTarget();
        }
        if (CurrentTarget == null)
        {
            targetRot = Quaternion.LookRotation(Vector3.back);
            towerModel.transform.rotation = Quaternion.RotateTowards(towerModel.transform.rotation, targetRot, Time.deltaTime * 100);
        }

        Vector3 targetPosition = CurrentTarget == null ? towerModel.transform.position + Vector3.back : CurrentTarget.transform.position;
        Vector3 targetDirection = (new Vector3(targetPosition.x, towerModel.transform.position.y, targetPosition.z) - towerModel.transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        towerModel.transform.rotation = Quaternion.RotateTowards(towerModel.transform.rotation, targetRotation, Time.deltaTime * RotationSpeed);
    }

    /// <summary>
    /// Update current target to the furthest enemy in range
    /// </summary>
    private void UpdateTarget()
    {
        // if the currently targeted enemy is ever deactivated, stop targetting it
        if (CurrentTarget != null && !CurrentTarget.activeInHierarchy)
        {
            CurrentTarget = null;
        }
        // filter out deactivated targets
        for (int i = Targets.Count - 1; i >= 0; i--)
        {
            GameObject target = Targets[i];

            if (!target.activeInHierarchy || !target.GetComponent<Collider>().enabled || target.GetComponentInParent<BaseEnemy>().IsDying())
            {
                RemoveTarget(target);
            }
        }
        // automatically target enemy furthest along path
        if (Targets.Count > 0)
        {
            // Towers will focus on the first target if none others present/valid
            float furthestDistance = 0f;
            int furthestIndex = 0;
            for (int i = 0; i < Targets.Count; i++)
            {
                float dist = Targets[i].GetComponentInParent<BaseEnemy>().GetDistance();

                if (dist > furthestDistance)
                {
                    // Ice towers prioritize non-frozen targets
                    if (optimalIceTargeting && !attributesList.Contains(Attributes.Ice) || !Targets[i].GetComponentInParent<IceSlow>())
                    {
                        furthestDistance = dist;
                        furthestIndex = i;
                    }
                    else {
                        furthestDistance = dist;
                        furthestIndex = i;
                    }

                    // TODO: maybe revise this system to make individual weapons target separately
                }     
            }
            CurrentTarget = Targets[furthestIndex];
            // TODO: Stress test this!
        }
        else
        {
            CurrentTarget = null;
        }
    }

    /// <summary>
    /// Remove an Enemy from the list of Targets
    /// </summary>
    /// <param name="enemy">The Enemy to remove from the list of Targets</param>
    /// <returns>Returns whether the Enemy was successfully removed</returns>
    public bool RemoveTarget(GameObject enemy)
    {
        if (enemy == CurrentTarget)
        {
            CurrentTarget = null;
        }
        if (Targets.Contains(enemy))
        {
            Targets.Remove(enemy);
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Reset all targets
    /// </summary>
    public void ResetTargets()
    {
        CurrentTarget = null;
        Targets.Clear();
    }

    /// <summary>
    /// Detect nearby Enemies
    /// </summary>
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Targets.Add(other.gameObject);
        }
    }

    /// <summary>
    /// Ignore enemies that move out of range
    /// </summary>
    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            RemoveTarget(other.gameObject);
        }
    }

    #endregion

    // =======
    //  STATS
    // =======
    #region Stats

    /// <summary>
    /// Updated the Tower's stats based on its Chip set
    /// AS WELL AS ITS MATERIAL
    /// </summary>
    public void UpdateStats()
    {
        // reset stats before updating
        Range = TowerBaseRef.BaseRange;
        attributesList.Clear();

        // apply weapon attributes
        foreach (TowerWeaponBehavior weapon in TowerWeapons)
        {
            // update attribute set
            Attributes weaponAttribute = weapon.BulletAttribute();
            if (!attributesList.Contains(weaponAttribute))
            {
                attributesList.Add(weaponAttribute);
            }
        }

        // Changes its color depending on the updated attribute list.
        UpdateMaterial();
    }

    /// <summary>
    /// Adds the given Chip to the Tower, if able
    /// </summary>
    /// <param name="chip">The Chip to add</param>
    /// <param name="slot">The slot index to add the chip to (-1 for automatic)</param>
    /// <returns>Returns whether successful</returns>
    public bool AddChip(ChipInstance chip, int slot)
    {
        // fail if chip slot invalid
        if (slot > TowerWeapons.Length)
        {
            return false;
        }
        // if passed chip is null, the chip itself hasn't moved but its tier and stats may need updating
        if (chip == null)
        {
            TowerWeapons[slot].UpdateWeaponStats();
            UpdateStats();
            return false;
        }

        // if specific slot declared, assign chip to it
        if (slot >= 0)
        {
            // attach if empty
            if (!TowerWeapons[slot].HasChip())
            {
                ReplaceChip(TowerWeapons[slot], chip);
                AudioManager.Instance.PlayAudio(AudioIdentifiers.ChipDrop);
                return true;
            }
            // merge if full and able
            else
            {
                if (InventoryData.CanMerge(chip, TowerWeapons[slot].Chip))
                {
                    UpgradeWeaponChip(TowerWeapons[slot]);
                    AudioManager.Instance.PlayAudio(AudioIdentifiers.ChipMerge);
                    return true;
                }
            }
            return false;
        }
        // if no slot specified, add to first available slot or merge
        else
        {
            // place in open slot if any exist
            foreach (TowerWeaponBehavior weapon in TowerWeapons)
            {
                if (!weapon.HasChip())
                {
                    ReplaceChip(weapon, chip);
                    AudioManager.Instance.PlayAudio(AudioIdentifiers.ChipDrop);
                    return true;
                }
            }
            // merge if no open slots
            foreach (TowerWeaponBehavior weapon in TowerWeapons)
            {
                if (InventoryData.CanMerge(chip, weapon.Chip))
                {
                    UpgradeWeaponChip(weapon);
                    AudioManager.Instance.PlayAudio(AudioIdentifiers.ChipMerge);
                    return true;
                }
            }
            return false;
        }
    }

    /// <summary>
    /// Attaches the chip to the weapon
    /// </summary>
    /// <param name="weapon">The Weapon to modify</param>
    /// <param name="chip">The Chip to add</param>
    public void ReplaceChip(TowerWeaponBehavior weapon, ChipInstance chip)
    {
        weapon.Chip = chip;
        weapon.UpdateWeaponStats();
        UpdateStats();
    }

    /// <summary>
    /// Upgrades the chip attached to the weapon
    /// </summary>
    /// <param name="weapon">The Weapon to modify</param>
    private void UpgradeWeaponChip(TowerWeaponBehavior weapon)
    {
        weapon.Chip.UpgradeTier();
        weapon.UpdateWeaponStats();
        UpdateStats();
    }

    /// <summary>
    /// Removes the given Chip from the Tower, if able
    /// </summary>
    /// <param name="chip">The Chip to add</param>
    /// <param name="slot">The slot index to add the chip to</param>
    /// <returns>Returns whether successful</returns>
    public bool RemoveChip(ChipInstance chip, int slot)
    {
        // fail if slot index out of range
        if (slot < 0 || slot > TowerWeapons.Length)
        {
            Debug.Log("ERROR: ATTEMPTED TO REMOVE CHIP FROM INVALID SLOT " + slot);
            return false;
        }
        // remove chip if match found
        if (TowerWeapons[slot].Chip == chip)
        {
            TowerWeapons[slot].Chip = null;
            TowerWeapons[slot].UpdateWeaponStats();
            UpdateStats();
            return true;
        }
        else
        {
            Debug.Log("ERROR: NO MATCHING CHIP FOUND IN SLOT " + slot);
            return false;
        }
    }

    #endregion

    // ============
    // SELECT TOWER
    // ============
    #region Select Tower

    /// <summary>
    /// If selected tower matches self, select
    /// Otherwise, deselect
    /// </summary>
    public void ToggleTowerSelect(GameObject tower)
    {
        selectIndicator.enabled = (tower == this.gameObject);
    }     
    
    /// <summary>          
    /// Show/Hide select ring sprite renderer to indicate tower selected         
    /// </summary>
    public void SetSelectionState(bool state)
    {
        selectIndicator.enabled = state;
    }
    #endregion

    #region Purchasing

    /// <summary>
    /// Enables/Disables the tower model discluding the base platform
    /// </summary>
    /// <param name="visible"></param>
    private void ChangeModelVisibility(bool visible)
    {
        MeshRenderer[] model = towerModel.GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < model.Length; i++)
        {
            model[i].enabled = visible;
        }
    }
    
    /// <summary>
    /// Call when tower is purchased
    /// </summary>
    public void Purchase()
    {
        if(SelectedTower.Instance.CurrentTower == gameObject)
        {
            purchasable = false;
            ChangeModelVisibility(true);
        }
    }

    #endregion
}
[System.Serializable]
public struct BulletPoolable
{
    public GameObject BulletPrefab;
    public Attributes BulletType;
    public int PoolSize;
}
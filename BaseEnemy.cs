using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.SceneManagement;

/// <summary>
/// Holds Enemy behaviors for pathing and taking damage
/// This is the base class for all enemy types
/// </summary>

[RequireComponent(typeof(Rigidbody))]
public abstract class BaseEnemy : MonoBehaviour, IDamageable, IStatusEffect
{
    [Tooltip("Reference to base stats")]
    public BaseEnemyData BaseEnemySettings;
    [Tooltip("The health bar for this enemy")]
    public EnemyHealthbar Healthbar;

    [Tooltip("The status effects on this Enemy")]
    public StatusEffects StatusEffect;

    [SerializeField][ReadOnly] private float health;
    private float startingHealth;

    [Tooltip("The prefab to spawn to create this enemy's death effect")]
    public GameObject DeathEffectPrefab;

    [Tooltip("Set to true if the enemy has 0 hp and is in its death animation")]
    private bool isDeathAnim = false;

    [Tooltip("Chip to be dropped on death")]
    public List<ChipBase> ChipsToDrop = new List<ChipBase>();

    private const int RAINBOW_BOX_CHANCE = 5;
    #region Properties
    /// <summary>
    /// Holds current HP of the enemy
    /// starts the death routine 
    /// </summary>
    public float Health
    {
        get
        {
            return health;
        }
        private set
        {
            health = value;
            if (health <= 0 && gameObject.activeSelf)
            {
                health = 0;
                StartCoroutine(DeathBehavior(true));
            }
        }
    }
    public List<Transform> Waypoints { get { return waypoints; } }
    public int PathingIndex { get { return pathingIndex; } }
    public float Movespeed { get; private set; }
    #endregion

    #region Pathing Variables
    private Rigidbody thisRB;
    private List<Transform> waypoints;
    protected MeshRenderer[] meshRenderers;
    private int pathingIndex = 0;
    private float totalDistanceTraveled = 0;
    private float currentDistanceTraveled = 0;
    private bool canMove = false;
    #endregion

    #region Init
    private void Awake()
    {
        meshRenderers = GetComponentsInChildren<MeshRenderer>();
        waypoints = GetComponentInParent<SpawningManager>().PathingNodes;
        thisRB = GetComponent<Rigidbody>();
        SetBaseStats();
    }

    /// <summary>
    /// Called when the gameobject is enabled
    /// Sets the defaust base stats for that enemy type
    /// </summary>
    private void OnEnable()
    {
        // reset & scale hp
        SetBaseStats();

        // use base health on first wave and calculate health on subsequent waves
        Health = (Health * (1 + BaseEnemySettings.hpScaling.Evaluate(PlayerStats.Instance.CurrentWave))); 
        startingHealth = Health;
        if(SceneManager.GetActiveScene().name == "Map02")
        {
            Health = Health * 0.8f;
        }

        // set appearance
        RemoveAllStatusEffects();
        ApplyDefaultMaterial();

        // HP bar
        Healthbar.enabled = true;    
        Healthbar.ResetFillAmount();

        // Pathing
        totalDistanceTraveled = 0;
        currentDistanceTraveled = 0;
        pathingIndex = 0;
        canMove = true;
        thisRB.isKinematic = true;
        transform.localScale = Vector3.one;
        TurnToNextPoint();
    }

    public virtual void SetBaseStats()
    {
        Health = BaseEnemySettings.BaseHealth;
        Movespeed = BaseEnemySettings.BaseMovespeed;
        ChipsToDrop.Clear();
    }
    #endregion

    private void FixedUpdate()
    {
        if (canMove)
        {
            thisRB.MovePosition(transform.position + transform.forward * Time.fixedDeltaTime * Movespeed);
            if (pathingIndex > 0)
            {
                currentDistanceTraveled = Vector3.Distance(transform.position, waypoints[pathingIndex - 1].transform.position);
            }
            UpdateDistance();
        }
    }


    // =========
    //  PATHING
    // =========
    #region Pathing

    /// <summary>
    /// Handles the pathing to points and tracking the total distance traveled
    /// so the towers can properly target
    /// </summary>
    public void UpdateDistance()
    {
        // Reach Waypoint
        if (Vector3.Distance(transform.position, Waypoints[pathingIndex].position) < .025f)
        {

            // Updating distance traveled for targeting logic
            totalDistanceTraveled += currentDistanceTraveled;
            currentDistanceTraveled = 0f;

            // Update pathing target
            pathingIndex++;
            if (pathingIndex >= waypoints.Count)
            {
                // At end of path, declare wave failed
                EventManager.Instance.RaiseGameEvent(EventConstants.WAVE_DEFEAT);
                RepoolThisObject();
                return;
            }

            // Update rotation
            TurnToNextPoint();
        }
    }
    /// <summary>
    /// Returns the total distance travelled
    /// </summary>
    public float GetDistance()
    {
        return totalDistanceTraveled + currentDistanceTraveled;
    }

    /// <summary>
    /// Rotate to face the next path target
    /// </summary>
    private void TurnToNextPoint()
    {
        gameObject.transform.rotation = Quaternion.LookRotation(waypoints[pathingIndex].transform.position - transform.position, Vector3.up);
    }

    #endregion

    // ========
    //  COMBAT
    // ========
    #region Combat
    /// <summary>
    /// Lowers the enemy health and repools if it dies
    /// </summary>
    /// <param name="amount"></param>
    public virtual void TakeDamage(float amount)
    {
        Health -= amount;
        float scale = 1.5f;
        transform.localScale = Vector3.one;
        transform.DOScale(new Vector3(scale,scale,scale), 0.2f).From();
        Healthbar.SetFillAmount(Mathf.Max(0,Health/startingHealth));
    }

    /// <summary>
    /// Returns whether the enemy is in the dying animation
    /// </summary>
    public bool IsDying() { return isDeathAnim; }

    /// <summary>
    /// Kills the enemy, but doesnt  the OnDeath event
    /// </summary>
    public void GameRestartDeath()
    {
        canMove = false;
        StartCoroutine(DeathBehavior(false));
    }

    /// <summary>
    /// Called when the enemy reaches 0 HP or 
    /// if GameRestartDeath() is called manually.
    /// </summary>
    /// <param name="showDeath"> 
    /// Whether or not to decrease active enemies when this dies. 
    /// True for normal death from dmg and false for death from failing a wave
    /// </param>
    public IEnumerator DeathBehavior(bool showDeath)
    {
        // Add stats and start death animation if trying
        if (showDeath)
        {
            isDeathAnim = true;
            EventManager.Instance.RaiseGameEvent(EventConstants.ENEMY_DEATH);
            PlayerStats.Instance.AddCurrency(BaseEnemySettings.MoneyOnKill);
            PlayerStats.Instance.AddExp(BaseEnemySettings.ExpOnKill);
        }

        // Disable collider during death
        thisRB.GetComponentInChildren<Collider>().enabled = false;

        // Hide meshes during death
        foreach (MeshRenderer mesh in meshRenderers) { mesh.enabled = false; }
        Healthbar.ReEnableMeshes();
        canMove = false;

        // Death effect & drop chip
        if (showDeath)
        {
            //USE THIS WHEN BOSSES ARE IN - OTHERWISE BETTER BOXES ARE RANDOM
            if (BaseEnemySettings.IsBossEnemy)
            {
                var randChip = Random.Range(0, PlayerStats.Instance.NumChipTypesAvailable);
                var chipType = PlayerStats.Instance.AvailableChipTypes[randChip];
                ChipInstance newChip = new ChipInstance { ChipBaseRef = chipType, Tier = PlayerStats.Instance.ChipEconomy + 2 };
                InventoryData.Instance.AddNewChip(newChip, BoxState.Adbox);
            }
            else if (ChipsToDrop != null && ChipsToDrop.Count > 0) //regular enemy
            {
                for (int i = 0; i < ChipsToDrop.Count; i++)
                {
                    ChipInstance newChip = new ChipInstance { ChipBaseRef = ChipsToDrop[i], Tier = PlayerStats.Instance.ChipEconomy };
                    InventoryData.Instance.AddNewChip(newChip);
                }     
            }

            // Start death sequences
            DeathEffect deathEffect = Instantiate(DeathEffectPrefab, transform.position, Quaternion.identity).GetComponent<DeathEffect>();
            deathEffect.ExplodeSequence(ChipsToDrop.Count != 0 || BaseEnemySettings.IsBossEnemy).Play();
            yield return new WaitForSeconds(deathEffect.Duration);

            isDeathAnim = false;
        }

        // Repool
        RepoolThisObject();
        StopAllCoroutines();
        yield return null;
    }

    #endregion

    /// <summary>
    /// Disables the enemy and repools it by sending it back to the 
    /// containers origin (start) to be spawned again
    /// </summary>
    public void RepoolThisObject()
    {
        transform.position = transform.parent.position;
        Healthbar.ResetFillAmount();
        gameObject.SetActive(false);
        thisRB.GetComponentInChildren<Collider>().enabled = true;
        foreach (MeshRenderer mesh in meshRenderers) { mesh.enabled = true; }
        isDeathAnim = false;
        canMove = true;
        ChipsToDrop.Clear();

        RemoveAllStatusEffects();
        ApplyDefaultMaterial();

        StopAllCoroutines();
    }

    #region Status Effects
    /// Calls when enemy dies
    public abstract void ApplyDefaultMaterial();

    /// <summary>
    /// If a status effect applies a stun, itll call this method
    /// </summary>
    /// <param name="duration"></param>
    public void StunStatus(float duration)
    {
        //Stunned behavior here
    }

    /// <summary>
    /// Changes the movements speed by a percentage
    /// Decrease by 20% = ChangeMovespeed(-20)
    /// Increase by 30% = ChangeMovespeed(30);
    /// </summary>
    /// <param name="percentage"></param>
    public virtual void ChangeMovespeedStatus(float percentage)
    {
       Movespeed *= ((100f + percentage) / 100);
    }

    
    /// <summary>
    /// Try to remove every status effect in the game 
    /// Add more TryGetComponents as you add more status effects
    /// </summary>
    public void RemoveAllStatusEffects()
    {
        if (TryGetComponent(out IceSlow iceSlow))
        {
            Destroy(iceSlow);
        }

        StatusEffect = StatusEffects.None;
    }

    public void ResetMoveSpeed()
    {
        Movespeed = BaseEnemySettings.BaseMovespeed;
    }

    public void SetExactMovespeed(float speed)
    {
        Movespeed = speed;
    }

    /// <summary>
    /// As you add status effects, make sure you
    /// add fucnctionality to RemoveAllStatusEffects()
    /// </summary>
    public enum StatusEffects
    {
        None,
        Slowed,
        Stunned, //Unused
    }
    #endregion

}

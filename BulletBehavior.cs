using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using UnityEngine;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Abstract class that holds all bullet data
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public abstract class BulletBehavior : MonoBehaviour, IProjectile
{
    #region Properties
    public float Damage { get { return damage; } }
    public int CurrentBulletTier { get { return currentBulletTier; } }
    private int currentBulletTier;
    #endregion

    #region Private Vars | Set via InitilizeFlightData
    [Tooltip("The enemy this Bullet is targetting")]
    private GameObject initialTarget;
    private TowerBehavior tb;
    private TowerWeaponBehavior tw;
    private Rigidbody thisRb;
    private float range = 0;
    private float speed = 0;
    private float damage = 0;

    #endregion

    private void Awake()
    {
        thisRb = GetComponent<Rigidbody>();
        tb = transform.parent.GetComponentInParent<TowerBehavior>();
    }

    /// <summary>
    /// Travel towards target
    /// or travel forward if the target died
    /// </summary>
    void FixedUpdate()
    {
        var viewportPos = Camera.main.WorldToViewportPoint(transform.position);
        if (viewportPos.x > 1 || viewportPos.x < 0 || viewportPos.y > 1 || viewportPos.y < 0)
        {
            RePool();
        }
        else
        {
            thisRb.MovePosition(transform.position + transform.forward * speed * Time.deltaTime);
        }
    }

    /// <summary>
    /// Disable the object and return it to the pool
    /// </summary>
    protected void RePool()
    {
        gameObject.SetActive(false);
        gameObject.transform.position = Vector3.zero;
    }

    /// <summary>
    /// Once re-activated, update self to match current Bullet type and fire
    /// </summary>
    /// <param name="weapon">The weapon firing the bullet</param>
    public void InitializeFlightData(TowerWeaponBehavior weapon)
    {
        // -- Inital data settings -- \\
        initialTarget = tb.CurrentTarget.gameObject;
        range = tb.Range;
        speed = weapon.BulletSpeed;
        damage = weapon.Damage;
        currentBulletTier = weapon.CurrentBulletTier;
        transform.position = weapon.FirePosition.position;

        // -- Leading its target to always hit no matter the bullet & enemy velocity -- \\
        if (initialTarget != null)
        {
            float time = (initialTarget.transform.position - transform.position).magnitude / speed;
            Vector3 futurePos = initialTarget.transform.position + initialTarget.GetComponentInParent<Rigidbody>().velocity * time;

            // -- Rotate to the future enemy location and enable the object -- \\
            Quaternion targetRotation = Quaternion.identity;
            Vector3 targetDirection = (new Vector3(futurePos.x, transform.position.y, futurePos.z) - transform.position).normalized;
            targetRotation = Quaternion.LookRotation(targetDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 360f);
            gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// When a bullet collides with an enemy we
    /// call collision behavior. The bullet will then
    /// call its interface method & repool itself.
    /// </summary>
    public abstract void CollisionBehavior(GameObject CollisionTarget);
    void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case "Enemy":
                CollisionBehavior(other.gameObject);
                RePool();
                break;
            case "Structure":
                RePool();
                break;
            default:
                break;
        }
    }
}

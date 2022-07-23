using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Defines the behavior for a lightning bullet
/// </summary>

public class LightningBullet : BulletBehavior
{
    [Tooltip("The lightning effect object to spawn")]
    public GameObject LightningEffect;

    struct EnemyDistance
    {
        [Tooltip("The object reference to the enemy hit")]
        public GameObject HitEnemy;
        [Tooltip("The position of the enemy along the beam path, from 0 (impact) to 1 (end)")]
        public float HitPosition;

        public EnemyDistance(GameObject enemy, float pos)
        {
            HitEnemy = enemy;
            HitPosition = pos;
        }
    }


    /// <summary>
    /// Default taking damage implementation
    /// </summary>
    public override void CollisionBehavior(GameObject CollisionObject)
    {

        // Determine spread distance via AnimationCurve in BulletStats.cs
        float maxDistance = BulletStats.Instance.LightningSpread.Evaluate(CurrentBulletTier);
        
        List<EnemyDistance> EnemiesHit = new List<EnemyDistance>();
        EnemiesHit.Add(new EnemyDistance(CollisionObject, 0f));

        // load enemy data
        BaseEnemy targetEnemy = CollisionObject.GetComponentInParent<BaseEnemy>();
        List<Transform> pathPoints = targetEnemy.Waypoints;
        int enemyIndex = targetEnemy.PathingIndex;
        Vector3 enemyPosition = CollisionObject.transform.position;

        // check backwards along the path
        EnemiesHit = TraceAlongPath(-1, enemyPosition, Mathf.Max(0,enemyIndex-1), pathPoints, maxDistance, EnemiesHit);

        // check forwards along the path
        EnemiesHit = TraceAlongPath(1, enemyPosition, enemyIndex, pathPoints, maxDistance, EnemiesHit);

        // Hurt all detected enemies
        foreach(EnemyDistance enemyHit in EnemiesHit)
        {
            if (enemyHit.HitEnemy != null)
            {
                enemyHit.HitEnemy.GetComponentInParent<IDamageable>().TakeDamage(Damage);
            }
// ================
// TODO: Calculate damage in the above line based on HitEnemy.HitPosition, where HitPosition is a value from 0 (meaning a direct hit) to 1 (meaning at the end of the beam)
// (this may be wholly unnecessary)
// ================
        }
        //Debug.Log("Hit " + EnemiesHit.Count + " enemies!");
    }

    /// <summary>
    /// Scan along the map's path to detect enemies and create visual effect
    /// </summary>
    /// <param name="direction">int (+1 or -1), indicating which direction to move in (+ is towards end of path)</param>
    /// <param name="startPosition">Vector3, indicating the position to start pathing from</param>
    /// <param name="startIndex">int, indicating the path index to start pathing from</param>
    /// <param name="pathPoints">List of Transform, the vertices along the path to follow</param>
    /// <param name="maxDistance">float, the maximum distance to check along the path</param>
    /// <param name="knownEnemies">List of GameObject, the set of previously found enemies found along the path</param>
    /// <returns>The combined set of previously and newly-detected enemies</returns>
    private List<EnemyDistance> TraceAlongPath(int direction, Vector3 startPosition, int startIndex, List<Transform> pathPoints, float maxDistance, List<EnemyDistance> knownEnemies)
    {
        Vector3 currentPosition = startPosition;
        int checkIndex = startIndex;
        float travelDistance = 0;

        // maintain list of positions
        List<Vector3> hitPositions = new List<Vector3>();
        hitPositions.Add(currentPosition);
        
        // raycast along path until max distance reached, collecting references to the Enemies hit
        while (checkIndex >= 0 && checkIndex < pathPoints.Count && travelDistance < maxDistance)
        {
            // calculate target and distance
            Vector3 targetPosition = pathPoints[checkIndex].position;
            float checkDistance = Mathf.Min(Vector3.Distance(currentPosition,targetPosition), maxDistance-travelDistance);

            // update position list for VFX
            hitPositions.Add(currentPosition + Vector3.Normalize(targetPosition-currentPosition)*checkDistance);
            Debug.DrawRay(currentPosition, Vector3.Normalize(targetPosition-currentPosition)*checkDistance, Color.red,1);

            // perform raycast(s)
            RaycastHit[] castResults = Physics.RaycastAll(currentPosition,targetPosition-currentPosition, checkDistance, LayerMask.GetMask("Enemy"));
            foreach (RaycastHit castResult in castResults)
            {
                // record detected enemies and relative position along path
                EnemyDistance hitObject = new EnemyDistance(castResult.collider.gameObject, castResult.distance/maxDistance);
                if (!knownEnemies.Contains(hitObject))
                {
                    knownEnemies.Add(hitObject);
                    Debug.DrawLine(currentPosition, hitObject.HitEnemy.transform.position, Color.yellow,1);
                }
            }

            // advance along the path
            currentPosition = targetPosition;
            travelDistance += checkDistance;
            checkIndex += direction;
        }

        // Spawn VFX
        GameObject lightning = Instantiate(LightningEffect, startPosition, Quaternion.identity);
        lightning.GetComponent<LightningGroundEffect>().Setup(hitPositions);

        // Return the updated set of known enemies
        return knownEnemies;
    }
}

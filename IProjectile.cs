using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IProjectile 
{
    void CollisionBehavior(GameObject CollisionTarget);
    void InitializeFlightData(TowerWeaponBehavior weapon);
}

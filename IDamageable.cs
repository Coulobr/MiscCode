using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Objects that can take damage should implement this
/// </summary>

public interface IDamageable
{
      void TakeDamage(float amount);
     // void OnTriggerEnter(Collider other);
}

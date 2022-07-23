using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "Dialogue/New Character")]
public class Character : ScriptableObject
{
    public string FullName;
    public Sprite Portrait;
}

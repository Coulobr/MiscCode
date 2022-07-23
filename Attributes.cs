using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represent the different types of Chips and their associated effects
/// </summary>
[Flags]
public enum Attributes
{
    Normal = 0,
    Ice = 1,
    Plasma = 2,
    Lightning = 4,
};

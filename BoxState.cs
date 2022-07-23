using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Indicates the status of a chip box
/// </summary>

public enum BoxState
{
    None, // No box (revealed chip)
    Opening, // Showing the open animation
    Box, // Simple box (hidden chip)
    Adbox, // Adbox (hidden chip, open to recieve prompt to watch an ad to get a better chip)
}
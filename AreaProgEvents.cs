using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


/// <summary>
/// Empty singleton class that listens to events.
/// Its a singtleton because we don't want to call duplicate events
/// </summary>
public class AreaProgEvents : MonoBehaviour
{
    public static AreaProgEvents Instance = null;
    void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(this);
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}


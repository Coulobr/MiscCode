using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Rotates the obj to face the camera
/// </summary>
public class Billboard : MonoBehaviour
{
    private Camera mainCam;
    private void Awake() => mainCam = Camera.main;
    void Update() => transform.rotation = mainCam.transform.rotation;
}

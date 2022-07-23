using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Visual effect for the lightning ability
/// </summary>

public class LightningGroundEffect : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private List<Vector3> pathPoints;
    [Tooltip("The length of time the lightning effect should last for")]
    public float LightningDuration = 0.4f;
    [Tooltip("The offset to apply to the lightning effect's position")]
    public Vector3 PositionOffset = Vector3.zero;
    private bool activated = false;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false;
    }

    /// <summary>
    /// Remotely setup the lightning effect's path
    /// </summary>
    /// <param name="points">The list of points to draw the line along</param>
    public void Setup(List<Vector3> points)
    {
        lineRenderer.positionCount = points.Count;
        Vector3[] pointArray = points.ToArray();
        for (int i = 0; i < pointArray.Length; i++)
        {
            pointArray[i] += PositionOffset;
        }
        lineRenderer.SetPositions(pointArray);
        lineRenderer.enabled = true;
        activated = true;
    }
    
    // Update is called once per frame
    void Update()
    {
        if (activated)
        {
            LightningDuration -= Time.deltaTime;
            if (LightningDuration <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnemyHealthbar : MonoBehaviour
{
    [Tooltip("The duration of the health change effect")]
    public float DamageTime = 0.2f;
    [Tooltip("The duration of the healthbar vanish-on-death effect")]
    public float VanishTime = 0.3f;

    [Tooltip("Default x-scale of fill object")]
    const float DEFAULT_FILL_SCALE = 0.95f;

    [Tooltip("Reference to the camera to point at")]
    private Transform cam;
    [Tooltip("Reference to the healthbar fill")]
    private Transform healthbarFill;
    private Vector3 healthbarInitialPosition;
    private MeshRenderer[] meshes;

    // Start is called before the first frame update
    void Awake()
    {
        cam = Camera.main.transform;
        healthbarFill = transform.GetChild(0).GetChild(0);
        healthbarInitialPosition = healthbarFill.localPosition;
        meshes = GetComponentsInChildren<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        // Orient this thing so it's facing the camera
        transform.rotation = cam.rotation;
    }

    /// <summary>
    /// Set the percentage of health the healthbar should display
    /// </summary>
    /// <param name="amt">Percentage of health</param>
    public void SetFillAmount(float amt)
    {
        float targetScale = DEFAULT_FILL_SCALE * amt;
        healthbarFill.DOLocalMoveX(-(targetScale-transform.localScale.x)/2, DamageTime);
        healthbarFill.DOScaleX(targetScale, DamageTime);
        
        // if enemy defeated, hide health
        if (amt <= 0)
        {
            transform.DOScaleX(0,VanishTime).SetEase(Ease.InExpo);
        }
    }

    /// <summary>
    /// Reset appearance
    /// </summary>
    public void ResetFillAmount()
    {
        transform.localScale = Vector3.one;
        healthbarFill.localPosition = healthbarInitialPosition;
        healthbarFill.localScale = Vector3.one;
    }

    /// <summary>
    /// Re-enable all meshes
    /// </summary>
    public void ReEnableMeshes()
    {
         foreach(MeshRenderer mesh in meshes) { mesh.enabled = true; }
    }

}

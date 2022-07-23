using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class DeathEffect : MonoBehaviour
{
    [Header("Timing")]
    [Tooltip("Delay before dissolving")]
    public float DissolveDelay = 1.2f;
    [Tooltip("Duration of dissolve effect")]
    public float DissolveDuration = 1f;

    [Header("Explosion")]
    public float MinForce = 275f;
    public float MaxForce = 350f;
    public float Radius = 4f;

    public GameObject BoxRewardPopup;

    public float Duration {get {return DissolveDelay + DissolveDuration;}}

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Generates a Sequence which simulates the explosion and dissolving of an enemy
    /// </summary>
    /// <returns> A sequence which simulates the enemy exploding and dissolving </returns>
    public Sequence ExplodeSequence(bool popup)
    {
        Sequence sequence = DOTween.Sequence();

        sequence.AppendCallback(() => gameObject.SetActive(true));

        // explode the body
        sequence.AppendCallback(Explode);

        //spawn popup
        if (popup)
        {
            Instantiate(BoxRewardPopup, transform.position, Quaternion.identity);
            Debug.Log("Spawn popup");
        }
        // after a delay, dissolve the parts and despawn
        foreach (MeshRenderer renderer in GetComponentsInChildren<MeshRenderer>())
        {
            if (!renderer.GetComponent<StaticMaterialTag>())
            {
                float dissolveAmount = 0;

                Tween tween =
                    DOTween.To(() => dissolveAmount, x => dissolveAmount = x, 1f, DissolveDuration)
                    .OnUpdate(() => renderer.material.SetFloat("_DissolveAmount", dissolveAmount));

                sequence.Insert(Mathf.Max(DissolveDelay - DissolveDuration, 0f), tween);
            }
        }

        sequence.OnComplete(() => {sequence.Kill(); Destroy(gameObject);});

        return sequence;
    }

    private void Explode()
    {
        // apply explosion force to each individual enemy part from below to simulate it exploding
        // randomize explosion slightly to make it look less fake
        foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
        {
            float force = Random.Range(MinForce, MaxForce);
            float maxOffset = 0.5f;
            rb.AddExplosionForce(
                force,
                transform.position + new Vector3(Random.Range(-maxOffset, maxOffset), Random.Range(-1f, -0.5f), Random.Range(-maxOffset, maxOffset)),
                Radius);
            rb.AddTorque(new Vector3(Random.Range(-.5f, .5f), Random.Range(-.5f, .5f), Random.Range(-.5f, .5f)) * force);
        }
    }


}

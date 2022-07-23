using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// This controls the arrow indicator that shows the player how to play
/// </summary>

public class TutorialArrowBehavior : MonoBehaviour
{
    private Image image;
    private ImageAnimator imageAnim;

    [Tooltip("The position to start moving from")]
    public Vector3 StartPosition;
    [Tooltip("The position to move to")]
    public Vector3 EndPosition;
    [Tooltip("The time it takes to move from its start to end position")]
    public float MoveTime = 1f;
    [Tooltip("The time it takes to reset before returning to the start position")]
    public float ResetTime = 0.5f;
    [Tooltip("The time it takes to fade in or out")]
    public float FadeTime = 0.5f;
    private bool animDone = true;
    private Tween currentTween;

    void Awake()
    {
        image = GetComponent<Image>();
        imageAnim = GetComponent<ImageAnimator>();
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Assign new animation to the arrow
    /// </summary>
    public void StartAnimation(Vector3 fromPos, Vector3 toPos, float moveTime)
    {
        StopAllCoroutines();
        if (currentTween != null)
        {
            currentTween.Kill();
        }
        animDone = false;
        StartPosition = fromPos;
        EndPosition = toPos;
        MoveTime = moveTime;
        transform.position = StartPosition;
        image.DOFade(0, 0);
        StartCoroutine(LoopAnim());
    }

    private IEnumerator LoopAnim()
    {
        while (!animDone)
        {
            // -- Tween Sequence -- \\
            // Fade in
            imageAnim.ResetAnimation();
            currentTween = image.DOFade(1, FadeTime);
            yield return new WaitForSeconds(FadeTime);
            
            // Move
            currentTween = transform.DOMove(EndPosition, MoveTime);
            yield return new WaitForSeconds(MoveTime);

            // Fade out
            currentTween = image.DOFade(0, FadeTime);
            yield return new WaitForSeconds(FadeTime);

            // Move back
            currentTween = transform.DOMove(StartPosition, ResetTime);
            yield return new WaitForSeconds(ResetTime);
        }
    }
}

using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct TweenIn
{
    public Vector2 StartingOffeset;
    public Ease EaseStyle;
    public float TweenTime;
}

[System.Serializable]
public struct TweenOut
{
    public Ease EaseStyle;
    public float TweenTime;
    public Vector2 EndPosition;
}
[System.Serializable]
public struct TweenableObject {
    public string name;
    [Header("Main Tweenable Object")]
    public RectTransform ObjToTween;
    [Header("-- Tweening Settings --")]
    public TweenIn TweenInSettings;
    public TweenOut TweenOutSettings;
    [Header("Fade In")]
    public bool fadeIn;
    public float fadeInSpeed;
    [Header("Fade Out")]
    public bool fadeOut;
    public float fadeOutSpeed;

    [Header(" -- CopyCats Are Objects That Copy These Tween Behaviors --")]
    public List<RectTransform> CopyCats;
}
public class DialogueTweener : MonoBehaviour
{
    [SerializeField] private List<TweenableObject> tweenableObjects;
    public List<TweenableObject> TweenableObjects
    {
        get { return tweenableObjects; }
        private set { tweenableObjects = value; }
    }

    /// <summary>
    /// Called on the last dialouge line
    /// </summary>
    public void DialogueTween(string TweenMethod)
    {
        foreach (TweenableObject tweenable in TweenableObjects)
        {
            if (tweenable.CopyCats.Count == 0) 
            {
                StartCoroutine(TweenMethod, tweenable);
                return;
            }
            else 
            {
                StartCoroutine(TweenMethod, tweenable);
                foreach (RectTransform copyObj in tweenable.CopyCats)
                {
                    TweenableObject newTweenable = new TweenableObject();
                    newTweenable = tweenable;
                    newTweenable.ObjToTween = copyObj;
                    StartCoroutine(TweenMethod, newTweenable);
                }
            }
        }
    }

    /// <summary>
    /// Tweens the UI objects position and alpha channels
    /// </summary>
    public IEnumerator OnStartTween(TweenableObject tweenable)
    {
        // -- Tween Alpha Channel -- \\
        if (tweenable.fadeIn)
        {
            if (tweenable.ObjToTween.TryGetComponent(out Image img))
            {
                float endAlpha = .8f;
                img.color = new Color(img.color.r, img.color.g, img.color.b, 0f);
                DOTween.To(() => img.color, x => img.color = x, new Color(img.color.r, img.color.g, img.color.b, endAlpha), tweenable.fadeInSpeed);
            }
            else if (tweenable.ObjToTween.TryGetComponent(out TextMeshProUGUI TMProUI))
            {
                float endAlpha = 1;
                TMProUI.color = new Color(TMProUI.color.r, TMProUI.color.g, TMProUI.color.b, 0f);
                DOTween.To(() => TMProUI.color, x => TMProUI.color = x, new Color(TMProUI.color.r, TMProUI.color.g, TMProUI.color.b, endAlpha), tweenable.fadeInSpeed);
            }
            else if (tweenable.ObjToTween.TryGetComponent(out Text TextUI))
            {
                float endAlpha = 1;
                TextUI.color = new Color(TextUI.color.r, TextUI.color.g, TextUI.color.b, 0f);
                DOTween.To(() => TextUI.color, x => TextUI.color = x, new Color(TextUI.color.r, TextUI.color.g, TextUI.color.b, endAlpha), tweenable.fadeInSpeed);
            }
        }

        // -- Tween Position -- \\
        tweenable.ObjToTween.anchoredPosition += tweenable.TweenInSettings.StartingOffeset;

        Sequence sequence = DOTween.Sequence();
        sequence.Append(tweenable.ObjToTween.DOAnchorPos(Vector2.zero, tweenable.TweenInSettings.TweenTime).SetEase(tweenable.TweenInSettings.EaseStyle));
        sequence.Play();
        yield return new WaitUntil(() => sequence.IsComplete());
        yield return 0;
    }

    public IEnumerator OnEndTween(TweenableObject tweenable)
    {
        // -- Tween Alpha Channel -- \\
        if (tweenable.fadeOut)
        {
            if (tweenable.ObjToTween.TryGetComponent(out Image img))
            {
                float endAlpha = 0;
                DOTween.To(() => img.color, x => img.color = x, new Color(img.color.r, img.color.g, img.color.b, endAlpha), tweenable.fadeOutSpeed);
            }
            else if (tweenable.ObjToTween.TryGetComponent(out TextMeshProUGUI TMProUI))
            {
                float endAlpha = 0;
                DOTween.To(() => TMProUI.color, x => TMProUI.color = x, new Color(TMProUI.color.r, TMProUI.color.g, TMProUI.color.b, endAlpha), tweenable.fadeOutSpeed);
            }
            else if (tweenable.ObjToTween.TryGetComponent(out Text TextUI))
            {
                float endAlpha = 0;
                DOTween.To(() => TextUI.color, x => TextUI.color = x, new Color(TextUI.color.r, TextUI.color.g, TextUI.color.b, endAlpha), tweenable.fadeOutSpeed);
            }
        }

        // -- Tween Position -- \\
        tweenable.ObjToTween.DOAnchorPosX(tweenable.TweenOutSettings.EndPosition.x, tweenable.TweenOutSettings.TweenTime).SetEase(tweenable.TweenOutSettings.EaseStyle);
        tweenable.ObjToTween.DOAnchorPosY(tweenable.TweenOutSettings.EndPosition.y, tweenable.TweenOutSettings.TweenTime).SetEase(tweenable.TweenOutSettings.EaseStyle);
        yield return new WaitForSeconds(tweenable.TweenOutSettings.TweenTime);
        yield return null;
    }
}

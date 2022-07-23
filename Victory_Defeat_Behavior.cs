using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

[System.Serializable]
public struct TweenOffset
{
    public static float LeftOffset = -1200f;
    public static float RightOffset = 1200f;
}

/// <summary>
///  Handles effects of winning and losing waves
/// </summary>

public class Victory_Defeat_Behavior : MonoBehaviour
{
    [Header("Materials To Blink On Wave Completion (in order)")]
    public List<MeshRenderer> buildingRenderers;
    
    [Header("Each line and backdrop is seperated for custom tweens")]
    public TextMeshProUGUI TextTop;
    public Image BackdropTop;
    public TextMeshProUGUI TextBottom;
    public Image BackdropBottom;

    [Header("Text Colors")]
    public Color WinColor = new Vector4(0, 1, 1, 0);
    public Color LoseColor = new Vector4(1, 0, 0, 0);

    [Header("Curve for blink speed")]
    public AnimationCurve BlinkSpeedCurve;


    #region Private Variables
    private Vector2 textBottomStart;
    private Vector2 textTopStart;
    private TweenOffset tweenOffset;
    private Volume postProcessing;
    private float minAlpha = 0f;
    private float maxAlpha = 1.0f;
    private float maxBackAlpha = 0.9f;
    #endregion
    private void Awake()
    {
        postProcessing = GameObject.FindWithTag("PostProcessing").GetComponent<Volume>();
        textTopStart = BackdropTop.rectTransform.anchoredPosition;
        textBottomStart = BackdropBottom.rectTransform.anchoredPosition;
    }
    void Start()
    {
        Vector4 backdropColor = BackdropTop.color;
        backdropColor.w = 0;
        TextTop.color = WinColor;
        BackdropBottom.color = backdropColor;
        TextBottom.color = WinColor;
        BackdropTop.color = backdropColor;

        //remove null assignments
        for (int i = 0; i < buildingRenderers.Count; i++)
        {
            if (buildingRenderers[i] == null)
            {
                buildingRenderers.RemoveAt(i);
                i--;
            }
        }
    }

    public enum TweenStartLoc
    {
        Left,
        Right,
    }

    public enum TextID
    {
        Top,
        Bottom,
    }


    public void WaveCompleted()
    {
        SetTextData(TextTop, "WAVE COMPLETED", WinColor);
        SetTextData(TextBottom, "WELL DONE!", WinColor);
        StartCoroutine(WaveCompletedRoutine());
        MaterialBlink();
    }

    public void WaveFailed()
    {
        SetTextData(TextTop, "WAVE FAILED", LoseColor);
        SetTextData(TextBottom, "REBOOTING...", LoseColor);
        StartCoroutine(WaveLostRoutine());
    }

    /// <summary>
    /// Builds and plays a custom DOTween sequence for animating in 
    /// from the left or right of the screen             
    /// </summary>
    /// <param name="m_textComponent"></param>
    /// <param name="startLocation"></param>

    public void TweenWaveResultText(TextID textID, TweenStartLoc startLocation, out Sequence tweenSequence)
    {
        // Determine 
        TextMeshProUGUI text;
        Image backdrop;
        switch(textID)
        {
            case TextID.Top:
                text = TextTop;
                backdrop = BackdropTop;
                break;
            case TextID.Bottom:
                text = TextBottom;
                backdrop = BackdropBottom;
                break;
            default:
                tweenSequence = null;
                return;
        }

        // Play animation
        switch (startLocation)
        {
            case TweenStartLoc.Left:
                // -- Build DOTween Sequence for left text-- \\
                Sequence TextTweenSequence_L = DOTween.Sequence();
                TextTweenSequence_L.Append(backdrop.rectTransform.DOAnchorPosX(1.5f * TweenOffset.LeftOffset, 0));
                TextTweenSequence_L.Append(text.DOFade(maxAlpha, 0));
                TextTweenSequence_L.Append(backdrop.DOFade(maxBackAlpha, 0));
                TextTweenSequence_L.Append(backdrop.rectTransform.DOAnchorPosX(textTopStart.x, 1f).SetEase(Ease.OutExpo));
                TextTweenSequence_L.AppendInterval(0.5f);
                TextTweenSequence_L.Append(backdrop.rectTransform.DOAnchorPosX(1.5f * TweenOffset.RightOffset, 0.7f).From(textTopStart).SetEase(Ease.OutExpo));
                TextTweenSequence_L.Append(text.DOFade(minAlpha, 0));
                TextTweenSequence_L.Append(backdrop.DOFade(minAlpha, 0));
                //Play animation
                TextTweenSequence_L.Play();
                tweenSequence = TextTweenSequence_L;
                break;
            case TweenStartLoc.Right:
                // -- Build DOTween Sequence for right text -- \\
                Sequence TextTweenSequence_R = DOTween.Sequence();
                TextTweenSequence_R.Append(backdrop.rectTransform.DOAnchorPosX(TweenOffset.RightOffset, 0));
                TextTweenSequence_R.Append(text.DOFade(maxAlpha, 0));
                TextTweenSequence_R.Append(backdrop.DOFade(maxBackAlpha, 0));
                TextTweenSequence_R.Append(backdrop.rectTransform.DOAnchorPosX(textBottomStart.x, 1f).SetEase(Ease.OutExpo));
                TextTweenSequence_R.AppendInterval(0.5f);
                TextTweenSequence_R.Append(backdrop.rectTransform.DOAnchorPosX(1.5f * TweenOffset.LeftOffset, 0.7f).From(textBottomStart).SetEase(Ease.OutExpo));
                TextTweenSequence_R.Append(text.DOFade(maxAlpha, 0));
                TextTweenSequence_R.Append(backdrop.DOFade(minAlpha, 0));
                //Play animation
                TextTweenSequence_R.Play();
                tweenSequence = TextTweenSequence_R;
                break;
            default:
                tweenSequence = null;
                break;
        }


    }
    /// <summary>
    /// Handles the Tweening of the UI text from off the screen into the middle
    /// </summary>
    /// <returns></returns>
    private IEnumerator WaveCompletedRoutine()
    {

        // Initialize sequences
        Sequence animSequenceL;
        Sequence animSequenceR;

        // Call tween
        TweenWaveResultText(TextID.Top, TweenStartLoc.Left, out animSequenceL);
        TweenWaveResultText(TextID.Bottom, TweenStartLoc.Right, out animSequenceR);

        //Enable GO's
        BackdropBottom.gameObject.SetActive(true);
        BackdropTop.gameObject.SetActive(true);

        // Wait for completion
        yield return new WaitUntil(animSequenceL.IsComplete);
        yield return new WaitUntil(animSequenceR.IsComplete);

        // Disbale GO's
        BackdropBottom.gameObject.SetActive(false);
        BackdropTop.gameObject.SetActive(false);

        yield return 0;
    }


    /// <summary>
    /// Handles the Tweening of the UI text from off the screen into the middle
    /// This contains different post processing effects so thats why they are two 
    /// different methods
    /// </summary>
    /// <returns></returns>
    private IEnumerator WaveLostRoutine()
    {
        Sequence animSequenceL;
        Sequence animSequenceR;
        // Tween top text
        TweenWaveResultText(TextID.Top, TweenStartLoc.Left, out animSequenceL);
        // Tween bottom text
        TweenWaveResultText(TextID.Bottom, TweenStartLoc.Right, out animSequenceR);

        //Camera Shake
        Camera.main.DOShakePosition(1.5f, .15f, 20, 90, true);

        #region Postprocessing
        // Chromatic Aberation Effect
        ChromaticAberration m_component = (ChromaticAberration)postProcessing.profile.components[3]; 
        VolumeParameter<float> intensity = m_component.intensity;

        Sequence ChromaticTweenSequence = DOTween.Sequence();
        ChromaticTweenSequence.Append(DOTween.To(() => intensity.value, x => intensity.value = x, 1.0f, 1.0f));
        ChromaticTweenSequence.Append(DOTween.To(() => intensity.value, x => intensity.value = x, 0f, 1.0f));

        // Color adjustments effect
        ColorAdjustments colorAdjust = (ColorAdjustments)postProcessing.profile.components[2];
        VolumeParameter<Color> color = colorAdjust.colorFilter;

        Sequence ColorTweenSequence = DOTween.Sequence();
        ColorTweenSequence.Append(DOTween.To(() => color.value, x => color.value = x, Color.red, 1f));
        ColorTweenSequence.Append(DOTween.To(() => color.value, x => color.value = x, Color.white, 1f));
        
        // Play animation
        ChromaticTweenSequence.Play();
        ColorTweenSequence.Play();
        #endregion

        yield return new WaitForSeconds(.5f);
        yield return new WaitUntil(animSequenceL.IsComplete);
        yield return new WaitUntil(animSequenceR.IsComplete);

        yield return 0;
    }

    public void MaterialBlink()
    {
        StartCoroutine(BlinkRoutine());
    }

    /// <summary>
    /// Cycles through the list of renderers
    /// and increases or decreases their intesity on a time interval.
    /// Results in a pinball game style lighting effect 
    /// </summary>
    /// <returns></returns>
    private IEnumerator BlinkRoutine()
    {
        float waitTime = BlinkSpeedCurve.Evaluate(buildingRenderers.Count);
        for (int i = 0; i < buildingRenderers.Count; i++)
        {
            buildingRenderers[i].materials[0].SetFloat("_EmissionIntensity", 1f);
            yield return new WaitForSeconds(waitTime);
        }
        for (int i = 0; i < buildingRenderers.Count; i++)
        {
            buildingRenderers[i].materials[0].SetFloat("_EmissionIntensity", 2.5f);
            yield return new WaitForSeconds(waitTime);         
        }
        for (int i = 0; i < buildingRenderers.Count; i++)
        {
            buildingRenderers[i].materials[0].SetFloat("_EmissionIntensity", 1f);
            yield return new WaitForSeconds(waitTime);
        }
        for (int i = 0; i < buildingRenderers.Count; i++)
        {
            buildingRenderers[i].materials[0].SetFloat("_EmissionIntensity", 2.5f);
            yield return new WaitForSeconds(waitTime);
        }
        for (int i = 0; i < buildingRenderers.Count; i++)
        {
            buildingRenderers[i].materials[0].SetFloat("_EmissionIntensity", 1f);
            yield return new WaitForSeconds(waitTime);
        }
       
        yield return 0;
    }

    /// <summary>
    /// Returns if the passed sequence is still active or not
    /// </summary>
    /// <param name="sequence"></param>
    /// <returns></returns>
    public bool TweenActive(Sequence sequence)
    {
        if (sequence.active)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Helper method to set the components text and color
    /// </summary>
    /// <param name="m_textComponent"></param>
    /// <param name="m_text"></param>
    /// <param name="m_color"></param>
    private void SetTextData(TextMeshProUGUI m_textComponent, string m_text, Color m_color)
    {
        m_textComponent.text = m_text;
        m_textComponent.color = m_color;
    }
}

using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class WaveTextEffect : MonoBehaviour
{
    [TextArea(2,2)]
    public string note = "Tweens each letter in the list by a scale and a time between each letter";

    public List<TextMeshPro> TextObjects;
    public float AnimationSpeed;

    public void StartTweening()
    {
        StartCoroutine(TweenAllLetters());
    }
    public IEnumerator TweenAllLetters()
    {
        yield return new WaitForSeconds(1.5f);
        for (int i = 0; i < TextObjects.Count; i++)
        {
            if (i == TextObjects.Count - 1)
            {
                // Update counter
                BroadcastMessage("SetWaveNum");
                //Last object in the tween list
                StartCoroutine(TweenLetter(TextObjects[i], .4f, Vector3.one*1.5f));
            }
            else
            {
                StartCoroutine(TweenLetter(TextObjects[i], .25f, Vector3.one*1.7f));
            }
            yield return new WaitForSeconds(AnimationSpeed);
        }
        // Play Wave Start sound
        AudioManager.Instance.PlayAudio(AudioIdentifiers.WaveStart);
        yield return 0;
    }

    private IEnumerator TweenLetter(TextMeshPro letter, float time, Vector3 maxScale)
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(letter.transform.DOScale(maxScale, time).From(Vector3.one).SetEase(Ease.OutCirc));
        sequence.Append(letter.transform.DOScale(Vector3.one, time));
        sequence.Play();
        yield return new WaitUntil(sequence.IsComplete);
        yield return 0;
    }
}

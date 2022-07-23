using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BossWarningTweener : DialogueTweener
{
    public TextMeshProUGUI Text;
    private void OnEnable()
    {
        transform.GetChild(0).gameObject.SetActive(true);
        StartCoroutine(TextBlink());
        DialogueTween("OnStartTween");
    }

    public IEnumerator TextBlink()
    {
        Camera cam = Camera.main;
        Sequence sequence = DOTween.Sequence();
        sequence.Append(Text.DOFade(.5f, .25f));
        sequence.Append(Text.DOFade(1f, .25f));
        sequence.Append(Text.DOFade(.5f, .25f));
        sequence.Append(Text.DOFade(1f, .25f));
        sequence.AppendCallback(() => cam.DOShakePosition(3f, .1f));
        sequence.Append(Text.DOFade(.5f, .25f));
        sequence.Append(Text.DOFade(1f, .25f));
        sequence.Append(Text.DOFade(.5f, .25f));
        sequence.Append(Text.DOFade(1f, .25f));
        sequence.Append(Text.DOFade(.5f, .25f));
        sequence.Append(Text.DOFade(1f, .25f));
        sequence.Append(Text.DOFade(.5f, .25f));
        sequence.Append(Text.DOFade(1f, .25f));

        sequence.Play();
        sequence.OnComplete(() => {
            sequence.Kill();
        });

        yield return new WaitForSeconds(3f);
        DialogueTween("OnEndTween");
        yield return new WaitForSeconds(2f);
        transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        transform.GetChild(0).gameObject.SetActive(false);
        this.enabled = false;
        yield return 0;
    }
}

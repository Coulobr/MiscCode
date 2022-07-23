using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class IncomingWaveArrow : MonoBehaviour
{
    public Transform StartLoc;
    public Transform EndLoc;

    private void OnEnable()
    {
        StartCoroutine(Animate());
    }

    private IEnumerator Animate()
    {
        // Move back and forth 3 times
        int animCount = 0;
        while (animCount < 3)
        {
            transform.DOMove(EndLoc.position, 1.5f).From(StartLoc.position);
            yield return new WaitForSeconds(1.5f);
            transform.position = StartLoc.position;
            animCount++;
        }

        //Fade out and disable
        transform.GetChild(0).GetComponent<TextMeshPro>().DOFade(0, .5f);
        GetComponent<SpriteRenderer>().DOFade(0, .5f);
        yield return new WaitForSeconds(.5f);   
        transform.GetChild(0).GetComponent<TextMeshPro>().DOFade(1, .1f);
        GetComponent<SpriteRenderer>().DOFade(1, .1f);
        gameObject.SetActive(false);
        StopCoroutine(Animate());
    }
}

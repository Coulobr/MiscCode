using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrencyIcon : MonoBehaviour
{
    private RectTransform TargetRT;
    private void Awake() => TargetRT = transform.GetChild(0).GetComponent<RectTransform>();
    public void Animate()
    {
        float scale = 2f;
        TargetRT.localScale = Vector3.one;
        TargetRT.DOScale(new Vector3(scale, scale, scale), 0.2f).From();
    }
}

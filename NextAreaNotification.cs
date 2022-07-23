using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class NextAreaNotification : MonoBehaviour
{
    public Text AreaCostText;

    private void OnEnable()
    {
        AreaCostText.text = "Cost: " + AreaProgression.Instance.CurrentAreaProgCost.ToString();
        Tween();
    }
    public void Tween()
    {
        transform.DOScale(1, 1).From(Vector3.zero).SetEase(Ease.OutBack);
    }
}

using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FlyingCog : MonoBehaviour
{
    public const float MIN_COG_FLY_SPEED = 1f;
    public const float MAX_COG_FLY_SPEED = 2.5f;

    private float cogSpeed;
    private RectTransform thisRT;
    private int easeType;
    private int RotDir;

    public Vector2 EndPosition;
    public Vector2 StartPosition;
    
    private void Awake()
    {
        thisRT = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        cogSpeed = UnityEngine.Random.Range(MIN_COG_FLY_SPEED, MAX_COG_FLY_SPEED);
        easeType = Random.Range(0, 4);
        RotDir = Random.Range(0, 2);
        StartCoroutine(Move(EndPosition, cogSpeed));
    }

    private void Update()
    {
        switch (RotDir)
        {
            case 0:
                transform.Rotate(new Vector3(0, 0, 2)); break;
            case 1:
                transform.Rotate(new Vector3(0, 0, -2)); break;
        }
    }

    public IEnumerator Move(Vector2 endPos, float time)
    {
        switch (easeType)
        {
            case 0:
                thisRT.DOAnchorPos(EndPosition, time).From(StartPosition).SetEase(Ease.OutCubic);
                yield return new WaitUntil(() => Vector2.Distance(EndPosition, thisRT.anchoredPosition) < 15);
                SendMessageUpwards("EffectCompleteCallback");
                gameObject.SetActive(false); 
                break;
            case 1:
                thisRT.DOAnchorPos(EndPosition, time).From(StartPosition).SetEase(Ease.OutSine);
                yield return new WaitUntil(() => Vector2.Distance(EndPosition, thisRT.anchoredPosition) < 15); 
                SendMessageUpwards("EffectCompleteCallback");
                gameObject.SetActive(false); 
                break;
            case 2:
                thisRT.DOAnchorPos(EndPosition, time).From(StartPosition).SetEase(Ease.InQuad);
                yield return new WaitUntil(() => Vector2.Distance(EndPosition, thisRT.anchoredPosition) < 15); 
                SendMessageUpwards("EffectCompleteCallback");
                gameObject.SetActive(false);
                break;
            case 3:
                thisRT.DOAnchorPos(EndPosition, time).From(StartPosition).SetEase(Ease.InOutQuad);
                yield return new WaitUntil(() => Vector2.Distance(EndPosition, thisRT.anchoredPosition) < 15);
                SendMessageUpwards("EffectCompleteCallback");
                gameObject.SetActive(false);
                break;
            default:
                break;
        }

    }
}

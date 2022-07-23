using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AdvanceAreaButtonTweener : DialogueTweener
{
    public SceneTransitionHandler SceneTransitionHandler;
    public TextMeshProUGUI buttonText;
    public GameObject NotEnoughMoneyError;
    private Button advanceButton;
    

    private void Start()
    {
        // Disable the object if progression is not wanted
        gameObject.SetActive(AreaProgression.Instance.EnableProgression);
    }

    private void OnEnable()
    {
        advanceButton = GetComponent<Button>();
        buttonText.text = InitButtonText();
        DialogueTween("OnStartTween");
    }

    public void TryProgressArea()
    {
        if (AreaProgression.Instance.CanProgress() )
        {
            DOTween.KillAll();
            EventManager.Instance.RaiseGameEvent(EventConstants.LOAD_FROM_MAIN_MENU);
            Invoke("AdvanceAreaEvent", SceneTransitionHandler.SCENE_TRANSITION_TIME);
        }
        else 
        {
            if (AreaProgression.Instance.NextAreaAvailable())
            {
                GameObject errorMessage = Instantiate(NotEnoughMoneyError, transform);
                errorMessage.GetComponent<TextMeshProUGUI>().text = "Cost: " + AreaProgression.Instance.CurrentAreaProgCost;
                errorMessage.GetComponent<RectTransform>().DOAnchorPosY(150, 3f).From(Vector2.zero).SetEase(Ease.OutCirc);
                errorMessage.GetComponent<TextMeshProUGUI>().DOFade(0, 3f).SetEase(Ease.InCubic);
            }
            else
            {
                Debug.LogWarning("No sectors available to advance to");
            }
        }
    }
    public void AdvanceAreaEvent() => EventManager.Instance.RaiseGameEvent(EventConstants.AREA_PROGRESS);

    public string InitButtonText()
    {
        //Checks if there are more sectors available for purchase
        string text = "";
        switch (AreaProgression.Instance.NextAreaAvailable())
        {
            case true: text = "Buy Next Sector";
                break;
            case false: text = "New Sectors Soon!";
                break;
        }
        return text;
    }
}

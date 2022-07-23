using DG.Tweening;
using TMPro;
using UnityEngine;

namespace HitTrax.Animations
{
    public class UIStateChanger : MonoBehaviour
    {
        public TextMeshProUGUI textComponent;
        public SmartButton leftButton;
        public SmartButton rightButton;
        public float customTextPos;
        public float mlbTextPos;
        public float leftArrowMlbPos;
        public float rightArrowMlbPos;
        public float leftArrowCustomPos;
        public float rightArrowCustomPos;

        public void InitMLB()
        {
            textComponent.rectTransform.DOAnchorPosX(mlbTextPos, 1f);
            leftButton.GetComponent<RectTransform>().DOAnchorPosX(leftArrowMlbPos, 1f);
            rightButton.GetComponent<RectTransform>().DOAnchorPosX(rightArrowMlbPos, 1f);

            leftButton.ToggleState(true);
            rightButton.ToggleState(false);
        }

        public void InitCustom()
        {
            textComponent.rectTransform.DOAnchorPosX(customTextPos, 1f);
            leftButton.GetComponent<RectTransform>().DOAnchorPosX(leftArrowCustomPos, 1f);
            rightButton.GetComponent<RectTransform>().DOAnchorPosX(rightArrowCustomPos, 1f);

            leftButton.ToggleState(false);
            rightButton.ToggleState(true);
        }
    }
}
using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HitTrax.Bataround
{
	public class BataroundPopup : MonoBehaviour
	{
		public TextMeshProUGUI text;
		public TextMeshProUGUI bgText;
		public Image coloredGlow;
		public Image coloredHex;
		public Color successColor;
		public Color failedColor;
		public RectTransform mainContainer;

		public void Initiate(string msg, bool blink, BataroundPopupTextHandler handler, bool failed)
		{
			StartCoroutine(Co_TextPulse(msg, blink, handler, failed));
		}

		private IEnumerator Co_TextPulse(string msg, bool blink, BataroundPopupTextHandler handler, bool failed)
		{
			text.text = msg;
			bgText.text = msg;

			mainContainer.gameObject.SetActive(true);

			if (!failed)
			{
				text.color = successColor;
				bgText.color = successColor;
				coloredGlow.color = successColor;
				coloredHex.color = successColor;
			}
			else
			{
				text.color = failedColor;
				bgText.color = failedColor;
				coloredGlow.color = failedColor;
				coloredHex.color = failedColor;
			}

			//text.DOFade(1, .75f).From(0);
			//bgText.DOFade(1, .75f).From(0);
			mainContainer.DOAnchorPosX(0, 1.5f).From(new Vector2(-1300,0)).SetEase(Ease.OutCubic)
				.OnComplete(()=> mainContainer.DOAnchorPosX(1300, 1.5f).From(new Vector2(0, mainContainer.anchoredPosition.y))
					.OnComplete(() => {
						mainContainer.gameObject.SetActive(false);
						handler.inPopup = false;
						Destroy(this);
					}));

			//bgText.rectTransform.DOAnchorPosX(0, .75f).From(new Vector2(-1300, 0)).SetEase(Ease.OutCubic)
			//	.OnComplete(() => bgText.rectTransform.DOAnchorPosX(1300, .75f).From(new Vector2(0, 0))
			//		.OnComplete(() => {
			//			bgText.gameObject.SetActive(false);
			//			handler.inPopup = false;			
			//		}));

			yield return null;
		}
	}
}

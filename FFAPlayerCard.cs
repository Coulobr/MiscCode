using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HitTrax.Bataround
{
	public class FFAPlayerCard : MonoBehaviour
	{
		public Image background;

		public TextMeshProUGUI playerName;
		public TextMeshProUGUI avgDistance;
		public TextMeshProUGUI avgVelocity;
		public TextMeshProUGUI currentMinigameScore;
		public TextMeshProUGUI totalScore;

		public void SetData(User user)
		{
			HideAll();
			StartCoroutine(Co_SetData(user));
		}

		private IEnumerator Co_SetData(User user)
		{
			playerName.text = user.screenName;
			avgDistance.text = string.Format("{0:0.0}", user.stats.avgDistance);
			avgVelocity.text = string.Format("{0:0.0}", user.stats.avgExitVel);
			totalScore.text = user.BAM.TotalGameScore.ToString();

			background.DOFillAmount(1, .5f).From(0);
			background.DOFade(1, 0);
			yield return new WaitForSeconds(.35f);

			playerName.DOFade(1, .25f).From(0);
			yield return new WaitForSeconds(.15f);

			avgDistance.DOFade(1, .25f).From(0);
			yield return new WaitForSeconds(.15f);

			avgVelocity.DOFade(1, .25f).From(0);
			yield return new WaitForSeconds(.15f);

			currentMinigameScore.DOFade(1, .25f).From(0);
			yield return new WaitForSeconds(.15f);

			totalScore.DOFade(1, .25f).From(0);
			totalScore.DOScale(1, .35f).From(0).SetEase(Ease.OutBack);
		}

		public void HideAll()
		{
			background.DOFade(0, 0);
			playerName.DOFade(0, 0);
			avgDistance.DOFade(0, 0);
			avgVelocity.DOFade(0, 0);
			currentMinigameScore.DOFade(0, 0);
			totalScore.DOFade(0, 0);
		}
	}
}

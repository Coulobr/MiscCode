using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HitTrax.Bataround
{
	public class TeamEndGameCard : MonoBehaviour
	{
		public Image background;
		public Image outline;
		public Image nameUnderline;

		public TextMeshProUGUI playerName;
		public TextMeshProUGUI avgDistance;
		public TextMeshProUGUI avgVelocity;
		//public TextMeshProUGUI currentMinigameScore;
		public TextMeshProUGUI totalScore;


		public void SetData(User user)
		{
			StartCoroutine(Co_SetData(user));
		}

		private IEnumerator Co_SetData(User user)
		{
			//background.DOFillAmount(1, 1).From(0);
			background.color = user.BataroundTeam == 0 ? Globals.jerseyBlue : Globals.jerseyRed;

			background.DOFade(1, .5f).From(0);
			outline.DOFade(1, .5f).From(0);

			//outline.rectTransform.DOAnchorPosY(-167, .5f).From(new Vector2(0, 0));
			//outline.rectTransform.DOSizeDelta(new Vector2(outline.rectTransform.GetWidth(), outline.rectTransform.GetHeight()), .5f).From(new Vector2(outline.rectTransform.GetWidth(), 0f));

			yield return new WaitForSeconds(.35f);

			nameUnderline.DOFade(1, .5f).From(0);

			playerName.text = user.screenName;
			avgDistance.text = "Avg. Dist: " + string.Format("{0:0.0}", user.stats.avgDistance);
			avgVelocity.text = "Avg. Velo: " + string.Format("{0:0.0}", user.stats.avgExitVel);
			//currentMinigameScore.text = currentScore;
			totalScore.text = "Score " + user.BAM.TotalGameScore;

			playerName.DOFade(1, .75f).From(0);
			avgDistance.DOFade(1, .75f).From(0);
			avgVelocity.DOFade(1, .75f).From(0);
			totalScore.DOFade(1, .75f).From(0);
		}

		public void HideAll()
		{
			background.DOFade(0, 0);
			playerName.DOFade(0, 0);
			avgDistance.DOFade(0, 0);
			avgVelocity.DOFade(0, 0);
			totalScore.DOFade(0, 0);
			outline.DOFade(0, 0);
			nameUnderline.DOFade(0, 0);
		}
	}
}


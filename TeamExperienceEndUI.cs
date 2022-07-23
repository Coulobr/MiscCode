using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HitTrax.Bataround
{
	public class TeamExperienceEndUI : Menu<TeamExperienceEndUI>
	{
		public GameObject teamCard;
		public EndExperienceUserListPanel leaderboardPanel;
		public BataroundPlaysListPanel playsListPanel;
		public Transform cardLayout;
		public TextMeshProUGUI winnerText;
		public TextMeshProUGUI teamAScore;
		public TextMeshProUGUI teamBScore;
		public Image topOutline;
		public Image bottomOutline;
		public Image leftGradient;
		public Image rightGradient;
		public SmartButton continueBtn;

		protected override void OnOpened()
		{
			base.OnOpened();

			continueBtn.onClick.AddListener(OnContinueClick);
			leaderboardPanel.Open();

			teamAScore.text = BataroundGameManager.Instance.CurrentBataroundGroup.TeamARoundsWon.ToString();
			teamBScore.text = BataroundGameManager.Instance.CurrentBataroundGroup.TeamBRoundsWon.ToString();

			var totalTeamAScore = BataroundGameManager.Instance.CurrentBataroundGroup.TeamARoundsWon;
			var totalTeamBScore = BataroundGameManager.Instance.CurrentBataroundGroup.TeamBRoundsWon;

			if (totalTeamAScore > totalTeamBScore)
			{
				winnerText.text = $"Team A Wins!";
			}
			else if (totalTeamAScore == totalTeamBScore)
			{
				winnerText.text = $"Tie!";
			}
			else
			{
				winnerText.text = $"Team B Wins!";
			}

			StartCoroutine(Co_ScoreSequence());
		}

		protected override void OnClosed()
		{
			base.OnClosed();
			leaderboardPanel.Close();
			continueBtn.onClick.RemoveAllListeners();
			BataroundSessionsManager.Instance.LoadSession<BataroundSplashSession>();
		}

		private void OnContinueClick()
		{
			Close();
		}

		private IEnumerator Co_ScoreSequence()
		{
			teamAScore.DOFade(0, 0);
			teamBScore.DOFade(0, 0);

			topOutline.DOFade(.65f, .5f).From(0);
			bottomOutline.DOFade(.65f, .5f).From(0);

			leftGradient.DOFillAmount(1, .5f).From(0);
			rightGradient.DOFillAmount(1, .5f).From(0);

			yield return new WaitForSeconds(.5f);

			teamAScore.DOFade(1, 0);
			teamAScore.DOScale(1, .5f).From(0).SetEase(Ease.OutBack);

			yield return new WaitForSeconds(.5f);

			teamBScore.DOFade(1, 0);
			teamBScore.DOScale(1, .5f).From(0).SetEase(Ease.OutBack);
		}
	}
}
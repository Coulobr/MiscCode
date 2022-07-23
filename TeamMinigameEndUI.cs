using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HitTrax.Bataround
{
	public class TeamMinigameEndUI : Menu<TeamMinigameEndUI>
	{
		public PostMatchUserListPanel userListPanel;
		public BataroundPlaysListPanel playsListPanel;

		public Image leftGradient;
		public Image rightGradient;
		public Image topOutline;
		public Image bottomOutline;

		public TextMeshProUGUI teamAScore;
		public TextMeshProUGUI teamBScore;
		public TextMeshProUGUI title;

		public GameObject playerCard;
		public Transform layout;
		public SmartButton continueBtn;

		protected override void OnInitialized()
		{
			base.OnInitialized();

			if (continueBtn)
			{
				continueBtn.onClick.RemoveAllListeners();
				continueBtn.onClick.AddListener(OnContinueClick);
			}
		}

		protected override void OnOpened()
		{
			base.OnOpened();

			teamAScore.text = BataroundGameManager.Instance.CurrentBataroundGroup.TeamARoundsWon.ToString();
			teamBScore.text = BataroundGameManager.Instance.CurrentBataroundGroup.TeamBRoundsWon.ToString();

			StartCoroutine(Co_ScoreSequence());

			userListPanel.listGenerated = false;
			userListPanel.Open();
		}

		protected override void OnClosed()
		{
			base.OnClosed();
			userListPanel.Close();
			if (BataroundGameManager.Instance.CurrentlyInLinas)
			{
				BataroundGameManager.Instance.CurrentMinigame = BataroundSessionsManager.Instance.LoadSession<BataroundAroundTheWorldMinigame>();
			}
			else if (BataroundGameManager.Instance.CurrentlyInAroundTheWorld)
			{
				BataroundGameManager.Instance.CurrentMinigame = BataroundSessionsManager.Instance.LoadSession<SmallBallMinigame>();
			}
			else if (BataroundGameManager.Instance.CurrentlyInSmallBall)
			{
				BataroundGameManager.Instance.CurrentMinigame = BataroundSessionsManager.Instance.LoadSession<LaserShowMinigame>();
			}
			else if (BataroundGameManager.Instance.CurrentlyInLaserShow)
			{
				BataroundGameManager.Instance.CurrentMinigame = BataroundSessionsManager.Instance.LoadSession<WalkOffMinigame>();
			}
			else if (BataroundGameManager.Instance.CurrentlyInWalkOff)
			{
				BataroundGameManager.Instance.CurrentMinigame = BataroundSessionsManager.Instance.LoadSession<BataroundModeMinigame>();
			}
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

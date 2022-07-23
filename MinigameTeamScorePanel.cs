using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

namespace HitTrax.Bataround
{
	public class MinigameTeamScorePanel : Panel<MinigameTeamScorePanel>
	{
		public TextMeshProUGUI teamALabel;
		public TextMeshProUGUI teamBLabel;
		public TextMeshProUGUI teamAScore;
		public TextMeshProUGUI teamBScore;
		public TextMeshProUGUI teamLeadsText;

		public TextMeshProUGUI gameModeTitle;
		public TextMeshProUGUI description;
		public RectTransform descriptionContainer;
		public RectTransform gameModeContainer;
		public Button gameModeBtn;
		
		protected override void OnOpened()
		{
			base.OnOpened();
			ResetPanel();
			UpdatePanel();
			SetGameModeName();
		}

		protected override void OnClosed()
		{
			base.OnClosed();
		}

		public override void SetupPanel()
		{
			base.SetupPanel();
			var gameManager = BataroundGameManager.Instance;

			Objects.Instance.natTrack().OnEnterPlay -= OnEnterPlay;
			gameManager.TeamAPoint -= OnTeamAPoint;
			gameManager.TeamBPoint -= OnTeamBPoint;
			gameManager.OnHitterChanged -= OnHitterChanged;
			gameManager.OnPlayerTurnStart -= OnPlayerTurnStart;
			gameManager.OnUndoLastPlay -= OnUndoLastPlay;
			gameModeBtn.onClick.RemoveAllListeners();

			Objects.Instance.natTrack().OnEnterPlay += OnEnterPlay;
			gameManager.TeamAPoint += OnTeamAPoint;
			gameManager.TeamBPoint += OnTeamBPoint;
			gameManager.OnHitterChanged += OnHitterChanged;
			gameManager.OnPlayerTurnStart += OnPlayerTurnStart;
			gameManager.OnUndoLastPlay += OnUndoLastPlay;
			gameModeBtn.onClick.AddListener(OnClickDropdown);
		}

		private void OnUndoLastPlay()
		{
			UpdatePanel();
		}

		private void OnEnterPlay(Play obj)
		{
			if (descriptionContainer.anchoredPosition.y > 0)
			{
				HideDescription();
			}
		}

		public override void ResetPanel()
		{
			base.ResetPanel();
			description.text = "";
			gameModeTitle.text = "";
			teamAScore.text = "0";
			teamBScore.text = "0";
			teamLeadsText.text = $"Match tied at 0 - 0";
		}

		public override void UpdatePanel()
		{
			base.UpdatePanel();
			SetTeamLeadsText(BataroundGameManager.Instance.CurrentBataroundGroup.TeamARoundsWon, BataroundGameManager.Instance.CurrentBataroundGroup.TeamBRoundsWon);
			SetDescription(); 
			SetGameModeName();

			if (BataroundGameManager.Instance.CurrentlyInBataround)
			{
				var teamAScore = BataroundGameManager.Instance.CurrentBataroundGroup.TeamARoundsWon + BataroundGameManager.Instance.CurrentBataroundGroup.TeamABataroundRuns;
				var teamBScore = BataroundGameManager.Instance.CurrentBataroundGroup.TeamBRoundsWon + BataroundGameManager.Instance.CurrentBataroundGroup.TeamBBataroundRuns;
				this.teamAScore.text = teamAScore.ToString();
				this.teamBScore.text = teamBScore.ToString();
			}
		}

		private void ShowDescription()
		{
			descriptionContainer.DOAnchorPosY(-42f, .66f);
		}

		private void HideDescription()
		{
			descriptionContainer.DOAnchorPosY(0f, .66f);
		}

		private void OnClickDropdown()
		{
			SetDescription();
			if (descriptionContainer.anchoredPosition.y < 0)
			{
				HideDescription();
			}
			else
			{
				ShowDescription();
			}
		}

		private void OnPlayerTurnStart()
		{
			HideDescription();
		}

		private void OnHitterChanged(User user)
		{
			UpdatePanel();
			ShowDescription();
		}

		private void OnTeamAPoint()
		{
			var currentScore = int.Parse(teamAScore.text);
			currentScore += 1;
			teamAScore.text = currentScore.ToString();
		}

		private void OnTeamBPoint()
		{
			var currentScore = int.Parse(teamBScore.text);
			currentScore += 1;
			teamBScore.text = currentScore.ToString();
		}

		private void SetDescription()
		{
			var gameManager = BataroundGameManager.Instance;

			if (gameManager.CurrentlyInLinas)
			{
				description.text = "Hit Line Drives";
			}
			else if (gameManager.CurrentlyInAroundTheWorld)
			{
				description.text = "Hit in the Wedges";
			}
			else if (gameManager.CurrentlyInLaserShow)
			{
				description.text = "Hit it Hard";
			}
			else if (gameManager.CurrentlyInSmallBall)
			{
				description.text = "Bat the Runner Home";
			}
			else if (gameManager.CurrentlyInBataround)
			{
				description.text = "Score Runs";
			}
			else if (gameManager.CurrentlyInWalkOff)
			{
				description.text = "Score a Run";
			}
		}

		private void SetGameModeName()
		{
			var gameManager = BataroundGameManager.Instance;

			if (gameManager.CurrentlyInLinas)
			{
				gameModeTitle.text = "Liñas";
			}
			else if (gameManager.CurrentlyInAroundTheWorld)
			{
				gameModeTitle.text = "Around the World";
			}
			else if (gameManager.CurrentlyInLaserShow)
			{
				gameModeTitle.text = "Laser Show";
			}
			else if (gameManager.CurrentlyInSmallBall)
			{
				gameModeTitle.text = "Small Ball";
			}
			else if (gameManager.CurrentlyInBataround)
			{
				gameModeTitle.text = "Bat Around";
			}
			else if (gameManager.CurrentlyInWalkOff)
			{
				gameModeTitle.text = "Walk Off";
			}
		}

		private void SetTeamLeadsText(int teamAScore, int TeamBScore)
		{
			if (BataroundGameManager.Instance.CurrentlyInBataround)
			{
				teamLeadsText.text = "N/A";
			}
			else
			{
				if (teamAScore == TeamBScore)
				{
					teamLeadsText.text = $"Match tied at {teamAScore} - {TeamBScore}";
				}
				else if (teamAScore > TeamBScore)
				{
					teamLeadsText.text = $"Team A leads {teamAScore} - {TeamBScore}";
				}
				else
				{
					teamLeadsText.text = $"Team B leads {teamAScore} - {TeamBScore}";
				}
			}
		}
	}
}

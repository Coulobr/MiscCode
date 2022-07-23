using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HitTrax.Bataround
{
	public class CurrentUserStatsPanel : Panel<CurrentUserStatsPanel>
	{
		public TextMeshProUGUI currentBatterName;
		public TextMeshProUGUI nextBatterName;
		public TextMeshProUGUI totalExp;
		public TextMeshProUGUI currentBatterScore;
		public ObjectPooler pooler;
		public GameObject nextUpContainer;
		private User currentUser;

		public Image currentBatterBackground;
		public Image nextBatterBackground;
		public Image infinitySymbol;

		public Color redTeamColor;
		public Color blueTeamColor;
		public Color ffaColor;

		protected override void OnOpened()
		{
			base.OnOpened();
			BataroundGameManager.Instance.OnHitterChanged += OnHitterChanged;
			BataroundGameManager.Instance.OnEndMinigame += OnEndMinigame;
			BataroundGameManager.Instance.OnUndoLastPlay += OnUndoLastPlay;
			Objects.Instance.natTrack().OnEnterPlay += OnEnterPlay;
			Objects.Instance.fielders().OnPlayReset += OnPlayReset;
			UpdatePanel(BataroundGameManager.Instance.CurrentMinigame.CurrentBatter);
		}

		protected override void OnClosed()
		{
			base.OnClosed();
			Objects.Instance.natTrack().OnEnterPlay -= OnEnterPlay;
			Objects.Instance.fielders().OnPlayReset -= OnPlayReset;
			BataroundGameManager.Instance.OnHitterChanged -= OnHitterChanged;
			BataroundGameManager.Instance.OnEndMinigame -= OnEndMinigame;
			BataroundGameManager.Instance.OnUndoLastPlay -= OnUndoLastPlay;
		}

		public override void SetupPanel()
		{
			base.SetupPanel();
		}

		public void UpdatePanel(User desiredUser)
		{
			ResetPanel();

			if (desiredUser == null)
			{
				Utility.LogError("Given user is null, cannot assign them to the panel");
				return;
			}

			currentUser = desiredUser;
			UpdatePanel();
		}

		public override void UpdatePanel()
		{
			base.UpdatePanel();

			if (currentUser == null)
			{
				ResetPanel();
				return;
			}

			var battingOrder = BataroundGameManager.Instance.CurrentBataroundGroup.PlayersLeftToBat;
			var nextBatter = battingOrder.Count > 1 ? battingOrder[1] : null;

			if (currentBatterName)
			{
				currentBatterName.text = $"{currentUser.screenName}";
			}

			if (totalExp)
			{
				totalExp.text = "EXP: +" + currentUser.BAM.TotalBataroundSessionEXP.ToString();
			}

			if (currentBatterScore)
			{
				if (BataroundGameManager.Instance.CurrentlyInLinas)
				{
					currentBatterScore.text = $"{currentUser.BAM.TotalLinasScore}";
				}
				else if (BataroundGameManager.Instance.CurrentlyInAroundTheWorld)
				{
					currentBatterScore.text = $"{currentUser.BAM.TotalATWScore}";
				}
				else if (BataroundGameManager.Instance.CurrentlyInLaserShow)
				{
					currentBatterScore.text = $"{currentUser.BAM.TotalLaserShowScore}";
				}
				else if (BataroundGameManager.Instance.CurrentlyInSmallBall)
				{
					currentBatterScore.text = $"{currentUser.BAM.TotalSmallBallScore}";
				}
				else if (BataroundGameManager.Instance.CurrentlyInWalkOff)
				{
					currentBatterScore.text = $"{currentUser.BAM.TotalWalkOffScore}";
				}
				else if (BataroundGameManager.Instance.CurrentlyInBataround && !BataroundGameManager.Instance.IsFreeForAll)
				{
					currentBatterScore.text = $"{currentUser.BAM.GetTotalAllGames()}";
				}
				else if (BataroundGameManager.Instance.CurrentlyInBataround && BataroundGameManager.Instance.IsFreeForAll)
				{
					currentBatterScore.text = $"{currentUser.BAM.TotalBataroundRoundsWon + currentUser.BAM.TotalBataroundMinigameScore}";
				}
			}

			if (nextUpContainer && nextBatterName)
			{
				if (nextBatter != null)
				{
					nextUpContainer.gameObject.SetActive(true);
					nextBatterName.text = nextBatter.screenName;
				}
				else
				{
					nextUpContainer.gameObject.SetActive(false);
				}
			}

			if (nextBatterBackground && currentBatterBackground)
			{
				if (!BataroundGameManager.Instance.IsFreeForAll)
				{
					currentBatterBackground.color = currentUser.BataroundTeam == 0 ? blueTeamColor : redTeamColor;

					if (nextBatter != null)
					{
						nextBatterBackground.color = nextBatter.BataroundTeam == 0 && nextBatter != null ? blueTeamColor : redTeamColor;
					}
				}
				else
				{
					currentBatterBackground.color = ffaColor;

					if (nextBatter != null)
					{
						nextBatterBackground.color = ffaColor;
					}
				}
			}

			if (pooler && !BataroundGameManager.Instance.CurrentlyInBataround)
			{
				if (infinitySymbol)
				{
					infinitySymbol.enabled = false;
				}

				pooler.ResetElement();
				pooler.SetupElement();

				if (BataroundGameManager.Instance.CurrentMinigame != null)
				{
					if (BataroundGameManager.Instance.CurrentlyInWalkOff)
					{
						for (int i = 0; i < BataroundGameManager.Instance.CurrentMinigame.MaxAttempts + currentUser.BAM.TotalBataroundBonusHits; i++)
						{
							pooler.ActivatePoolableObject(pooler.GetFreeObject());
						}

						//for (int i = 0; i < pooler.usedContainer.childCount; i++)
						//{
						//	var item = pooler.usedContainer.GetChild(i).GetComponent<BataroundAttemptPoolable>();
						//	item.ballImg.enabled = true;
						//}

						for (int i = pooler.usedContainer.childCount - 1; i > currentUser.BataroundAttemptsRemaining - 1; i--)
						{
							pooler.usedContainer.GetChild(i).gameObject.SetActive(false);
						}
					}
					else
					{
						for (int i = 0; i < BataroundGameManager.Instance.CurrentMinigame.MaxAttempts; i++)
						{
							pooler.ActivatePoolableObject(pooler.GetFreeObject());
						}

						//for (int i = 0; i < pooler.usedContainer.childCount; i++)
						//{
						//	var item = pooler.usedContainer.GetChild(i).GetComponent<BataroundAttemptPoolable>();
						//	item.ballImg.enabled = true;
						//}

						for (int i = pooler.usedContainer.childCount - 1; i > currentUser.BataroundAttemptsRemaining - 1; i--)
						{
							pooler.usedContainer.GetChild(i).gameObject.SetActive(false);
						}
					}
				}
			}
			else if (pooler && BataroundGameManager.Instance.CurrentlyInBataround)
			{
				pooler.ResetElement();
				if (infinitySymbol)
				{
					infinitySymbol.enabled = true;
				}
			}
		}

		public override void ResetPanel()
		{
			base.ResetPanel();

			currentUser = null;

			if (currentBatterName)
			{
				currentBatterName.text = "";
			}

			if (currentBatterScore)
			{
				currentBatterScore.text = "SCORE <size=350%>0</size>";
			}
		}

		private void OnUndoLastPlay()
		{
			UpdatePanel();
		}

		private void OnEndMinigame(BataroundMinigame obj)
		{
			Close();
		}

		private void OnEnterPlay(Play play)
		{
			if (!isOpen)
			{
				return;
			}

			UpdatePanel(play.user);
		}

		private void OnPlayReset(Play play)
		{
			if (play != null)
			{
				UpdatePanel(play.user);
			}
		}

		private void OnHitterChanged(User user)
		{
			UpdatePanel(user);
		}
	}
}


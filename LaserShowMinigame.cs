using System.Collections.Generic;
using UnityEngine;

namespace HitTrax.Bataround
{
	public class LaserShowMinigame : BataroundMinigame
	{
		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.F1))
			{
				EndMinigame();
			}
			if (Input.GetKeyDown(KeyCode.F2))
			{
				NextHitter();
			}
		}

		protected override void LoadSession()
		{
			if (GameManager.CurrentBataroundGroup?.Clinic == null || GameManager.CurrentBataroundGroup.Clinic.users.Count < 1)
			{
				Utility.LogError("Cannot start minigame, there is either no Bat Around Group or active Clinic!");
				BataroundSessionsManager.Instance.LoadSession<GameModeSelectSession>();
				return;
			}

			BataroundMenuManager.Instance.CloseAllMenus(); 
			base.LoadSession();

			GameManager.GameSessionActive = true;
			GameManager.CurrentlyInLaserShow = true;
			ShowFielders = false;
			MaxAttempts = 6;

			foreach (User user in GameManager.CurrentBataroundGroup.GameBattingOrder)
			{
				user.BAM.TotalLaserShowScore = 0;
				user.BAM.TotalBataroundBonusPoints = 0;
			}

			ThisGame = null;
			BataroundGameDirectionsUI.Open();
		}

		public override void UnloadSession()
		{
			base.UnloadSession();
			GameManager.CurrentlyInLaserShow = false;
		}

		public override void OnEnterPlay(Play play)
		{
			if (!GameManager.CurrentlyInLaserShow)
			{
				return;
			}

			if (InPlay)
			{
				return;
			}

			base.OnEnterPlay(play);
			CurrentBatter.BataroundAttemptsRemaining -= 1; 
			play.BatAroundMinigame = BatAroundMinigame.LaserShow;
			//CurrentBatter.stats.plays.Add(play);
			CurrentBatter.stats.Calculate();

			if (play != null && play.hasImpact)
			{
				ApplyPoints(play);

				CurrentBatter.BAM.TotalBataroundSessionEXP += (int)play.Points();

				if (CurrentBatter.BataroundAttemptsRemaining == 0)
				{
					ChangeHitterAfterPlay = true;
				}
				else
				{
					ChangeHitterAfterPlay = false;
				}
			}
		}

		public override void SetGameDifficulty(MinigameDifficulty difficulty)
		{
			switch (difficulty)
			{
				case MinigameDifficulty.Easy:
					MinVelocity = 50;
					BonusMphThreshold = float.Parse(Globals.FormatVel(Mathf.RoundToInt((float)CurrentBatter.stats.maxExitVel), false, false)) * .95f;
					break;
				case MinigameDifficulty.Medium:
					MinVelocity = 70;
					BonusMphThreshold = float.Parse(Globals.FormatVel(Mathf.RoundToInt((float)CurrentBatter.stats.maxExitVel), false, false)) * .9f;
					break;
				case MinigameDifficulty.Hard:
					MinVelocity = 90;
					BonusMphThreshold = float.Parse(Globals.FormatVel(Mathf.RoundToInt((float)CurrentBatter.stats.maxExitVel), false, false)) * .85f;
					break;
				default:
					break;
			}

			MinDistance = 0;
			MinLaunchAngle = 0;
			MaxLaunchAngle = 0;
		}

		protected override void NextHitter()
		{
			base.NextHitter();
		}

		private void ApplyPoints(Play play)
		{
			var maxVelo = CurrentBatter.stats.maxExitVel;

			if (float.Parse(Globals.FormatVel(play.exitBallVel.magnitude, false, false)) >= MinVelocity)
			{
				play.bataroundSuccessfulHit = true;
				OnLasered();
			}

			if (float.Parse(Globals.FormatVel(play.exitBallVel.magnitude, false, false)) >= BonusMphThreshold)
			{
				play.bataroundBonusHit = true;

				CurrentBatter.BAM.TotalBataroundBonusPoints += 1;
				GameManager.OnBonusProgress?.Invoke();

				if (CurrentBatter.BAM.TotalBataroundBonusPoints == 4)
				{
					CurrentBatter.BAM.TotalBataroundBonusHits += 1;
				}
			}
		}

		public void OnLasered()
		{

			CurrentBatter.BAM.TotalLaserShowScore += 1;
			CurrentBatter.BAM.TotalGameScore += 1;

			if (CurrentBatter.BataroundTeam == 0)
			{
				GameManager.CurrentBataroundGroup.TeamATotalScore += 1;
				GameManager.TeamAPoint?.Invoke();
			}
			else
			{
				GameManager.CurrentBataroundGroup.TeamBTotalScore += 1;
				GameManager.TeamBPoint?.Invoke();
			}
		}

		public override void EndMinigame()
		{
			base.EndMinigame();

			int teamALaserShowTotalScore = 0;
			int teamBLaserShowTotalScore = 0;
			if (!GameManager.IsFreeForAll)
			{
				foreach (User user in GameManager.CurrentBataroundGroup.Clinic.users)
				{
					if (user.BataroundTeam == 0)
					{
						teamALaserShowTotalScore += user.BAM.TotalLaserShowScore;
					}

					if (user.BataroundTeam == 1)
					{
						teamBLaserShowTotalScore += user.BAM.TotalLaserShowScore;
					}
				}

				if (teamALaserShowTotalScore > teamBLaserShowTotalScore)
				{
					GameManager.CurrentBataroundGroup.TeamATotalScore += 1;
				}
				else if (teamBLaserShowTotalScore > teamALaserShowTotalScore)
				{
					GameManager.CurrentBataroundGroup.TeamBTotalScore += 1;
				}
				else
				{
					Debug.Log("Tie");
				}
			}
			else
			{
				User winner = null;
				List<User> winners = new List<User>();
				foreach (User user in GameManager.CurrentBataroundGroup.Clinic.users)
				{
					if (winner == null)
					{
						winner = user;
					}
					else
					{
						if (user.BAM.TotalLaserShowScore > winner.BAM.TotalLaserShowScore)
						{
							winner = user;
						}
					}
				}

				foreach (User user in GameManager.CurrentBataroundGroup.Clinic.users)
				{
					winners.Add(winner);
					if (user.BAM.TotalLaserShowScore == winner.BAM.TotalLaserShowScore && user.id.MasterID != winner.id.MasterID)
					{
						winners.Add(user);
					}
				}

				foreach (User user in winners)
				{
					user.BAM.TotalBataroundRoundsWon += 1;
				}
			}

			if (GameManager.IsFreeForAll)
			{
				FadeTransitionUI.Open();
				FadeTransitionUI.Instance.Transition(1.5f, .5f, () =>
				{
					FFAEndMinigameUI.Open();
					LaserShowInGameUI.Close();
				});
			}
			else
			{
				FadeTransitionUI.Open();
				FadeTransitionUI.Instance.Transition(1.5f, .5f, () =>
				{
					TeamMinigameEndUI.Open();
					LaserShowInGameUI.Close();
				});
			}

			GameManager.OnEndMinigame?.Invoke(this);
		}
	}
}

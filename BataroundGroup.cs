using System;
using System.Collections.Generic;
using UnityEngine;

namespace HitTrax.Bataround
{
	public class BataroundGroup
	{
		public Clinic Clinic { get; set; } = new Clinic();

		// Batting Order Lists
		public List<User> GameBattingOrder { get; set; } = new List<User>();
		public List<User> AlreadyBattedList { get; set; } = new List<User>();
		public List<User> PlayersLeftToBat { get; set; } = new List<User>();

		/// <summary>
		/// Users in the bataround experience | Before put into a batting order
		/// </summary>
		public List<User> Participants { get; set; } = new List<User>();

		//public Stats TeamASessionStats { get; set; } = new Stats();
		//public Stats TeamBSessionStats { get; set; } = new Stats();
		public int TeamATotalScore { get; set; }
		public int TeamBTotalScore { get; set; }
		public int TeamARoundsWon { get; set; }
		public int TeamBRoundsWon { get; set; }
		public int TeamABataroundRuns { get; set; }
		public int TeamBBataroundRuns { get; set; }

		public BataroundGroup()
		{
			Clinic = new Clinic();
			GameBattingOrder = new List<User>();
			AlreadyBattedList = new List<User>();
			PlayersLeftToBat = new List<User>();
			Participants = new List<User>();
			//TeamASessionStats = new Stats();
			//TeamBSessionStats = new Stats();
		}

		public void ResetParticipants()
		{
			Participants.Clear();
		}

		public void ResetBattingOrder()
		{
			GameBattingOrder.Clear();
			AlreadyBattedList.Clear();
		}

		/// <summary>
		/// Called after a play is removed 
		/// </summary>
		public void RemovePlay(Play playToRemove, User user)
		{
			var gameManager = BataroundGameManager.Instance;

			// Add an attempt
			user.BataroundAttemptsRemaining += 1;

			// Remove the bonus if it was gained
			if (playToRemove.bataroundBonusHit)
			{
				user.BAM.TotalBataroundBonusPoints -= 1;
				Mathf.Clamp(user.BAM.TotalBataroundBonusPoints, 0, 10);
			}

			// Remove a point if one was scored
			if (playToRemove.bataroundSuccessfulHit)
			{
				user.BAM.TotalGameScore -= 1;

				if (gameManager.CurrentlyInLinas)
				{
					user.BAM.TotalLinasScore -= 1;
				}
				else if (gameManager.CurrentlyInAroundTheWorld)
				{
					user.BAM.TotalATWScore -= 1;
				}
				else if (gameManager.CurrentlyInLaserShow)
				{
					user.BAM.TotalLaserShowScore -= 1;
				}
				else if (gameManager.CurrentlyInSmallBall)
				{
					user.BAM.TotalSmallBallScore -= 1;
				}
				else if (gameManager.CurrentlyInBataround)
				{
					user.BAM.TotalBataroundMinigameScore -= 1;
				}

				// For a team game, remove the team point
				if (!gameManager.IsFreeForAll)
				{
					if (user.BataroundTeam == 0)
					{
						TeamATotalScore -= 1;
					}
					else if (user.BataroundTeam == 1)
					{
						TeamBTotalScore -= 1;
					}
				}

				var game = BataroundGameManager.Instance.CurrentMinigame.ThisGame;
				if (game != null)
				{
					game.RemoveLastPlay();
				}

				BataroundGameManager.Instance.CurrentMinigame.CurrentBatter.BAM.TotalBataroundSessionEXP -= (int)playToRemove.Points();

				Mathf.Clamp(user.BAM.TotalGameScore, 0, 1000);
				Mathf.Clamp(user.BAM.TotalLinasScore, 0, 1000);
				Mathf.Clamp(user.BAM.TotalATWScore, 0, 1000);
				Mathf.Clamp(user.BAM.TotalLaserShowScore, 0, 1000);
				Mathf.Clamp(user.BAM.TotalSmallBallScore, 0, 1000);
				Mathf.Clamp(user.BAM.TotalBataroundMinigameScore, 0, 1000);
				Mathf.Clamp(user.BAM.TotalWalkOffScore, 0, 1000);
			}
		}
	}
}

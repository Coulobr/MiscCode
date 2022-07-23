using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HitTrax;
using System;
using TMPro;

namespace HitTrax.Bataround {
	public class BataroundAroundTheWorldMinigame : BataroundMinigame
	{
		public bool bonus = true;
		public DangerZone DangerZone { get; set; }
		public DangerZone DangerZoneSprayColor { get; set; }
		public GameObject Inidcator { get; set; }
		public Vector2 ActiveSlice { get; set; } = new Vector2();
		public bool PullComplete { get; set; } = false;
		public bool CenterComplete { get; set; } = false;
		public bool OppoComplete { get; set; } = false;
		public bool PlaySuccessPull { get; set; }
		public bool PlaySuccessCenter { get; set; }
		public bool PlaySuccessOppo { get; set; }
		public bool InPlayReset { get; private set; }

		private int pullSideHits;
		private int centerFieldHits;
		private int oppoSideHits;

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
			GameManager.CurrentlyInAroundTheWorld = true;
			ShowFielders = false;
			MaxAttempts = 6;

			foreach (User user in GameManager.CurrentBataroundGroup.GameBattingOrder)
			{
				user.BAM.TotalATWScore = 0;
				user.BAM.TotalBataroundBonusPoints = 0;
			}

			ReadyHitIndicators();
			SetupDangerZone();

			BataroundGameDirectionsUI.Open();
		}

		public override void UnloadSession()
		{
			base.UnloadSession();
			GameManager.CurrentlyInAroundTheWorld = false;

			if (DangerZone != null && DangerZoneSprayColor != null)
			{
				Destroy(DangerZone.gameObject);
				Destroy(DangerZoneSprayColor.gameObject);
			}
			
			if (Inidcator != null)
			{
				Destroy(Inidcator);
			}
		}

		public override void SetGameDifficulty(MinigameDifficulty difficulty)
		{
			base.SetGameDifficulty(difficulty);
			MinLaunchAngle = 0;
			MaxLaunchAngle = 0;
			BonusMphThreshold = 0;
			MinDistance = 0;
			MinVelocity = 0;
		}

		public override void OnPlayReset(Play play)
		{
			if (InPlayReset)
			{
				return;
			}

			base.OnPlayReset(play);
			InPlayReset = true;

			if (PlaySuccessPull)
			{
				pullSideHits++;
				Inidcator.GetComponentInChildren<TextMeshPro>().text = pullSideHits.ToString();
				
				var colorL = pullSideHits == 1 ? DangerZone.ColorOptions.Orange : DangerZone.ColorOptions.White;
				DangerZone.SetColor(colorL, DangerZone.GrowthDirection.Right);

				var splashColorL = pullSideHits == 1 ? DangerZone.ColorOptions.BataroundOrangeSprayChart : DangerZone.ColorOptions.White;
				DangerZoneSprayColor.SetColor(splashColorL, DangerZone.GrowthDirection.Right);

				if (pullSideHits == 2)
				{
					PullComplete = true;
				}

				PlaySuccessPull = false;
			}
			else if (PlaySuccessCenter)
			{
				centerFieldHits++;
				Inidcator.GetComponentInChildren<TextMeshPro>().text = centerFieldHits.ToString();
				
				var colorL = centerFieldHits == 1 ? DangerZone.ColorOptions.Orange : DangerZone.ColorOptions.White;
				DangerZone.SetColor(colorL, DangerZone.GrowthDirection.Right);

				var splashColorL = centerFieldHits == 1 ? DangerZone.ColorOptions.BataroundOrangeSprayChart : DangerZone.ColorOptions.White;
				DangerZoneSprayColor.SetColor(splashColorL, DangerZone.GrowthDirection.Right);
			
				if (centerFieldHits == 2)
				{
					CenterComplete = true;
				}

				PlaySuccessCenter = false;
			}
			else if (PlaySuccessOppo)
			{
				oppoSideHits++;
				Inidcator.GetComponentInChildren<TextMeshPro>().text = oppoSideHits.ToString();

				var colorL = oppoSideHits == 1 ? DangerZone.ColorOptions.Orange : DangerZone.ColorOptions.White;
				DangerZone.SetColor(colorL, DangerZone.GrowthDirection.Right);

				var splashColorL = oppoSideHits == 1 ? DangerZone.ColorOptions.BataroundOrangeSprayChart : DangerZone.ColorOptions.White;
				DangerZoneSprayColor.SetColor(splashColorL, DangerZone.GrowthDirection.Right);
				
				if (oppoSideHits == 2)
				{
					if(CenterComplete && PullComplete)
					{
						bonus = true;
					}
				}

				PlaySuccessOppo = false;
			}

			if (CurrentBatter.BataroundAttemptsRemaining % 2 == 0)
			{
				SetupDangerZone();
				ReadyHitIndicators();
			}
		}

		public override void OnEnterPlay(Play play)
		{
			if (!GameManager.CurrentlyInAroundTheWorld) { return; }

			if (InPlay)
			{
				return;
			}

			if (!play.hasImpact)
			{
				return;
			}

			base.OnEnterPlay(play);

			InPlay = true;
			InPlayReset = false;

			play.BatAroundMinigame = BatAroundMinigame.AroundTheWorld;
			CurrentBatter.stats.plays.Add(play);
			CurrentBatter.stats.Calculate();
			CurrentBatter.BAM.TotalBataroundSessionEXP += (int)play.points;
			CurrentBatter.BataroundAttemptsRemaining -= 1;

			switch (CurrentBatter.BAM.MinigameDifficulty)
			{
				case MinigameDifficulty.Easy:

					if (CurrentBatter.BataroundAttemptsRemaining >= 4)
					{
						if (CurrentBatter.bats == Globals.BATS_LEFT)
						{
							ActiveSlice = new Vector2(0, 45);
						}
						else
						{
							ActiveSlice = new Vector2(-45, 0);
						}
						break;
					}
					else if (CurrentBatter.BataroundAttemptsRemaining >= 2)
					{
						ActiveSlice = new Vector2(-22.5f, 22.5f);
						break;
					}
					else if (CurrentBatter.BataroundAttemptsRemaining >= 0)
					{
						if (CurrentBatter.bats == Globals.BATS_LEFT)
						{
							ActiveSlice = new Vector2(-45, 0);
						}
						else
						{
							ActiveSlice = new Vector2(0, 45);
						}
						break;
					}
					break;
				case MinigameDifficulty.Medium:

					if (CurrentBatter.BataroundAttemptsRemaining >= 4)
					{
						if (CurrentBatter.bats == Globals.BATS_LEFT)
						{
							ActiveSlice = new Vector2(5, 45);
						}
						else
						{
							ActiveSlice = new Vector2(-45, -5);
						}
						break;
					}
					else if (CurrentBatter.BataroundAttemptsRemaining >= 2)
					{
						ActiveSlice = new Vector2(-20, 20);
						break;
					}
					else if (CurrentBatter.BataroundAttemptsRemaining >= 0)
					{
						if (CurrentBatter.bats == Globals.BATS_LEFT)
						{
							ActiveSlice = new Vector2(-45, -5);
						}
						else
						{
							ActiveSlice = new Vector2(5, 45);
						}
						break;
					}
					break;
				case MinigameDifficulty.Hard:

					if (CurrentBatter.BataroundAttemptsRemaining >= 4)
					{
						if (CurrentBatter.bats == Globals.BATS_LEFT)
						{
							ActiveSlice = new Vector2(10, 45);
						}
						else
						{
							ActiveSlice = new Vector2(-45, -10);
						}
						break;
					}
					else if (CurrentBatter.BataroundAttemptsRemaining >= 2)
					{
						ActiveSlice = new Vector2(-17.5f, 17.5f);
						break;
					}
					else if (CurrentBatter.BataroundAttemptsRemaining >= 0)
					{
						if (CurrentBatter.bats == Globals.BATS_LEFT)
						{
							ActiveSlice = new Vector2(-45, -10);
						}
						else
						{
							ActiveSlice = new Vector2(10, 45);
						}
						break;
					}
					break;
			}

			// In active zone
			if (play.horizontalAngle >= ActiveSlice.x && play.horizontalAngle <= ActiveSlice.y)
			{
				play.bataroundSuccessfulHit = true;
				play.bataroundBonusHit = true;

				CurrentBatter.BAM.TotalATWScore += 1;
				CurrentBatter.BAM.TotalGameScore += 1;
				CurrentBatter.BAM.TotalBataroundBonusPoints += 1;

				if (!GameManager.IsFreeForAll)
				{
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

				BataroundGameManager.Instance.OnSuccess?.Invoke();
				GameManager.OnBonusProgress?.Invoke();

				if (CurrentBatter.BataroundAttemptsRemaining >= 4)
				{
					PlaySuccessPull = true;
				}
				else if (CurrentBatter.BataroundAttemptsRemaining >= 2)
				{
					PlaySuccessCenter = true;
				}
				else if (CurrentBatter.BataroundAttemptsRemaining >= 0)
				{
					PlaySuccessOppo = true;
				}
			}

			//ApplyStraightFlushBonus(bonus);

			if (CurrentBatter.BataroundAttemptsRemaining == 0)
			{
				ChangeHitterAfterPlay = true;
			}
			else
			{
				ChangeHitterAfterPlay = false;
			}
		}
	
		protected override void NextHitter()
		{
			base.NextHitter();
			pullSideHits = 0;
			centerFieldHits = 0;
			oppoSideHits = 0;
			PullComplete = false;
			CenterComplete = false;
			OppoComplete = false;
			SetupDangerZone();
			ReadyHitIndicators();
		}

		private void SetupDangerZone()
		{
			var difficulty = CurrentBatter.BAM.MinigameDifficulty;
			float degreesPerSection = 0f;
			float leftOffset = 0;
			float centerOffset = 0;
			float rightOffset = 0;

			switch (difficulty)
			{
				case MinigameDifficulty.Easy:
					degreesPerSection = 45;
					leftOffset = 0;
					centerOffset = 22.5f;
					rightOffset = 45;
					break;
				case MinigameDifficulty.Medium:
					degreesPerSection = 40;
					leftOffset = 0;
					centerOffset = 25;
					rightOffset = 50;
					break;
				case MinigameDifficulty.Hard:
					degreesPerSection = 35;
					leftOffset = 0;
					centerOffset = 27.5f;
					rightOffset = 55;
					break;
			}

			if (DangerZone != null && DangerZoneSprayColor != null)
			{
				Destroy(DangerZone.gameObject);
				Destroy(DangerZoneSprayColor.gameObject);
			}

			var bats = CurrentBatter.bats;

			if (bats == Globals.BATS_RIGHT) // righty
			{
				if (CurrentBatter.BataroundAttemptsRemaining >= 5)
				{
					DangerZone = DangerZone.CreateDangerZone(OB.homePlate.transform, Objects.Instance.CenterFieldDistance(), DangerZone.GrowthDirection.Right);
					DangerZone.SetDegrees(degreesPerSection, leftOffset);
					DangerZone.SetColor(DangerZone.ColorOptions.BataroundGreen, DangerZone.GrowthDirection.Right);

					DangerZoneSprayColor = DangerZone.CreateDangerZone(OB.homePlate.transform, Objects.Instance.CenterFieldDistance(), DangerZone.GrowthDirection.Right);
					DangerZoneSprayColor.SetDegrees(degreesPerSection, leftOffset);
					DangerZoneSprayColor.SetColor(DangerZone.ColorOptions.BataroundGreenSprayChart, DangerZone.GrowthDirection.Right);
					DangerZoneSprayColor.gameObject.layer = LayerMask.NameToLayer("SprayChart");
					return;
				}
				else if (CurrentBatter.BataroundAttemptsRemaining >= 3)
				{
					DangerZone = DangerZone.CreateDangerZone(OB.homePlate.transform, Objects.Instance.CenterFieldDistance(), DangerZone.GrowthDirection.Right);
					DangerZone.SetDegrees(degreesPerSection, centerOffset);
					DangerZone.SetColor(DangerZone.ColorOptions.BataroundGreen, DangerZone.GrowthDirection.Right);

					DangerZoneSprayColor = DangerZone.CreateDangerZone(OB.homePlate.transform, Objects.Instance.CenterFieldDistance(), DangerZone.GrowthDirection.Right);
					DangerZoneSprayColor.SetDegrees(degreesPerSection, centerOffset);
					DangerZoneSprayColor.SetColor(DangerZone.ColorOptions.BataroundGreenSprayChart, DangerZone.GrowthDirection.Right);
					DangerZoneSprayColor.gameObject.layer = LayerMask.NameToLayer("SprayChart");
					return;
				}
				else if (CurrentBatter.BataroundAttemptsRemaining >= 1)
				{
					DangerZone = DangerZone.CreateDangerZone(OB.homePlate.transform, Objects.Instance.CenterFieldDistance(), DangerZone.GrowthDirection.Right);
					DangerZone.SetDegrees(degreesPerSection, rightOffset);
					DangerZone.SetColor(DangerZone.ColorOptions.BataroundGreen, DangerZone.GrowthDirection.Right);

					DangerZoneSprayColor = DangerZone.CreateDangerZone(OB.homePlate.transform, Objects.Instance.CenterFieldDistance(), DangerZone.GrowthDirection.Right);
					DangerZoneSprayColor.SetDegrees(degreesPerSection, rightOffset);
					DangerZoneSprayColor.SetColor(DangerZone.ColorOptions.BataroundGreenSprayChart, DangerZone.GrowthDirection.Right);
					DangerZoneSprayColor.gameObject.layer = LayerMask.NameToLayer("SprayChart");
					return;
				}
			}
			else if (bats == Globals.BATS_LEFT) // left
			{
				if (CurrentBatter.BataroundAttemptsRemaining >= 5)
				{
					DangerZone = DangerZone.CreateDangerZone(OB.homePlate.transform, Objects.Instance.CenterFieldDistance(), DangerZone.GrowthDirection.Right);
					DangerZone.SetDegrees(degreesPerSection, rightOffset);
					DangerZone.SetColor(DangerZone.ColorOptions.BataroundGreen, DangerZone.GrowthDirection.Right);

					DangerZoneSprayColor = DangerZone.CreateDangerZone(OB.homePlate.transform, Objects.Instance.CenterFieldDistance(), DangerZone.GrowthDirection.Right);
					DangerZoneSprayColor.SetDegrees(degreesPerSection, rightOffset);
					DangerZoneSprayColor.SetColor(DangerZone.ColorOptions.BataroundGreenSprayChart, DangerZone.GrowthDirection.Right);
					DangerZoneSprayColor.gameObject.layer = LayerMask.NameToLayer("SprayChart");
					return;
				}
				else if (CurrentBatter.BataroundAttemptsRemaining >= 3)
				{
					DangerZone = DangerZone.CreateDangerZone(OB.homePlate.transform, Objects.Instance.CenterFieldDistance(), DangerZone.GrowthDirection.Right);
					DangerZone.SetDegrees(degreesPerSection, centerOffset);
					DangerZone.SetColor(DangerZone.ColorOptions.BataroundGreen, DangerZone.GrowthDirection.Right);

					DangerZoneSprayColor = DangerZone.CreateDangerZone(OB.homePlate.transform, Objects.Instance.CenterFieldDistance(), DangerZone.GrowthDirection.Right);
					DangerZoneSprayColor.SetDegrees(degreesPerSection, centerOffset);
					DangerZoneSprayColor.SetColor(DangerZone.ColorOptions.BataroundGreenSprayChart, DangerZone.GrowthDirection.Right);
					DangerZoneSprayColor.gameObject.layer = LayerMask.NameToLayer("SprayChart");
					return;
				}
				else if (CurrentBatter.BataroundAttemptsRemaining >= 1)
				{
					DangerZone = DangerZone.CreateDangerZone(OB.homePlate.transform, Objects.Instance.CenterFieldDistance(), DangerZone.GrowthDirection.Right);
					DangerZone.SetDegrees(degreesPerSection, leftOffset);
					DangerZone.SetColor(DangerZone.ColorOptions.BataroundGreen, DangerZone.GrowthDirection.Right);

					DangerZoneSprayColor = DangerZone.CreateDangerZone(OB.homePlate.transform, Objects.Instance.CenterFieldDistance(), DangerZone.GrowthDirection.Right);
					DangerZoneSprayColor.SetDegrees(degreesPerSection, leftOffset);
					DangerZoneSprayColor.SetColor(DangerZone.ColorOptions.BataroundGreenSprayChart, DangerZone.GrowthDirection.Right);
					DangerZoneSprayColor.gameObject.layer = LayerMask.NameToLayer("SprayChart");
					return;
				}
			}
		}

		private void ReadyHitIndicators()
		{
			if (Inidcator != null)
			{
				Destroy(Inidcator);
			}		

			if (CurrentBatter.BataroundAttemptsRemaining >= 5)
			{
				if (CurrentBatter.bats == Globals.BATS_RIGHT) 
				{
					Inidcator = Instantiate(Resources.Load<GameObject>("ATWIndicator"));
					Inidcator.transform.position = (OB.baseContainerController().GetBase(3).position + OB.baseContainerController().GetBase(5).position) / 2;
					Inidcator.transform.position = new Vector3(Inidcator.transform.position.x, .6f, Inidcator.transform.position.z);
					Inidcator.GetComponentInChildren<TextMeshPro>().text = "2";
				}
				else
				{
					Inidcator = Instantiate(Resources.Load<GameObject>("ATWIndicator"));
					Inidcator.transform.position = (OB.baseContainerController().GetBase(5).position + OB.baseContainerController().GetBase(1).position) / 2;
					Inidcator.transform.position = new Vector3(Inidcator.transform.position.x, .6f, Inidcator.transform.position.z);
					Inidcator.GetComponentInChildren<TextMeshPro>().text = "2";
				}
			}
			else if (CurrentBatter.BataroundAttemptsRemaining >= 3)
			{
				Inidcator = Instantiate(Resources.Load<GameObject>("ATWIndicator"));
				Inidcator.transform.position = (OB.baseContainerController().GetBase(5).position + OB.baseContainerController().GetBase(2).position) / 2;
				Inidcator.transform.position = new Vector3(Inidcator.transform.position.x, .6f, Inidcator.transform.position.z);
				Inidcator.GetComponentInChildren<TextMeshPro>().text = "2";
			}
			else if (CurrentBatter.BataroundAttemptsRemaining >= 1)
			{
				if (CurrentBatter.bats == Globals.BATS_RIGHT)
				{
					Inidcator = Instantiate(Resources.Load<GameObject>("ATWIndicator"));
					Inidcator.transform.position = (OB.baseContainerController().GetBase(5).position + OB.baseContainerController().GetBase(1).position) / 2;
					Inidcator.transform.position = new Vector3(Inidcator.transform.position.x, .6f, Inidcator.transform.position.z);
					Inidcator.GetComponentInChildren<TextMeshPro>().text = "2";
				}
				else
				{
					Inidcator = Instantiate(Resources.Load<GameObject>("ATWIndicator"));
					Inidcator.transform.position = (OB.baseContainerController().GetBase(3).position + OB.baseContainerController().GetBase(5).position) / 2;
					Inidcator.transform.position = new Vector3(Inidcator.transform.position.x, .6f, Inidcator.transform.position.z);
					Inidcator.GetComponentInChildren<TextMeshPro>().text = "2";				
				}
			}
		}

		public override void EndMinigame()
		{
			//TODO: Intermission
			base.EndMinigame();


			if (Inidcator != null)
			{
				Destroy(Inidcator);
			}

			if (DangerZone != null && DangerZoneSprayColor != null)
			{
				Destroy(DangerZone.gameObject);
				Destroy(DangerZoneSprayColor.gameObject);
			}

			int teamAATWTotalScore = 0;
			int teamBATWTotalScore = 0;
			if (!GameManager.IsFreeForAll)
			{
				foreach (User user in GameManager.CurrentBataroundGroup.Clinic.users)
				{
					if (user.BataroundTeam == 0)
					{
						teamAATWTotalScore += user.BAM.TotalATWScore;
					}

					if (user.BataroundTeam == 1)
					{
						teamBATWTotalScore += user.BAM.TotalATWScore;
					}
				}

				if (teamAATWTotalScore > teamBATWTotalScore)
				{
					GameManager.CurrentBataroundGroup.TeamARoundsWon += 1;
				}
				else if (teamBATWTotalScore > teamAATWTotalScore)
				{
					GameManager.CurrentBataroundGroup.TeamBRoundsWon += 1;
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
						if (user.BAM.TotalATWScore > winner.BAM.TotalATWScore)
						{
							winner = user;
						}
					}
				}

				foreach (User user in GameManager.CurrentBataroundGroup.Clinic.users)
				{
					winners.Add(winner);
					if (user.BAM.TotalATWScore == winner.BAM.TotalATWScore && user.id.MasterID != winner.id.MasterID)
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
				FadeTransitionUI.Instance.Transition(1.5f, .5f, () => {
					FFAEndMinigameUI.Open();
					AroundTheWorldInGameUI.Close();
				});
			}
			else
			{
				FadeTransitionUI.Open();
				FadeTransitionUI.Instance.Transition(1.5f, .5f, () => {
					TeamMinigameEndUI.Open();
					AroundTheWorldInGameUI.Close();
				});
			}

			GameManager.OnEndMinigame?.Invoke(this);
		}
	}
}

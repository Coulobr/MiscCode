using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HitTrax.Bataround
{
	public class SmallBallMinigame : BataroundMinigame
	{
		public static Action<HittingDrill> SituationChanged;

		private static HittingDrill manOnSecondDrill;
		public static HittingDrill ManOnSecondDrill
		{
			get
			{
				if (manOnSecondDrill == null)
				{
					manOnSecondDrill = new HittingDrill(DrillObjective.SituationalHitting);
					manOnSecondDrill.situation = new DrillSituation(DrillSituationId.BA_SmallBall_RunnerOnSecond);
					manOnSecondDrill.minHits = 100;
					manOnSecondDrill.maxHits = 100;
					manOnSecondDrill.description = "Man on second, no outs | score a run!";
				}
				return manOnSecondDrill;
			}
		}

		private static HittingDrill manOnThirdFieldersOut;
		public static HittingDrill ManOnThirdFieldersOut
		{
			get
			{
				if (manOnThirdFieldersOut == null)
				{
					manOnThirdFieldersOut = new HittingDrill(DrillObjective.SituationalHitting);
					manOnThirdFieldersOut.situation = new DrillSituation(DrillSituationId.BA_SmallBall_RunnerOnThird_InfieldOut);
					manOnThirdFieldersOut.minHits = 100;
					manOnThirdFieldersOut.maxHits = 100;
					manOnThirdFieldersOut.description = "Man on third, fileders out, no outs | score a run!";
				}
				return manOnThirdFieldersOut;
			}
		}

		private static HittingDrill manOnThirdFieldersIn;
		public static HittingDrill ManOnThirdFieldersIn
		{
			get
			{
				if (manOnThirdFieldersIn == null)
				{
					manOnThirdFieldersIn = new HittingDrill(DrillObjective.SituationalHitting);
					manOnThirdFieldersIn.situation = new DrillSituation(DrillSituationId.BA_SmallBall_RunnerOnThird_InfieldIn);
					manOnThirdFieldersIn.minHits = 100;
					manOnThirdFieldersIn.maxHits = 100;
					manOnThirdFieldersIn.description = "Man on third, fileders in, no outs | score a run!";
				}
				return manOnThirdFieldersIn;
			}
		}

		public HittingDrill CurrentDrill { get; set; }

		public enum CurrentSituation
		{
			SecondBase,
			ThirdBase_FieldIn,
			ThirdBase_FieldOut,
		}
		private CurrentSituation CurSituation { get; set; }


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

			OB.state.mode = Globals.MODE_SITUATIONAL_HITTING;
			GameManager.GameSessionActive = true;
			GameManager.CurrentlyInSmallBall = true;
			ShowFielders = true;
			MaxAttempts = 6;

			BataroundGameManager.Instance.OnHitterChanged += OnHitterChanged;

			foreach (User user in GameManager.CurrentBataroundGroup.GameBattingOrder)
			{
				user.BAM.TotalSmallBallScore = 0;
				user.BAM.TotalBataroundBonusPoints = 0;

				user.situationalHittingProfile = CreateSituationalHittingProfile();
				user.clinicStatsHitting.type = Stats.TYPE_DRILL;
				user.clinicStatsHitting.drillSituationId = DrillSituationId.BA_SmallBall_RunnerOnSecond;
				SetupUser(user);
			}

			OB.state.SetHittingSession(CurrentBatter.stats);
			OB.camReset();
			ThisGame = SetDrillGameScenario(ManOnSecondDrill, CurrentBatter);
			GameManager.OnUpdateBaseDisplayPanel?.Invoke();

			BataroundGameDirectionsUI.Open();
		}

		public override void UnloadSession()
		{
			base.UnloadSession();
			GameManager.CurrentlyInSmallBall = false; 
			BataroundGameManager.Instance.OnHitterChanged -= OnHitterChanged;
		}

		private void OnDestroy()
		{
			
		}

		public override void OnEnterPlay(Play play)
		{
			if (play != null && play.hasImpact)
			{
				base.OnEnterPlay(play);
				CurrentBatter.BataroundAttemptsRemaining -= 1;
				play.gameType = new GameType(SportType.Baseball, ThisGame.type);

				switch (CurSituation)
				{
					case CurrentSituation.SecondBase:
						play.gameType.infieldConfiguration = InfieldDefenceConfiguration.Standard;
						break;
					case CurrentSituation.ThirdBase_FieldIn:
						play.gameType.infieldConfiguration = InfieldDefenceConfiguration.In;
						break;
					case CurrentSituation.ThirdBase_FieldOut:
						play.gameType.infieldConfiguration = InfieldDefenceConfiguration.Out;
						break;
				}
			}
		}

		public override void OnPlayReset(Play play)
		{
			base.OnPlayReset(play);
		}

		public override void UpdateDrill(Play play)
		{
			base.UpdateDrill(play);
			if (!GameManager.CurrentlyInSmallBall)
			{
				return;
			}

			play.BatAroundMinigame = BatAroundMinigame.SmallBall;
			CurrentBatter.stats.plays.Add(play);
			CurrentBatter.stats.Calculate();
			CurrentDrill.EnterPlay(play);
			ThisGame.EnterPlay(play);
			CurrentBatter.stats.atbats = CurrentDrill.atBats.Count;
			ScorePlay(play);

			switch (CurrentBatter.BataroundAttemptsRemaining)
			{
				case 6:
					ChangeHitterAfterPlay = false;
					StartCoroutine(StartNextAttemptCoroutine(ManOnSecondDrill, CurrentBatter));
					break;
				case 5:
					ChangeHitterAfterPlay = false;
					StartCoroutine(StartNextAttemptCoroutine(ManOnSecondDrill, CurrentBatter));
					break;
				case 4:
					ChangeHitterAfterPlay = false;
					StartCoroutine(StartNextAttemptCoroutine(ManOnThirdFieldersOut, CurrentBatter));
					SituationChanged?.Invoke(ManOnThirdFieldersOut);
					break;
				case 3:
					ChangeHitterAfterPlay = false;
					StartCoroutine(StartNextAttemptCoroutine(ManOnThirdFieldersOut, CurrentBatter));
					break;
				case 2:
					ChangeHitterAfterPlay = false;
					StartCoroutine(StartNextAttemptCoroutine(ManOnThirdFieldersIn, CurrentBatter));
					SituationChanged?.Invoke(ManOnThirdFieldersIn);
					break;
				case 1:
					ChangeHitterAfterPlay = false;
					StartCoroutine(StartNextAttemptCoroutine(ManOnThirdFieldersIn, CurrentBatter));
					break;
				case 0:
					ChangeHitterAfterPlay = true;
					break;
				default:
					break;
			}
		}

		public override void SetGameDifficulty(MinigameDifficulty difficulty)
		{
			base.SetGameDifficulty(difficulty);
			switch (difficulty)
			{
				case MinigameDifficulty.Easy:
					MinLaunchAngle = 0;
					MaxLaunchAngle = 90;
					BonusMphThreshold = 1000;
					MinDistance = 0;
					MinVelocity = 0;
					break;
				case MinigameDifficulty.Medium:
					MinLaunchAngle = 0;
					MaxLaunchAngle = 90;
					BonusMphThreshold = 1000;
					MinDistance = 0;
					MinVelocity = 0;
					break;
				case MinigameDifficulty.Hard:
					MinLaunchAngle = 0;
					MaxLaunchAngle = 90;
					BonusMphThreshold = 1000;
					MinDistance = 0;
					MinVelocity = 0;
					break;
				default:
					break;
			}
		}

		protected override void NextHitter()
		{
			base.NextHitter();
		}

		private void OnHitterChanged(User user)
		{
			StartCoroutine(StartNextAttemptCoroutine(ManOnSecondDrill, user));
		}

		private HittingDrillProfile CreateSituationalHittingProfile()
		{
			var profile = new HittingDrillProfile(DrillCategory.Drill);
			profile.AddDrill(ManOnSecondDrill);
			profile.AddDrill(ManOnThirdFieldersIn);
			profile.AddDrill(ManOnThirdFieldersOut);
			return profile;
		}

		private void SetupUser(User user)
		{
			user.stats.drillId.Copy(ManOnSecondDrill.id);
			user.stats.drillProfileId.Copy(user.situationalHittingProfile.id);
			user.stats.drillIndex = ManOnSecondDrill.index;
			user.stats.drillSituationId = ManOnSecondDrill.situationId;
			user.stats.tag = ManOnSecondDrill.name;
			user.stats.subType = (int)ManOnSecondDrill.objective;
			user.stats.CopyStatsFromUser(user);
			user.stats.stadium = OB.stadiumController.currentStadium;
		}

		private IEnumerator StartNextAttemptCoroutine(HittingDrill drill, User hitter)
		{
			while (this.enabled && hitter != null && !hitter.situationalHittingProfile.finished && drill.objective.HasRunners() && OB.fielders().animationActive)
			{
				yield return new WaitForSeconds(.1f);
			}

			ThisGame = SetDrillGameScenario(drill, hitter);
			GameManager.OnUpdateBaseDisplayPanel?.Invoke();
			yield return null;
		}

		public Game SetDrillGameScenario(HittingDrill drill, User hitter, Game game = null)
		{
			if (game == null)
			{
				Team awayTeam = new Team("Away");
				awayTeam.gameType.Copy(hitter.gameType);
				Team homeTeam = new Team("Home");
				homeTeam.gameType.Copy(hitter.gameType);

				for (int i = 0; i < 3; i++)
				{
					Player p = new Player("Player");
					awayTeam.AddPlayer(p, i);
					p.id.id = p.battingOrder;

					p = new Player("Player");
					homeTeam.AddPlayer(p, i);
					p.id.id = 12 + p.battingOrder;
				}
				game = new Game(homeTeam, awayTeam);
				game.type = Game.DEMO;
				game.stadium = OB.stadiumController.currentStadium;
				game.playInnings = 3;
				game.maxOuts = 3;
				game.pitchRule = -1;
				game.Start(false);
			}

			if (drill.situation != null)
			{
				int outs = drill.situation.outs - (drill.situation.outsLessThan ? 1 : 0);
				for (int i = 0; i < outs; i++)
				{
					PutManOnBase(game, Globals.RESULT_OUT);
				}
				if (drill.situation.onBase[1] && drill.situation.onBase[2] && drill.situation.onBase[3])
				{
					PutManOnBase(game, Globals.RESULT_HBP);
					PutManOnBase(game, Globals.RESULT_HBP);
					PutManOnBase(game, Globals.RESULT_HBP);
				}
				else if (!drill.situation.onBase[1] && !drill.situation.onBase[2] && drill.situation.onBase[3])
				{
					PutManOnBase(game, Globals.RESULT_HBP);
					PutManOnBase(game, Globals.RESULT_ADVANCE_RUNNERS);
					PutManOnBase(game, Globals.RESULT_ADVANCE_RUNNERS);
				}
				else if (!drill.situation.onBase[1] && drill.situation.onBase[2] && drill.situation.onBase[3])
				{
					PutManOnBase(game, Globals.RESULT_HBP);
					PutManOnBase(game, Globals.RESULT_HBP);
					PutManOnBase(game, Globals.RESULT_ADVANCE_RUNNERS);
				}
				else if (drill.situation.onBase[1] && drill.situation.onBase[2] && !drill.situation.onBase[3])
				{
					PutManOnBase(game, Globals.RESULT_HBP);
					PutManOnBase(game, Globals.RESULT_HBP);
				}
				else if (!drill.situation.onBase[1] && drill.situation.onBase[2] && !drill.situation.onBase[3])
				{
					PutManOnBase(game, Globals.RESULT_HBP);
					PutManOnBase(game, Globals.RESULT_ADVANCE_RUNNERS);
				}
				else if (drill.situation.onBase[1] && !drill.situation.onBase[2] && !drill.situation.onBase[3])
				{
					PutManOnBase(game, Globals.RESULT_HBP);
				}
			}

			switch (drill.situationId)
			{
				case DrillSituationId.BA_SmallBall_RunnerOnSecond:
					CurSituation = CurrentSituation.SecondBase;
					game.currentHalf.gameType.infieldConfiguration = InfieldDefenceConfiguration.Standard;
					break;
				case DrillSituationId.BA_SmallBall_RunnerOnThird_InfieldIn:
					CurSituation = CurrentSituation.ThirdBase_FieldIn;
					game.currentHalf.gameType.infieldConfiguration = InfieldDefenceConfiguration.In;
					break;
				case DrillSituationId.BA_SmallBall_RunnerOnThird_InfieldOut:
					CurSituation = CurrentSituation.ThirdBase_FieldOut;
					game.currentHalf.gameType.infieldConfiguration = InfieldDefenceConfiguration.Out;
					break;
			}

			CurrentDrill = drill;

			OB.state.SetRecordingGame(game);
			OB.natTrack().SetGame(game);
			OB.fielders().SetGame(game);
			OB.fielders().Reset(game.currentHalf.gameType, game.currentPlayer.user, true);

			QualitySettings.antiAliasing = 4;
			return game;
		}

		public void ScorePlay(Play play)
		{
			if (ThisGame.atBatFromPlay(play).rbis > 0)
			{
				play.bataroundSuccessfulHit = true;

				CurrentBatter.BAM.TotalGameScore += 1;
				CurrentBatter.BAM.TotalSmallBallScore += 1;
				GameManager.OnSuccess?.Invoke();

				if (CurrentBatter.BataroundTeam == 0 && !GameManager.IsFreeForAll)
				{
					GameManager.CurrentBataroundGroup.TeamATotalScore += 1;
					GameManager.TeamAPoint?.Invoke();
				}
				else if (CurrentBatter.BataroundTeam == 1 && !GameManager.IsFreeForAll)
				{
					GameManager.CurrentBataroundGroup.TeamBTotalScore += 1;
					GameManager.TeamBPoint?.Invoke();
				}

				// Base hit
				if (play.fplay.resultAtBat[0] >= 1 || play.result == Globals.RESULT_HOME)
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
		}

		private void PutManOnBase(Game game, int result)
		{
			Play play = OB.license().NewPlay(OB.natTrack().strikeZone, game);
			play.result = result;
			game.EnterPlay(play);
		}

		public override void EndMinigame()
		{
			//TODO: Intermission
			base.EndMinigame();

			int teamASmallBallTotalScore = 0;
			int teamBSmallBallTotalScore = 0;
			if (!GameManager.IsFreeForAll)
			{
				foreach (User user in GameManager.CurrentBataroundGroup.Clinic.users)
				{
					if (user.BataroundTeam == 0)
					{
						teamASmallBallTotalScore += user.BAM.TotalSmallBallScore;
					}

					if (user.BataroundTeam == 1)
					{
						teamBSmallBallTotalScore += user.BAM.TotalSmallBallScore;
					}
				}

				if (teamASmallBallTotalScore > teamBSmallBallTotalScore)
				{
					GameManager.CurrentBataroundGroup.TeamARoundsWon += 1;
				}
				else if (teamBSmallBallTotalScore > teamASmallBallTotalScore)
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
						if (user.BAM.TotalSmallBallScore > winner.BAM.TotalSmallBallScore)
						{
							winner = user;
						}
					}
				}

				foreach (User user in GameManager.CurrentBataroundGroup.Clinic.users)
				{
					winners.Add(winner);
					if (user.BAM.TotalSmallBallScore == winner.BAM.TotalSmallBallScore && user.id.MasterID != winner.id.MasterID)
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
					BarrelsInGameUI.Close();
				});
			}
			else
			{
				FadeTransitionUI.Open();
				FadeTransitionUI.Instance.Transition(1.5f, .5f, () =>
				{
					TeamMinigameEndUI.Open();
					BarrelsInGameUI.Close();
				});
			}

			BataroundGameManager.Instance.OnHitterChanged -= OnHitterChanged;
			GameManager.OnEndMinigame?.Invoke(this);
		}
	}
}

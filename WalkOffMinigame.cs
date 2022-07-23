using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HitTrax.Bataround
{
	public class WalkOffMinigame : BataroundMinigame
	{
		private static HittingDrill drill;
		public static HittingDrill Drill
		{
			get
			{
				if (drill == null)
				{
					drill = new HittingDrill(DrillObjective.SituationalHitting);
					drill.situation = new DrillSituation(DrillSituationId.BA_WalkOff_BasesLoaded_2_outs);
					drill.minHits = 100;
					drill.maxHits = 100;
					drill.description = "Bottom of 9th, Bases Loaded, 2 Outs | Score a run!";
				}
				return drill;
			}
		}

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
			GameManager.CurrentlyInWalkOff = true;
			ShowFielders = true;
			MaxAttempts = 6;

			BataroundGameManager.Instance.OnHitterChanged += OnHitterChanged;

			foreach (User user in GameManager.CurrentBataroundGroup.GameBattingOrder)
			{
				user.BAM.TotalWalkOffScore = 0;
				user.BAM.TotalBataroundBonusPoints = 0;
				user.BataroundAttemptsRemaining += user.BAM.TotalBataroundBonusHits;

				user.situationalHittingProfile = CreateSituationalHittingProfile();
				user.clinicStatsHitting.type = Stats.TYPE_DRILL;
				user.clinicStatsHitting.drillSituationId = DrillSituationId.BA_WalkOff_BasesLoaded_2_outs;
				SetupUser(user);
			}

			OB.strikeZoneDisplay().gameObject.SetActive(false);
			OB.state.SetHittingSession(CurrentBatter.stats);
			OB.camReset();
			ThisGame = SetDrillGameScenario(Drill, CurrentBatter);

			BataroundGameDirectionsUI.Open();
		}

		public override void UnloadSession()
		{
			base.UnloadSession();
			GameManager.CurrentlyInWalkOff = false;
		}

		private HittingDrillProfile CreateSituationalHittingProfile()
		{
			var profile = new HittingDrillProfile(DrillCategory.Drill);
			profile.AddDrill(Drill);
			return profile;
		}

		public override void LoadStadium()
		{
			switch (CurrentBatter.BAM.MinigameDifficulty)
			{
				case MinigameDifficulty.Easy:
					OB.SetStadium(Stadium.BAT_AROUND_NEW, new GameType(SportType.Baseball, GameType.LEVEL_BASEBALL_12U), CurrentBatter);
					break;
				case MinigameDifficulty.Medium:
					OB.SetStadium(Stadium.BAT_AROUND_NEW, new GameType(SportType.Baseball, GameType.LEVEL_BASEBALL_HIGH_SCHOOL), CurrentBatter);
					break;
				case MinigameDifficulty.Hard:
					OB.SetStadium(Stadium.BAT_AROUND_NEW, new GameType(SportType.Baseball, GameType.LEVEL_BASEBALL_MAJOR), CurrentBatter);
					break;
			}
		}

		private void SetupUser(User user)
		{
			user.stats.drillId.Copy(Drill.id);
			user.stats.drillProfileId.Copy(user.situationalHittingProfile.id);
			user.stats.drillIndex = Drill.index;
			user.stats.drillSituationId = Drill.situationId;
			user.stats.tag = Drill.name;
			user.stats.subType = (int)Drill.objective;
			user.stats.CopyStatsFromUser(user);
			user.stats.stadium = OB.stadiumController.currentStadium;
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
				switch (CurrentBatter.BAM.MinigameDifficulty)
				{
					case MinigameDifficulty.Easy:
						game.type = GameType.LEVEL_BASEBALL_12U;
						break;
					case MinigameDifficulty.Medium:
						game.type = GameType.LEVEL_BASEBALL_HIGH_SCHOOL;
						break;
					case MinigameDifficulty.Hard:
						game.type = GameType.LEVEL_BASEBALL_MAJOR;
						break;
				}
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

			OB.state.SetRecordingGame(game);
			OB.natTrack().SetGame(game);
			OB.fielders().SetGame(game);
			OB.fielders().Reset(new GameType(SportType.Baseball, game.type), game.currentPlayer.user, true);
			QualitySettings.antiAliasing = 4;
			return game;
		}

		private void PutManOnBase(Game game, int result)
		{
			Play play = OB.license().NewPlay(OB.natTrack().strikeZone, game);
			play.result = result;
			game.EnterPlay(play);
		}

		public override void OnEnterPlay(Play play)
		{
			if (play != null && play.hasImpact)
			{
				base.OnEnterPlay(play);
				play.gameType = new GameType(SportType.Baseball, ThisGame.type);
				CurrentBatter.BataroundAttemptsRemaining -= 1;
			}
		}

		public override void OnPlayReset(Play play)
		{
			base.OnPlayReset(play);
		}

		public override void UpdateDrill(Play play)
		{
			base.UpdateDrill(play);
			if (!GameManager.CurrentlyInWalkOff)
			{
				return;
			}

			play.BatAroundMinigame = BatAroundMinigame.WalkOff;
			Drill.EnterPlay(play);
			ThisGame.EnterPlay(play);
			CurrentBatter.stats.Calculate();
			CurrentBatter.stats.atbats = Drill.atBats.Count;
			ScorePlay(play);

			switch (CurrentBatter.BataroundAttemptsRemaining)
			{
				case 0:
					ChangeHitterAfterPlay = true;
					break;
				default:
					StartCoroutine(StartNextAttemptCoroutine(Drill, CurrentBatter));
					break;
			}
		}

		public void ScorePlay(Play play)
		{
			if (ThisGame.atBatFromPlay(play).rbis > 0)
			{
				play.bataroundSuccessfulHit = true;

				CurrentBatter.BAM.TotalGameScore += 1;
				CurrentBatter.BAM.TotalWalkOffScore += 1;
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
			}
		}

		private void OnHitterChanged(User user)
		{
			StartCoroutine(StartNextAttemptCoroutine(Drill, user));
		}

		private IEnumerator StartNextAttemptCoroutine(HittingDrill drill, User hitter)
		{
			while (this.enabled && hitter != null && !hitter.situationalHittingProfile.finished && drill.objective.HasRunners() && OB.fielders().animationActive)
			{
				yield return new WaitForSeconds(.1f);
			}

			ThisGame = SetDrillGameScenario(drill, hitter);
			yield return null;
		}

		public override void EndMinigame()
		{
			//TODO: Intermission
			base.EndMinigame();

			int teamAWalkOffTotalScore = 0;
			int teamBWalkOffTotalScore = 0;
			if (!GameManager.IsFreeForAll)
			{
				foreach (User user in GameManager.CurrentBataroundGroup.Clinic.users)
				{
					if (user.BataroundTeam == 0)
					{
						teamAWalkOffTotalScore += user.BAM.TotalWalkOffScore;
					}

					if (user.BataroundTeam == 1)
					{
						teamBWalkOffTotalScore += user.BAM.TotalWalkOffScore;
					}
				}

				if (teamAWalkOffTotalScore > teamBWalkOffTotalScore)
				{
					GameManager.CurrentBataroundGroup.TeamARoundsWon += 1;
				}
				else if (teamBWalkOffTotalScore > teamAWalkOffTotalScore)
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
						if (user.BAM.TotalWalkOffScore > winner.BAM.TotalWalkOffScore)
						{
							winner = user;
						}
					}
				}

				foreach (User user in GameManager.CurrentBataroundGroup.Clinic.users)
				{
					winners.Add(winner);
					if (user.BAM.TotalWalkOffScore == winner.BAM.TotalWalkOffScore && user.id.MasterID != winner.id.MasterID)
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
					WalkOffInGameUI.Close();
				});
			}
			else
			{
				FadeTransitionUI.Open();
				FadeTransitionUI.Instance.Transition(1.5f, .5f, () =>
				{
					TeamMinigameEndUI.Open();
					WalkOffInGameUI.Close();
				});
			}

			BataroundGameManager.Instance.OnHitterChanged -= OnHitterChanged;
			GameManager.OnEndMinigame?.Invoke(this);
		}
	}
}
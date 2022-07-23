using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Collections;

namespace HitTrax.Bataround
{
	public class BataroundModeMinigame : BataroundMinigame
	{
        public Team HomeTeam { get; set; } = new Team();
        public Team AwayTeam { get; set; } = new Team();
        public int TeamAtBat { get; set; }
        public int Outs { get; set; }

        private IEnumerator halfFinishedCoroutine;

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

            GameManager.CurrentlyInBataround = true;
            GameManager.GameSessionActive = true;
            ShowFielders = true;
            FFAGameTime = 180;
            TeamGameTime = 300;

            foreach (var user in GameManager.CurrentBataroundGroup.GameBattingOrder)
            {
                user.BAM.TotalBataroundMinigameScore = 0;
                user.BAM.TotalBataroundBonusPoints = 0;
            }

			if (GameManager.IsFreeForAll)
            {
                GameManager.CurrentBataroundGroup.PlayersLeftToBat = new List<User>(GameManager.CurrentBataroundGroup.GameBattingOrder);
                SetupFFAGame();
			}
			else
			{
                SetupTeamGame();
            }

            OB.strikeZoneDisplay().gameObject.SetActive(false);


            BataroundGameManager.Instance.OnHitterChanged?.Invoke(ThisGame.currentPlayer.user);
            OB.natTrack().MakeReady(OB.state.GetRecordingSession(ThisGame.currentPlayer.user), null, null, GameManager.CurrentBataroundGroup.Clinic, 0);
            OB.state.SwitchHitter(ThisGame.currentPlayer.user);

            BataroundGameDirectionsUI.Open();
        }

		private void OnPlayerChange()
		{
			if (!GameManager.IsFreeForAll)
			{
                StartCoroutine(Co_PlayerChange());       
            }
        }

		private IEnumerator Co_PlayerChange()
		{
			while (InPlay)
			{
                yield return new WaitForEndOfFrame();
			}

            GameManager.CurrentBataroundGroup.PlayersLeftToBat.MoveIndex(0, GameManager.CurrentBataroundGroup.PlayersLeftToBat.Count - 1);
            BataroundGameManager.Instance.OnHitterChanged?.Invoke(ThisGame.currentPlayer.user);
            OB.natTrack().MakeReady(OB.state.GetRecordingSession(ThisGame.currentPlayer.user), null, null, GameManager.CurrentBataroundGroup.Clinic, 0);
            OB.state.SwitchHitter(ThisGame.currentPlayer.user);
        }

        private void OnInningFinished(bool obj)
		{
            Debug.Log("Inning Finished");
            EndMinigame();
		}

        private void OnInningHalfFinished()
        {
			if (GameManager.IsFreeForAll)
			{
                if (halfFinishedCoroutine == null)
				{
                    halfFinishedCoroutine = Co_FFAInningHalfFinished();
                    StartCoroutine(halfFinishedCoroutine);
                }
            }
			else
			{
                if (halfFinishedCoroutine == null)
                {
                    halfFinishedCoroutine = Co_TeamsInningHalfFinished();
                    StartCoroutine(halfFinishedCoroutine);
                }                  
            }
        }

		private IEnumerator Co_TeamsInningHalfFinished()
		{
            while (InPlay)
            {
                yield return new WaitForEndOfFrame();
            }

            TeamAtBat = TeamAtBat == 0 ? 1 : 0;

            var leftToBat = new List<User>();
            foreach (Player player in HomeTeam.players)
            {
                leftToBat.Add(player.user);
            }
            GameManager.CurrentBataroundGroup.PlayersLeftToBat = new List<User>(leftToBat);

            BataroundGameManager.Instance.OnHitterChanged?.Invoke(ThisGame.currentPlayer.user);
            OB.natTrack().MakeReady(OB.state.GetRecordingSession(ThisGame.currentPlayer.user), null, null, GameManager.CurrentBataroundGroup.Clinic, 0);
            OB.state.SwitchHitter(ThisGame.currentPlayer.user);

            BataroundInGameUI.Instance.nextTurnPanel.Open();
            halfFinishedCoroutine = null;
        }

        private IEnumerator Co_FFAInningHalfFinished()
		{
			while (InPlay)
			{
                yield return new WaitForEndOfFrame();
			}

            GameManager.CurrentBataroundGroup.PlayersLeftToBat.Remove(CurrentBatter);
            if (GameManager.CurrentBataroundGroup.PlayersLeftToBat.Count != 0)
            {
                SetupFFAGame();

                BataroundGameManager.Instance.OnHitterChanged?.Invoke(ThisGame.currentPlayer.user);
                OB.natTrack().MakeReady(OB.state.GetRecordingSession(ThisGame.currentPlayer.user), null, null, GameManager.CurrentBataroundGroup.Clinic, 0);
                OB.state.SwitchHitter(ThisGame.currentPlayer.user);
                BataroundInGameUI.Instance.nextTurnPanel.Open();
                halfFinishedCoroutine = null;
			}
			else
			{
                EndMinigame();
                yield return null;
            }
        }

        public override void UnloadSession()
        {
            base.UnloadSession();
            ThisGame.OnInningFinished -= OnInningFinished;
            ThisGame.OnPlayerChanged -= OnPlayerChange;
            ThisGame.OnInningHalfFinished -= OnInningHalfFinished;

            GameManager.CurrentlyInBataround = false;
        }

        private void SetupFFAGame()
        {
            AwayTeam = new Team("Away");
            AwayTeam.AddPlayer(new Player(GameManager.CurrentBataroundGroup.PlayersLeftToBat[0]));

            HomeTeam.gameType.Copy(GameManager.CurrentBataroundGroup.PlayersLeftToBat[0].gameType);
            Player p = new Player("Player");
            HomeTeam.AddPlayer(p);
            p.id.id = p.battingOrder;

            ThisGame = new Game(HomeTeam, AwayTeam)
            {
                type = Game.TEAM_BATTLE,
                stadium = Stadium.BAT_AROUND_NEW,
                playInnings = 1,
                //pitchRule = Game.IntToPitchRule(SuiteGlobals.Values.TeamBattlePitchesCount)
            };


            // Clear all of the old atBats for the player since we don't need to save any data for now
            ThisGame.CleanAtBats();
            
            for (var i = 0; i < AwayTeam.players.Count; ++i)
            {
                if (AwayTeam.players[i].atBats != null)
                {
                    AwayTeam.players[i].atBats.Clear();
                }
            }

            for (var i = 0; i < HomeTeam.players.Count; ++i)
            {
                if (HomeTeam.players[i].atBats != null)
                {
                    HomeTeam.players[i].atBats.Clear();
                }
            }

            OB.state.mode = Globals.MODE_DEMO_GAME;
            ThisGame.Start(false);
            OB.state.GameAdd(ThisGame);
            OB.state.SetRecordingGame(ThisGame);
            OB.fielders().SetGame(ThisGame);
            OB.gamesManager().LoadRunningGame(ThisGame, Objects.FORM_MAIN);

            ThisGame.OnInningFinished += OnInningFinished;
            ThisGame.OnInningHalfFinished += OnInningHalfFinished;
            ThisGame.OnPlayerChanged += OnPlayerChange;
        }
      
        protected void SetupTeamGame()
        {
            foreach (User user in GameManager.CurrentBataroundGroup.GameBattingOrder)
            {
                if (user.BataroundTeam == 0)
                {
                    AwayTeam.AddPlayer(new Player(user));
                }
                else if (user.BataroundTeam == 1)
                {
                    HomeTeam.AddPlayer(new Player(user));
                }
            }

            TeamAtBat = 0;
            ThisGame = new Game(HomeTeam, AwayTeam)
            {
                type = Game.TEAM_BATTLE,
                stadium = Stadium.BOSTON,
                playInnings = 1,
                //pitchRule = Game.IntToPitchRule(SuiteGlobals.Values.TeamBattlePitchesCount)
            };

            var leftToBat = new List<User>();
            foreach (Player player in AwayTeam.players)
            {
                leftToBat.Add(player.user);
            }
            GameManager.CurrentBataroundGroup.PlayersLeftToBat = new List<User>(leftToBat);

            // Clear all of the old atBats for the player since we don't need to save any data for now
            for (var i = 0; i < AwayTeam.players.Count; ++i)
            {
                if (AwayTeam.players[i].atBats != null)
                {
                    AwayTeam.players[i].atBats.Clear();
                }
            }

            for (var i = 0; i < HomeTeam.players.Count; ++i)
            {
                if (HomeTeam.players[i].atBats != null)
                {
                    HomeTeam.players[i].atBats.Clear();
                }
            }

            OB.state.mode = Globals.MODE_DEMO_GAME;
            ThisGame.Start(false);
            OB.state.GameAdd(ThisGame);
            OB.state.SetRecordingGame(ThisGame);
            OB.fielders().SetGame(ThisGame);
            OB.gamesManager().LoadRunningGame(ThisGame, Objects.FORM_MAIN);

            ThisGame.OnInningFinished += OnInningFinished;
            ThisGame.OnInningHalfFinished += OnInningHalfFinished;
            ThisGame.OnPlayerChanged += OnPlayerChange;
        }
   
        public override void OnPlayReset(Play play)
		{
			base.OnPlayReset(play);

            GameManager.OnUpdateBaseDisplayPanel?.Invoke();
            OB.strikeZoneDisplay().Show(false);
		}

		public override void OnImpactMade(Play play)
		{
			base.OnImpactMade(play);
            CurrentBatter.stats.atbats = ThisGame.atBats.Count;
            ScorePlay(play);
        }

		public void ScorePlay(Play play)
        {
			if (ThisGame.atBatFromPlay(play) != null && ThisGame.atBatFromPlay(play).rbis > 0)
            {
                play.bataroundSuccessfulHit = true;

                CurrentBatter.BAM.TotalGameScore += ThisGame.atBatFromPlay(play).rbis;
				CurrentBatter.BAM.TotalBataroundMinigameScore += ThisGame.atBatFromPlay(play).rbis;
				GameManager.OnSuccess?.Invoke();

				if (CurrentBatter.BataroundTeam == 0 && !GameManager.IsFreeForAll)
				{
					GameManager.CurrentBataroundGroup.TeamATotalScore += ThisGame.atBatFromPlay(play).rbis;

					for (int i = 0; i < ThisGame.atBatFromPlay(play).rbis; i++)
					{
                        GameManager.TeamAPoint?.Invoke();
                    }
                }
				else if (CurrentBatter.BataroundTeam == 1 && !GameManager.IsFreeForAll)
				{
					GameManager.CurrentBataroundGroup.TeamBTotalScore += ThisGame.atBatFromPlay(play).rbis;
                   
                    for (int i = 0; i < ThisGame.atBatFromPlay(play).rbis; i++)
                    {
                        GameManager.TeamBPoint?.Invoke();
                    }
                }
            }
        }   

        public override void OnEnterPlay(Play play)
        {
            if (!GameManager.CurrentlyInBataround)
            {
                return;
            }

            if (InPlay)
            {
                return;
            }

            if (play != null && play.hasImpact)
            {
                InPlay = true;
                base.OnEnterPlay(play);
                play.BatAroundMinigame = BatAroundMinigame.BatAround;
                CurrentBatter.BataroundAttemptsRemaining -= 1;
               // CurrentBatter.stats.plays.Add(play);
                CurrentBatter.stats.Calculate();

                CurrentBatter.BAM.TotalBataroundSessionEXP += (int)play.Points();
                ThisGame.paused = false;    
            }
        }

        public override void SetGameDifficulty(MinigameDifficulty difficulty)
        {
            base.SetGameDifficulty(difficulty);
            switch (difficulty)
            {
                case MinigameDifficulty.Easy:
                    MinLaunchAngle = 0;
                    MaxLaunchAngle = 0;
                    BonusMphThreshold = 0;
                    MinDistance = 0;
                    MinVelocity = 0;
                    break;
                case MinigameDifficulty.Medium:
                    MinLaunchAngle = 0;
                    MaxLaunchAngle = 0;
                    BonusMphThreshold = 0;
                    MinDistance = 0;
                    MinVelocity = 0;
                    break;
                case MinigameDifficulty.Hard:
                    MinLaunchAngle = 0;
                    MaxLaunchAngle = 0;
                    BonusMphThreshold = 0;
                    MinDistance = 0;
                    MinVelocity = 0;
                    break;
                default:
                    break;
            }
        }

        public override void EndMinigame()
        {
            base.EndMinigame();

            int teamABatAroundTotalScore = 0;
            int teamBBatAroundTotalScore = 0;
            if (!GameManager.IsFreeForAll)
            {
                foreach (User user in GameManager.CurrentBataroundGroup.Clinic.users)
                {
                    if (user.BataroundTeam == 0)
                    {
                        teamABatAroundTotalScore += user.BAM.TotalBataroundMinigameScore;
                    }

                    if (user.BataroundTeam == 1)
                    {
                        teamBBatAroundTotalScore += user.BAM.TotalBataroundMinigameScore;
                    }
                }

                if (teamABatAroundTotalScore > teamBBatAroundTotalScore)
                {
                    GameManager.CurrentBataroundGroup.TeamARoundsWon += 1;
                }
                else if (teamBBatAroundTotalScore > teamABatAroundTotalScore)
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
                        if (user.BAM.TotalBataroundMinigameScore > winner.BAM.TotalBataroundMinigameScore)
                        {
                            winner = user;
                        }
                    }
                }

                foreach (User user in GameManager.CurrentBataroundGroup.Clinic.users)
                {
                    winners.Add(winner);
                    if (user.BAM.TotalBataroundMinigameScore == winner.BAM.TotalBataroundMinigameScore && user.id.MasterID != winner.id.MasterID)
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
                    BataroundMenuManager.Instance.CloseAllMenus();
                    FFAExperienceEndUI.Open();
                });
            }
            else
            {
                FadeTransitionUI.Open();
                FadeTransitionUI.Instance.Transition(1.5f, .5f, () =>
                {
                    BataroundMenuManager.Instance.CloseAllMenus();
                    TeamExperienceEndUI.Open();
                });
            }

            GameManager.OnEndMinigame?.Invoke(this);
        }
    }
}
using System;
using System.Collections.Generic;
using UnityEngine;

namespace HitTrax.Bataround
{
    public class BataroundGameManager : GameManager<BataroundGameManager>
    {
        public Action<BataroundMinigame> OnEndMinigame { get; set; }
        public Action<User> OnHitterChanged { get; set; }
        public Action OnUpdateBaseDisplayPanel { get;  set; }
        public Action OnUndoLastPlay { get; set; }
        public Action OnTeamChanged { get; set; }
        public Action OnPlayerTurnStart { get; set; }
        public Action OnBatterSkipped { get; set; }
        public Action OnBonusProgress { get; set; }
        public Action OnSuccess { get; set; }
        public Action OnHitTooShort { get; set; }
        public Action OnHitTooSlow { get; set; }
        public Action OnLATooLow { get; set; }
        public Action OnLATooHigh { get; set; }
        public Action OnPause { get; set; }
        public Action TeamAPoint { get; set; }
        public Action TeamBPoint { get; set; }
        public Action BataroundClockStart { get; set; }
        public BataroundGroup CurrentBataroundGroup { get; set; }
        public BataroundMinigame CurrentMinigame { get; set; }
        public bool UsersLoggedIn { get; set; } = false;
        public bool CurrentlyInLinas { get; set; }
        public bool CurrentlyInAroundTheWorld { get; set; }
        public bool CurrentlyInWalkOff { get; set; }
        public bool CurrentlyInSmallBall { get; set; }
        public bool CurrentlyInLaserShow { get; set; }
        public bool CurrentlyInBataround { get; set; }
        public bool IsPaused { get; set; } = false;
        public bool IsFreeForAll { get; set; } = false;
        public bool GameSessionActive { get; set; }
        public bool NewGame { get; set; } = false;

        public bool IsGameSim { 
            get 
            { 
                return CurrentlyInBataround || CurrentlyInSmallBall || CurrentlyInWalkOff; 
            } 
        }


		public static Color BataroundTeamRed = new Color(195f / 255, 32f / 255, 51f / 255, 1f);
        public static Color BataroundTeamBlue = new Color(33f / 255, 45f / 255, 101f / 255, 1f);

        protected override void Awake()
        {
            base.Awake();
        }

		public void LaunchBatAround()
        {
            Objects.Instance.CurrentlyInBataround = true;

            Objects.Instance.fielders().OnPlayReset += OnPlayReset;
            Objects.Instance.natTrack().OnEnterPlay += OnEnterPlay;
            Objects.Instance.natTrack().OnImpactMade += OnImpactMade;

            BataroundMenuManager.Instance.LoadAllUI();
            BataroundSessionsManager.Instance.LoadSession<BataroundSplashSession>();
        }

        public void CloseBatAround()
        {
            Objects.Instance.CurrentlyInBataround = false;
            Objects.Instance.state.mode = Globals.MODE_UNDEFINED;

            Objects.Instance.fielders().OnPlayReset -= OnPlayReset;
            Objects.Instance.natTrack().OnEnterPlay -= OnEnterPlay;
            Objects.Instance.natTrack().OnImpactMade -= OnImpactMade;

            Objects.Instance.entertainmentSelect().enabled = true;
        }

        public void ResetExperience()
        {
            BataroundSessionsManager.Instance.LoadSession<BataroundSplashSession>(); 
        }

        private void OnEnterPlay(Play play)
        {
            CurrentMinigame.OnEnterPlay(play);
        }

        private void OnPlayReset(Play play)
        {
            CurrentMinigame.OnPlayReset(play);
        }

        private void OnImpactMade(Play play)
        {
            CurrentMinigame.OnImpactMade(play);
        }

        public void StartMinigameExperience()
        {
            CurrentMinigame = BataroundSessionsManager.Instance.LoadSession<BarrelsMinigame>();
        }

        public void AddParticipant(string name, BataroundMinigame.MinigameDifficulty difficulty, int handiness)
        {
            var user = new User
            {
                displayName = name,
                screenName = name,
                MinigameDifficulty = difficulty,
                gameType = new GameType(SportType.Baseball, GameType.LEVEL_BASEBALL_HIGH_SCHOOL),
                bats = handiness,
                temporary = true,
                id =
                {
                    id = 999
                }
            };

            if (CurrentBataroundGroup == null)
            {
                CurrentBataroundGroup = new BataroundGroup();
                return;
            }

            if (!CurrentBataroundGroup.Participants.Contains(user))
            {
                CurrentBataroundGroup.Participants.Add(user);
            }
        }

        public void RemoveLastPlay()
		{
            var playToRemove = CurrentMinigame.CurrentBatter.stats.RemoveLast();
            if (playToRemove != null)
            {
                CurrentBataroundGroup.RemovePlay(playToRemove, playToRemove.user);
                OnUndoLastPlay?.Invoke();
            }
        }

        public void LaunchFFA()
        {
            IsFreeForAll = true;

            var clinic = new Clinic { name = $"Bat Around Clinic" };
            
            foreach (var user in CurrentBataroundGroup.Participants)
            {
                clinic.AddUser(user, null, Stats.TYPE_DRILL);
                clinic.sessions.Add(user.stats);
                CurrentBataroundGroup.GameBattingOrder.Add(user);
            }

            CurrentBataroundGroup.Clinic = clinic;
            StartMinigameExperience();
        }

        public void Launch2v2()
        {
            IsFreeForAll = false;

            var clinic = new Clinic { name = $"Bat Around Clinic" };
            
            for (var i = 0; i < CurrentBataroundGroup.Participants.Count; i++)
            {
                clinic.AddUser(CurrentBataroundGroup.Participants[i], null, Stats.TYPE_DRILL);
                clinic.sessions.Add(CurrentBataroundGroup.Participants[i].stats);
            }

            CurrentBataroundGroup.Clinic = clinic;

            // Add users to batting order alternating teams and starting with Team A
            var addToTeamA = true;
            var addToTeamB = false;
            
            while (CurrentBataroundGroup.GameBattingOrder.Count != 4)
            {
                for (var i = 0; i < CurrentBataroundGroup.Participants.Count; i++)
                {
                    if (addToTeamA && CurrentBataroundGroup.Participants[i].BataroundTeam == 0)
                    {
                        if (CurrentBataroundGroup.GameBattingOrder.Contains(CurrentBataroundGroup.Participants[i]))
                        {
                            continue;
                        }

                        CurrentBataroundGroup.GameBattingOrder.Add(CurrentBataroundGroup.Participants[i]);
                        addToTeamA = false;
                        addToTeamB = true;
                    }
                    else if (addToTeamB && CurrentBataroundGroup.Participants[i].BataroundTeam == 1)
                    {
                        if (CurrentBataroundGroup.GameBattingOrder.Contains(CurrentBataroundGroup.Participants[i]))
                        {
                            continue;
                        }

                        CurrentBataroundGroup.GameBattingOrder.Add(CurrentBataroundGroup.Participants[i]);
                        addToTeamB = false;
                        addToTeamA = true;
                    }
                }
            }

            StartMinigameExperience();
        }

        public void Launch3v3()
        {
            IsFreeForAll = false;

            var clinic = new Clinic { name = $"Bat Around Clinic" };

            for (var i = 0; i < CurrentBataroundGroup.Participants.Count; i++)
            {
                clinic.AddUser(CurrentBataroundGroup.Participants[i], null, Stats.TYPE_DRILL);
                clinic.sessions.Add(CurrentBataroundGroup.Participants[i].stats);
            }

            CurrentBataroundGroup.Clinic = clinic;

            // Add users to batting order alternating teams and starting with Team A
            var addToTeamA = true;
            var addToTeamB = false;
            
            while (CurrentBataroundGroup.GameBattingOrder.Count != 6)
            {
                for (var i = 0; i < CurrentBataroundGroup.Participants.Count; i++)
                {
                    if (addToTeamA && CurrentBataroundGroup.Participants[i].BataroundTeam == 0)
                    {
                        if (CurrentBataroundGroup.GameBattingOrder.Contains(CurrentBataroundGroup.Participants[i]))
                        {
                            continue;
                        }

                        CurrentBataroundGroup.GameBattingOrder.Add(CurrentBataroundGroup.Participants[i]);
                        addToTeamA = false;
                        addToTeamB = true;
                    }
                    else if (addToTeamB && CurrentBataroundGroup.Participants[i].BataroundTeam == 1)
                    {
                        if (CurrentBataroundGroup.GameBattingOrder.Contains(CurrentBataroundGroup.Participants[i]))
                        {
                            continue;
                        }

                        CurrentBataroundGroup.GameBattingOrder.Add(CurrentBataroundGroup.Participants[i]);
                        addToTeamB = false;
                        addToTeamA = true;
                    }
                }
            }

            StartMinigameExperience();
        }


        public string TextFromSkillLevel(int level)
        {
            switch (level)
            {
                case 0: return "12U Baseball";
                case 1: return "15U Baseball";
                case 2: return "Highschool Baseball";
                case 3: return "College Baseball";
                case 4: return "Major League";
                case 5: return "13U Baseball";
                case 6: return "10U Baseball";
                case 7: return "8U Baseball";
                default: return "";
            }
        }

        public void TogglePause()
        {
            if (IsPaused)
            {
                BataroundPauseUI.Close();
                IsPaused = false;
            }
            else
            {
                BataroundPauseUI.Open();
                IsPaused = true;
            }
        }
    }
}
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;


#region ReSharper Comments

// ReSharper disable MemberCanBeProtected.Global

#endregion

namespace HitTrax.Bataround
{
    public class BataroundMinigame : BataroundSession
    {
        public int MaxAttempts { get; set; } = 6;
        public bool InPlay { get; set; }
        public List<User> BattingOrder => GameManager.CurrentBataroundGroup.GameBattingOrder;
        public User CurrentBatter => OB.state.CurrentHitter();
        public int CurrentPlayExp { get; set; }
        public Play CurrentPlay { get; set; }
        public Game ThisGame { get; set; }
        public bool ChangeHitterAfterPlay { get; set; }
        public float MaxLaunchAngle { get; set; }
        public float MinLaunchAngle { get; set; }
        public float BonusMphThreshold { get; set; }
        public float MinDistance { get; set; }
        public float MinVelocity { get; set; }
        public int TeamGameTime { get; set; }
        public int FFAGameTime { get; set; }

        public enum MinigameDifficulty
        {
            Easy = 0,
            Medium = 1,
            Hard = 2
        }

        private BataroundGameManager gameManager;

        public BataroundGameManager GameManager
        {
            get
            {
                if (gameManager == null)
                {
                    gameManager = BataroundGameManager.Instance;
                }

                return gameManager;
            }
        }

        protected override void LoadSession()
        {
            base.LoadSession();
            OB.DisableEverything();

            OB.CurrentlyInBataround = true;
            OB.state.mode = Globals.MODE_BATAROUND;
            GameManager.CurrentlyInLinas = false;
            GameManager.CurrentlyInAroundTheWorld = false;
            GameManager.CurrentlyInLaserShow = false;
            GameManager.CurrentlyInSmallBall = false;
            GameManager.OnBatterSkipped -= SkipUserTurn;
            GameManager.OnBatterSkipped += SkipUserTurn;
            GameManager.OnUndoLastPlay -= UndoLastPlay;
            GameManager.OnUndoLastPlay += UndoLastPlay;

            OB.ballDisplay().Clear();
            OB.mouseMove().enabled = true;
            OB.measureDistance().MakeReady();
            OB.camEntertainment().SetActive(true);

            if (!GameManager.UsersLoggedIn)
            {
                LoginUsers(GameManager.CurrentBataroundGroup.Clinic);
            }

            foreach (User user in GameManager.CurrentBataroundGroup.Clinic.users)
            {
                user.BataroundAttemptsRemaining = MaxAttempts;
            }

            SetFirstHitter();
        }

		private void UndoLastPlay()
		{
            CurrentBatter.stats.RemoveLast();
        }

		public virtual void OnEnterPlay(Play play)
        {
            InPlay = true;
            CurrentPlay = play;
            play.BatAroundMinigame = BatAroundMinigame.BatAround;

            if (play.exitBallVel.magnitude > CurrentBatter.BataroundMaxGameVelo)
            {
                CurrentBatter.BataroundMaxGameVelo = play.exitBallVel.magnitude;
            }

            if (!Objects.Instance.natTrack().paused)
            {
                Objects.Instance.natTrack().paused = true;
            }
        }

        public virtual void UpdateDrill(Play play)
        {
            CurrentPlay = play;
            play.BatAroundMinigame = BatAroundMinigame.BatAround;

            if (play.exitBallVel.magnitude > CurrentBatter.BataroundMaxGameVelo)
            {
                CurrentBatter.BataroundMaxGameVelo = play.exitBallVel.magnitude;
            }

            if (!Objects.Instance.natTrack().paused)
            {
                Objects.Instance.natTrack().paused = true;
            }
        }

        public virtual void OnPlayReset(Play play)
        {
            InPlay = false;
            CurrentPlay = null;

            if (Objects.Instance.natTrack().paused)
            {
                Objects.Instance.natTrack().paused = false;
            }

			if (ChangeHitterAfterPlay)
			{
				ChangeHitterAfterPlay = false;
				NextHitter();
			}
		}

        public virtual void OnImpactMade(Play play)
		{

		}

		protected virtual void NextHitter()
        {
            if (GameManager.CurrentMinigame != null && GameManager.CurrentMinigame.CurrentBatter != null && GameManager.CurrentMinigame.CurrentBatter.BataroundAttemptsRemaining == 0)
            {
                GameManager.CurrentBataroundGroup.PlayersLeftToBat.Remove(GameManager.CurrentMinigame.CurrentBatter);
                GameManager.CurrentBataroundGroup.AlreadyBattedList.Add(GameManager.CurrentMinigame.CurrentBatter);
            }

            if (GameManager.CurrentBataroundGroup.PlayersLeftToBat.Count == 0)
            {
                EndMinigame();
            }
            else
            {
                var hitter = GameManager.CurrentBataroundGroup.PlayersLeftToBat[0];
                OB.state.SwitchHitter(hitter);

                OB.natTrack().MakeReady(OB.state.GetRecordingSession(hitter), null, null, GameManager.CurrentBataroundGroup.Clinic, 0);
                LoadStadium();
                SetGameDifficulty();
                BataroundGameManager.Instance.OnHitterChanged?.Invoke(hitter);
            }
        }

        public void SetGameDifficulty()
        {
            SetGameDifficulty(CurrentBatter.BAM.MinigameDifficulty);
        }

        public virtual void SetGameDifficulty(MinigameDifficulty difficulty)
        {
        }

        /// <summary>
        /// Skips the user and places them in the end of the batting order
        /// </summary>
        private void SkipUserTurn()
        {
            GameManager.CurrentBataroundGroup.PlayersLeftToBat.MoveIndex(0, GameManager.CurrentBataroundGroup.PlayersLeftToBat.Count - 1);
            NextHitter();
        }

        public virtual void LoadStadium()
        {
            LoadStadium(CurrentBatter.BAM.MinigameDifficulty);
        }

        public void LoadStadium(MinigameDifficulty difficulty)
        {
            OB.SetStadium(Stadium.BAT_AROUND_NEW, CurrentBatter.gameType, CurrentBatter);
            var stadium = GameObject.Find("sBatAroundNew(Clone)").GetComponent<StadiumData>().field;
            var stadiumPos = new Vector3(stadium.transform.position.x, 1.88f, stadium.transform.position.z);
            stadium.transform.position = stadiumPos;

            // Setup PostProcessing
            //         var camera = GameObject.Find("Cameras").FindChild("cameraEntertainment");
            //         if (camera.GetComponent<PostProcessLayer>() == null)
            //{
            //             var ppLayer = camera.AddComponent<PostProcessLayer>();
            //             ppLayer.volumeLayer = LayerMask.GetMask("Stadium");
            //            // ppLayer.fog.enabled = false;
            //            // ppLayer.antialiasingMode = PostProcessLayer.Antialiasing.None;
            //}
            //else
            //{
            //             var layer = camera.GetComponent<PostProcessLayer>();
            //             layer.enabled = true;
            //         }
        }

		public virtual void EndMinigame()
        {
            GameManager.CurrentBataroundGroup.AlreadyBattedList.Clear();
            //var camera = GameObject.Find("Cameras").FindChild("bataroundCamera");
            //Destroy(camera.GetComponent<PostProcessLayer>());
        }

        public virtual void LoginUsers(Clinic clinic)
        {
            foreach (var user in clinic.users)
            {
                OB.state.loginUser(user);
            }

            for (var i = 0; i < clinic.users.Count; ++i)
            {
                var hittingSession = OB.license().NewStats(clinic.users[i].gameType, User.DefaultHeight(clinic.users[i].gameType), OB.app().extendStrikeZone, OB.app().lowerStrikeZone, Stats.TYPE_BATAROUND);
                //hittingSession.BAM.MinigameDifficulty = clinic.users[i].MinigameDifficulty;
                clinic.users[i].stats = hittingSession;
                hittingSession.stadium = OB.stadiumController.currentStadium;
                OB.state.SetRecordingSession(CurrentBatter, hittingSession);
            }
            
            gameManager.UsersLoggedIn = true;
        }

        public virtual void SetFirstHitter()
        {
            GameManager.CurrentBataroundGroup.PlayersLeftToBat = new List<User>(GameManager.CurrentBataroundGroup.GameBattingOrder);
            NextHitter();
        }
    }
}
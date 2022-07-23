using System.Collections.Generic;
using UnityEngine;

namespace HitTrax.Bataround
{
    public class BarrelsMinigame : BataroundMinigame
    {
        public Material TargetMeshMaterial { get; set; }
        public GameObject TargetMeshObject { get; set; }
        public DangerZone DangerZone { get; set; }
        public DangerZone DangerZoneSprayColor { get; set; }


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

            GameManager.CurrentlyInLinas = true;
            GameManager.GameSessionActive = true;
            ShowFielders = false;

            foreach (var user in GameManager.CurrentBataroundGroup.GameBattingOrder)
            {
                user.BAM.TotalLinasScore = 0;
                user.BAM.TotalBataroundSessionEXP = 0;
                user.BAM.TotalBataroundBonusPoints = 0;
                user.BataroundMaxGameVelo = 0;
            }

            if (DangerZone != null && DangerZoneSprayColor != null)
            {
                Destroy(DangerZone.gameObject);
                Destroy(DangerZoneSprayColor.gameObject);
            }

            DangerZone = DangerZone.CreateDangerZone(OB.homePlate.transform, Objects.Instance.CenterFieldDistance(), DangerZone.GrowthDirection.Right);
            DangerZone.SetDegrees(30, 30);
            DangerZone.SetColor(DangerZone.ColorOptions.BataroundBarrelsOutline);

            DangerZoneSprayColor = DangerZone.CreateDangerZone(OB.homePlate.transform, Objects.Instance.CenterFieldDistance(), DangerZone.GrowthDirection.Right);
            DangerZoneSprayColor.SetDegrees(30, 30);
            DangerZoneSprayColor.SetColor(DangerZone.ColorOptions.BataroundWhiteSprayChart);
            DangerZoneSprayColor.gameObject.layer = LayerMask.NameToLayer("SprayChart");

            BataroundGameDirectionsUI.Open();
        }

        public override void UnloadSession()
        {
            base.UnloadSession();
            GameManager.CurrentlyInLinas = false;

            if (DangerZone != null && DangerZoneSprayColor != null)
            {
                Destroy(DangerZone.gameObject);
                Destroy(DangerZoneSprayColor.gameObject);
            }
        }

        public override void OnEnterPlay(Play play)
        {
            if (!GameManager.CurrentlyInLinas)
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

                CurrentBatter.BataroundAttemptsRemaining -= 1;
                play.BatAroundMinigame = BatAroundMinigame.Linas;
                CurrentBatter.stats.plays.Add(play);
                CurrentBatter.stats.Calculate();

                bool bonus;
                var lineDrive = IsLineDrive(play, BonusMphThreshold, MinLaunchAngle, MaxLaunchAngle, MinDistance, out bonus);
                if (lineDrive)
                {
                    OnLineDrive(play);
                    play.bataroundSuccessfulHit = true;
                }

                if (Globals.M2F(play.distance) < MinDistance)
                {
                    BataroundGameManager.Instance.OnHitTooShort?.Invoke();
                }

                CurrentBatter.BAM.TotalBataroundSessionEXP += (int)play.Points();
               
                if (CurrentBatter.BataroundAttemptsRemaining <= 0)
                {
                    ChangeHitterAfterPlay = true;
				}
				else
				{
                    ChangeHitterAfterPlay = false;
                }

                if (bonus)
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

        public override void SetGameDifficulty(MinigameDifficulty difficulty)
        {
            base.SetGameDifficulty(difficulty);
            switch (difficulty)
            {
                case MinigameDifficulty.Easy:
                    MinLaunchAngle = 5;
                    MaxLaunchAngle = 30;
                    BonusMphThreshold = 50;
                    MinDistance = 35;
                    MinVelocity = 0;
                    break;
                case MinigameDifficulty.Medium:
                    MinLaunchAngle = 10;
                    MaxLaunchAngle = 25;
                    BonusMphThreshold = 70;
                    MinDistance = 60;
                    MinVelocity = 0;
                    break;
                case MinigameDifficulty.Hard:
                    MinLaunchAngle = 10;
                    MaxLaunchAngle = 20;
                    BonusMphThreshold = 90;
                    MinDistance = 75;
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

        private void OnLineDrive(Play play)
        {
            CrowdCheer();

            CurrentBatter.BAM.TotalGameScore += 1;
            CurrentBatter.BAM.TotalLinasScore += 1;
            GameManager.OnSuccess?.Invoke();

			if (!BataroundGameManager.Instance.IsFreeForAll)
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
        }

        public static bool IsLineDrive(Play play, float mphBonusThreshold, float minAngle, float maxAngle, float distanceReq, out bool bonus)
        {
            bonus = false;

            if (!play.isCenterField)
            {
                return false;
            }

            if (Globals.M2F(play.distance) < distanceReq)
            {
                return false;
            }

            if (play.elevation >= minAngle && play.elevation <= maxAngle)
            {
                var mph = Globals.MPS2MHP(play.exitBallVel.magnitude);
                if (mph >= mphBonusThreshold)
                {
                    bonus = true;
                }

                return true;
            }

            return false;
        }

        public override void EndMinigame()
        {
            base.EndMinigame();

            int teamAinasTotalScore = 0;
            int teamBLinasTotalScore = 0;
            if (!GameManager.IsFreeForAll)
            {
                foreach (User user in GameManager.CurrentBataroundGroup.Clinic.users)
                {
                    if (user.BataroundTeam == 0)
                    {
                        teamAinasTotalScore += user.BAM.TotalLinasScore;
                    }

                    if (user.BataroundTeam == 1)
                    {
                        teamBLinasTotalScore += user.BAM.TotalLinasScore;
                    }
                }

                if (teamAinasTotalScore > teamBLinasTotalScore)
                {
                    GameManager.CurrentBataroundGroup.TeamARoundsWon += 1;
                }
                else if (teamBLinasTotalScore > teamAinasTotalScore)
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
                        if (user.BAM.TotalLinasScore > winner.BAM.TotalLinasScore)
                        {
                            winner = user;
                        }
                    }
                }

                foreach (User user in GameManager.CurrentBataroundGroup.Clinic.users)
                {
                    winners.Add(winner);
                    if (user.BAM.TotalLinasScore == winner.BAM.TotalLinasScore && user.id.MasterID != winner.id.MasterID)
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
                    FFAEndMinigameUI.Open();
                });
            }
            else
            {
                FadeTransitionUI.Open();
                FadeTransitionUI.Instance.Transition(1.5f, .5f, () =>
                {
                    BataroundMenuManager.Instance.CloseAllMenus();
                    TeamMinigameEndUI.Open();
                });
            }

            GameManager.OnEndMinigame?.Invoke(this);
        }

        private void CrowdCheer()
        {
            var rand = Random.Range(0, 1);
            var sound = rand == 1 ? OB.soundCrowd.soundCroud.cheer3 : OB.soundCrowd.soundCroud.cheer2;
            sound.Play();
        }

        private void ExplosionEffect()
        {
            var explosion = Resources.Load<GameObject>("ExplosionEffect");
            explosion = Instantiate(explosion);
            explosion.transform.position = OB.homePlate.transform.position; // + new Vector3(0, 5, 0);
        }

        public static Color GetBarrelsTrailColor(Play play)
        {
            var level = Objects.Instance.state.CurrentHitter().gameType.level;

            // Change trail color based on velo
            if (Globals.MPS2MHP(play.exitBallVel.magnitude) >= BarreledBySkillLevel.GetBarreledMinimunVel(level))
            {
                return new Color(1f, 0f, 0f); // red
            }
            else if (Globals.MPS2MHP(play.exitBallVel.magnitude) > BarreledBySkillLevel.GetBarreledMinimunVel(level) * .5f)
            {
                return new Color(1f, .54f, 0f); // orange
            }
            else
            {
                return new Color(.2f, .8f, 0f); // green
            }
        }
    }
}
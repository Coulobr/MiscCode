using System;
using TMPro;
using UnityEngine;

namespace HitTrax.Bataround
{
    public class BataroundInGameUI : Menu<BataroundInGameUI>
    {
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

        private MinigameTeamScorePanel minigameTeamScorePanel;
        private CurrentUserStatsPanel currentUserStatsPanel;
        private BataroundGizmosPanel bataroundGizmosPanel;
        private OptionsPanel optionsPanel;
        private BataroundBonusPanel bonusPanel;
        private FFALeaderboardPanel ffaLeaderboardPanel;
        public NextPlayerTurnPanel nextTurnPanel;
        private GameClockPanel gameClockPanel;
        private UndoHitPanel undoHitPanel;

        protected override void OnInitialized()
        {
            base.OnInitialized();
        }

        public override void GeneratePanels()
        {
            base.GeneratePanels();

            if (undoHitPanel == null)
            {
                undoHitPanel = UndoHitPanel.Create(transform);
                undoHitPanel.SetupPanel();
            }

            if (minigameTeamScorePanel == null)
            {
                minigameTeamScorePanel = MinigameTeamScorePanel.Create(transform);
                minigameTeamScorePanel.SetupPanel();
            }

            if (ffaLeaderboardPanel == null)
            {
                ffaLeaderboardPanel = FFALeaderboardPanel.Create(transform);
                ffaLeaderboardPanel.SetupPanel();
            }

            if (currentUserStatsPanel == null)
            {
                currentUserStatsPanel = CurrentUserStatsPanel.Create(transform);
                currentUserStatsPanel.SetupPanel();
            }

            if (bataroundGizmosPanel == null)
            {
                bataroundGizmosPanel = BataroundGizmosPanel.Create(transform);
                bataroundGizmosPanel.SetupPanel();
            }

            if (optionsPanel == null)
            {
                optionsPanel = OptionsPanel.Create(transform);
                optionsPanel.SetupPanel();
            }

            if (bonusPanel == null)
            {
                bonusPanel = BataroundBonusPanel.Create(transform);
                bonusPanel.SetupPanel();
            }

            if (nextTurnPanel == null)
            {
                nextTurnPanel = NextPlayerTurnPanel.Create(transform);
                nextTurnPanel.SetupPanel();
            }

            if (gameClockPanel == null)
            {
                gameClockPanel = GameClockPanel.Create(transform);
                gameClockPanel.SetupPanel();
            }
        }

        protected override void OnOpened()
        {
            base.OnOpened();
            OpenAllPanels();

            if (BataroundGameManager.Instance.CurrentlyInBataround)
			{
               // BataroundGameManager.Instance.CurrentMinigame.ThisGame.OnInningHalfFinished += OnChangeTeam;
                GameClockPanel.OnTimerEnd += () => {
                    BataroundGameManager.Instance.CurrentMinigame.ThisGame.FinishInningHalf();
                    nextTurnPanel.Open();
                };
            }
        }

		protected override void OnClosed()
        {
            base.OnClosed();
            CloseAllPanels();

            if (BataroundGameManager.Instance.CurrentlyInBataround)
            {
               // BataroundGameManager.Instance.CurrentMinigame.ThisGame.OnInningHalfFinished -= OnChangeTeam;
                GameClockPanel.OnTimerEnd -= () => BataroundGameManager.Instance.CurrentMinigame.ThisGame.FinishInningHalf();
            }
        }

        private void OpenAllPanels()
        {
            if (GameManager.IsFreeForAll)
            {
                ffaLeaderboardPanel.Open();
                minigameTeamScorePanel.Close();
            }
            else
            {
                minigameTeamScorePanel.Open();
                ffaLeaderboardPanel.Close();
            }

            currentUserStatsPanel.Open();
            bataroundGizmosPanel.Open();
            optionsPanel.Open();
            nextTurnPanel.Open();
            gameClockPanel.Open();
            undoHitPanel.Open();
        }

        private void CloseAllPanels()
        {
            if (ffaLeaderboardPanel != null)
            {
                ffaLeaderboardPanel.Close();
            }

            if (minigameTeamScorePanel != null)
            {
                minigameTeamScorePanel.Close();
            }

            if (currentUserStatsPanel != null)
            {
                currentUserStatsPanel.Close();
            }

            if (bataroundGizmosPanel != null)
            {
                bataroundGizmosPanel.Close();
            }

            if (optionsPanel != null)
            {
                optionsPanel.Close();
            }

            if (bonusPanel != null)
            {
                bonusPanel.Close();
            }

            if (nextTurnPanel != null)
            {
                nextTurnPanel.Close();
            }

            if (gameClockPanel != null)
            {
                gameClockPanel.Close();
            }

            if (undoHitPanel != null)
            {
                undoHitPanel.Close();
            }
        }
    }
}
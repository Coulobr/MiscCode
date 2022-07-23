using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace HitTrax.Bataround
{
	public class WalkOffInGameUI : Menu<WalkOffInGameUI>
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
        private FFALeaderboardPanel ffaLeaderboardPanel;
        private NextPlayerTurnPanel nextTurnPanel;
        private SmallBallSituationPanel smallBallSituationPanel;
        private UndoHitPanel undoHitPanel;

        protected override void OnInitialized()
        {
            base.OnInitialized();
        }

        public override void GeneratePanels()
        {
            base.GeneratePanels();

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

            if (nextTurnPanel == null)
            {
                nextTurnPanel = NextPlayerTurnPanel.Create(transform);
                nextTurnPanel.SetupPanel();
            }

            if (smallBallSituationPanel == null)
            {
                smallBallSituationPanel = SmallBallSituationPanel.Create(transform);
                smallBallSituationPanel.SetupPanel();
            }

            if (undoHitPanel == null)
            {
                undoHitPanel = UndoHitPanel.Create(transform);
                undoHitPanel.SetupPanel();
            }
        }

        protected override void OnOpened()
        {
            base.OnOpened();
            GameManager.OnHitterChanged += OnHitterChanged;
            OpenAllPanels();
            SmallBallMinigame.SituationChanged?.Invoke(WalkOffMinigame.Drill);
        }

        protected override void OnClosed()
        {
            base.OnClosed();
            GameManager.OnHitterChanged -= OnHitterChanged;
            CloseAllPanels();
        }

        private void OnHitterChanged(User user)
        {
            nextTurnPanel.Open();
            SmallBallMinigame.SituationChanged?.Invoke(SmallBallMinigame.ManOnSecondDrill);
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
            smallBallSituationPanel.Open();
            undoHitPanel.Open();
            nextTurnPanel.Open();
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

            if (nextTurnPanel != null)
            {
                nextTurnPanel.Close();
            }

            if (undoHitPanel != null)
            {
                undoHitPanel.Close();
            }
        }
    }
}

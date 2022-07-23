using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HitTrax.Bataround
{
    public class NumPlayersUI : Menu<NumPlayersUI>
    {
        public SmartButton backBtn;
        public SmartButton continueBtn;
        public SmartButton manuallyAddUserBtn;
        public Panel userListPanel;
        public GameObject playerInfoPrefab;
        public Transform currentLineupLayout;
        public List<PlayerInfoCard> Players { get; set; } = new List<PlayerInfoCard>();

        protected override void OnInitialized()
        {
            if (backBtn)
            {
                backBtn.onClick.RemoveAllListeners();
                backBtn.onClick.AddListener(OnBackClick);
            } 

            if (continueBtn)
            {
                continueBtn.onClick.RemoveAllListeners();
                continueBtn.onClick.AddListener(OnContinueClick);
            }

            if (manuallyAddUserBtn)
            {
                manuallyAddUserBtn.onClick.RemoveAllListeners();
                manuallyAddUserBtn.onClick.AddListener(()=> userListPanel.Open());
            }
        }

        protected override void OnOpened()
        {
            base.OnOpened();
            ClearUserList();

			foreach (var player in BataroundGameManager.Instance.CurrentBataroundGroup.Participants)
			{
				var infoCardGO = Instantiate(playerInfoPrefab, currentLineupLayout);
				var infoCard = infoCardGO.GetComponent<PlayerInfoCard>();

				if (infoCard != null)
				{
                    infoCard.playerName.text = player.screenName;
					infoCard.playerDifficultyDropdown.value = (int)player.MinigameDifficulty;
                    infoCard.handinessDropdown.value = (player.bats - 1) < 2 ? player.bats - 1 : 0;
                    infoCard.userGuid = player.id.MasterID;

					Players.Add(infoCard);
				}
				else
				{
					Utility.LogError("playerInfoPrefab does not have a PlayerInfoCard component, destroying generated gameobject!");
					Destroy(infoCardGO);
				}
			}

            SetContinueButtonState();
        }

        private void ClearUserList()
        {
            foreach (var player in Players)
            {
                Destroy(player.gameObject);
            }

            Players.Clear();
        }

        private void OnContinueClick()
        {
            if (Players.Count == 0)
            {
                return;
            }

            if (Players.Any(player => string.IsNullOrWhiteSpace(player.playerName.text)))
            {
                Utility.LogError("Tried to add player with same ID");
                return;
            }

            BataroundGameManager.Instance.CurrentBataroundGroup.ResetParticipants();
            BataroundGameManager.Instance.CurrentBataroundGroup.ResetBattingOrder();
            BataroundGameManager.Instance.UsersLoggedIn = false;

            // If user has a handiness of 0, they are a righty
            foreach (var player in Players)
            {
                if (player.Handiness == 0)
				{
                    player.Handiness = Globals.BATS_RIGHT;
				}

                BataroundGameManager.Instance.AddParticipant(player.playerName.text, player.Difficulty, player.Handiness);
            }

            GameTypeSelectUI.Open();
            Close();
        }

		public void RemovePlayer(PlayerInfoCard playerInfoCard)
		{
            Players.Remove(playerInfoCard);
            SetContinueButtonState();
        }

        private void OnBackClick()
        {
            BataroundSessionsManager.Instance.LoadSession<BataroundSplashSession>();
            Close();
        }

        /// <summary>
        /// Adds the passed user into the current lineup layout
        /// </summary>
        /// <param name="user"></param>
        public void AddPlayer(User user)
        {
            if (Players.Count == 6)
            {
                return;
            }

			foreach (var userInfo in Players)
			{
                if (userInfo.userGuid == user.id.MasterID)
				{
                    Utility.LogError("Tried to add player with same ID");
                    return;
				}
            }

            var infoCardGO = Instantiate(playerInfoPrefab, currentLineupLayout);
            var infoCard = infoCardGO.GetComponent<PlayerInfoCard>();

            if (infoCard != null)
            {
                infoCard.playerName.text = user.firstName;
                infoCard.Difficulty = BataroundMinigame.MinigameDifficulty.Easy;
                infoCard.playerDifficultyDropdown.value = 0;
				infoCard.handinessDropdown.value = (user.bats - 1) < 2 ? user.bats - 1 : 0;
				infoCard.userGuid = user.id.MasterID;

                Players.Add(infoCard);          
            }
            else
            {
                Utility.LogError("playerInfoPrefab does not have a PlayerInfoCard component, destroying generated gameobject!");
                Destroy(infoCardGO);
            }

            SetContinueButtonState();
        }

        private void SetContinueButtonState()
		{
            if (Players.Count == 0)
            {
                continueBtn.interactable = false;
			}
			else
			{
                continueBtn.interactable = true;
            }
        }
    }
}
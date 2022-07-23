using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace HitTrax.Bataround
{
    public class BataroundTeamSelectionUI : Menu<BataroundTeamSelectionUI>
    {
        public Transform playerLayout;
        public Transform teamAlayout;
        public Transform teamBlayout;

        public TeamSelectionCard template;

        [FormerlySerializedAs("Buttons")]
        public SmartButton continueBtn;
        public SmartButton backBtn;



        public List<TeamSelectionCard> UserCards { get; set; } = new List<TeamSelectionCard>();

        protected override void OnInitialized()
        {
            base.OnInitialized();

            if (continueBtn)
            {
                continueBtn.onClick.RemoveAllListeners();
                continueBtn.onClick.AddListener(OnContinueClick);
            }

            if (backBtn)
            {
                backBtn.onClick.RemoveAllListeners();
                backBtn.onClick.AddListener(OnClickBack);
            }
        }

        // protected void Update()
        // {
        //     if (continuteBtn)
        //     {
        //         continuteBtn.interactable = teamAList.Count == 
        //     }
        // }

        protected override void OnOpened()
        {
            base.OnOpened();
            foreach (var user in BataroundGameManager.Instance.CurrentBataroundGroup.Participants)
            {
                var userCardGO = Instantiate(template, playerLayout);
                var userCard = userCardGO.GetComponent<TeamSelectionCard>();

                if (userCard != null)
                {
                    userCard.SetData(user);
                    UserCards.Add(userCard);
                }
                else
                {
                    Utility.LogError("The given template does not have a TeamSelectionCard component, destroying gameobject!");
                    Destroy(userCardGO);
                }
            }
        }

		protected override void OnClosed()
		{
			base.OnClosed();
            ResetLayouts();
		}

		private void OnContinueClick()
        {
            // TODO: Refactor

            var teamACards = teamAlayout.GetComponentsInChildren<TeamSelectionCard>();
            var teamBCards = teamBlayout.GetComponentsInChildren<TeamSelectionCard>();

            if (teamACards.Length != teamBCards.Length)
            {
                return;
            }

			foreach (var card in teamACards)
			{
				card.User.BataroundTeam = 0;
			}

			foreach (var card in teamBCards)
			{
				card.User.BataroundTeam = 1;
			}

			if (teamACards.Length == 3)
            {
                BataroundGameManager.Instance.Launch3v3();
                Close();
            }
            else if (teamACards.Length == 2)
            {
                BataroundGameManager.Instance.Launch2v2();
                Close();
            }
        }

        private void OnClickBack()
        {
            GameTypeSelectUI.Open();
            Close();
        }

        public void ResetLayouts()
        {
            for (var i = UserCards.Count - 1; i >= 0; --i)
            {
                Destroy(UserCards[i].gameObject);
            }

            UserCards.Clear();
        }
    }
}
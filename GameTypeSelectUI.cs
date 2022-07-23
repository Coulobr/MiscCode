#region ReSharper Comments

// ReSharper disable IdentifierTypo

#endregion

namespace HitTrax.Bataround
{
    public class GameTypeSelectUI : Menu<GameTypeSelectUI>
    {
        public SmartButton ffaBtn;
        public SmartButton twoVStwoBtn;
        public SmartButton threeVSthreeBtn;
        public SmartButton backBtn;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            if (ffaBtn)
            {
                ffaBtn.onClick.RemoveAllListeners();

                ffaBtn.onClick.AddListener(OnClickFFA);
            }

            if (twoVStwoBtn)
            {
                twoVStwoBtn.onClick.RemoveAllListeners();

                twoVStwoBtn.onClick.AddListener(OnClick2v2);
            }

            if (threeVSthreeBtn)
            {
                threeVSthreeBtn.onClick.RemoveAllListeners();
                threeVSthreeBtn.onClick.AddListener(OnClick3v3);
            }

            if (backBtn)
            {
                backBtn.onClick.RemoveAllListeners();
                backBtn.onClick.AddListener(OnClickBack);
            }
        }

        protected override void OnOpened()
        {
            base.OnOpened();
            var participantCount = BataroundGameManager.Instance.CurrentBataroundGroup.Participants.Count;
            twoVStwoBtn.interactable = participantCount == 4;
            threeVSthreeBtn.interactable = participantCount == 6;
        }

        private void OnClickBack()
        {
            NumPlayersUI.Open();
            Close();
        }

        public void OnClick3v3()
        {
            BataroundTeamSelectionUI.Open();
            Close();
        }

        public void OnClick2v2()
        {
            BataroundTeamSelectionUI.Open();
            Close();
        }

        public void OnClickFFA()
        {
            BataroundGameManager.Instance.LaunchFFA();
            Close();
        }
    }
}
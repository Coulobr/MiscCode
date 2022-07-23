using TMPro;

namespace HitTrax.Bataround
{
	public class FFAEndMinigameUI : Menu<FFAEndMinigameUI>
	{
		public PostMatchUserListPanel userListPanel;
		public BataroundPlaysListPanel playsListPanel;
		public TextMeshProUGUI title;
		public SmartButton continueBtn;
		public BataroundGameManager GameManager => BataroundGameManager.Instance;

		protected override void OnInitialized()
		{
			base.OnInitialized();

			if (continueBtn)
			{
				continueBtn.onClick.RemoveAllListeners();
				continueBtn.onClick.AddListener(OnContinueClick);
			}
		}

		protected override void OnOpened()
		{
			base.OnOpened();
			userListPanel.listGenerated = false;
			userListPanel.Open();
		}

		protected override void OnClosed()
		{
			base.OnClosed();
			userListPanel.Close();

			if (BataroundGameManager.Instance.CurrentlyInLinas)
			{
				BataroundGameManager.Instance.CurrentMinigame = BataroundSessionsManager.Instance.LoadSession<BataroundAroundTheWorldMinigame>();
			}
			else if (BataroundGameManager.Instance.CurrentlyInAroundTheWorld)
			{
				BataroundGameManager.Instance.CurrentMinigame = BataroundSessionsManager.Instance.LoadSession<SmallBallMinigame>();
			}
			else if (BataroundGameManager.Instance.CurrentlyInSmallBall)
			{
				BataroundGameManager.Instance.CurrentMinigame = BataroundSessionsManager.Instance.LoadSession<LaserShowMinigame>();
			}
			else if (BataroundGameManager.Instance.CurrentlyInLaserShow)
			{
				BataroundGameManager.Instance.CurrentMinigame = BataroundSessionsManager.Instance.LoadSession<WalkOffMinigame>();
			}
			else if (BataroundGameManager.Instance.CurrentlyInWalkOff)
			{
				BataroundGameManager.Instance.CurrentMinigame = BataroundSessionsManager.Instance.LoadSession<BataroundModeMinigame>();
			}
		}

		private void OnContinueClick()
		{
			Close();
		}
	}
}


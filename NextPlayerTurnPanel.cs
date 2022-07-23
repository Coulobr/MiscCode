using TMPro;

namespace HitTrax.Bataround
{
	public class NextPlayerTurnPanel : Panel<NextPlayerTurnPanel>
	{
		public TextMeshProUGUI batterName;
		public SmartButton playBtn;
		public SmartButton skipBtn;

		protected override void OnOpened()
		{
			base.OnOpened();
			
			playBtn.onClick.AddListener(OnPlayClick);
			skipBtn.onClick.AddListener(OnSkipClick);

			// Pause tracking
			if (!Objects.Instance.natTrack().paused)
			{
				Objects.Instance.natTrack().paused = true;
			}

			UpdatePanel();
		}
		protected override void OnClosed()
		{
			base.OnClosed();
			playBtn.onClick.RemoveAllListeners();
			skipBtn.onClick.RemoveAllListeners();

			if (Objects.Instance.natTrack().paused)
			{
				Objects.Instance.natTrack().paused = false;
			}
		}

		public override void SetupPanel()
		{
			base.SetupPanel();
		}

		public override void UpdatePanel()
		{
			base.UpdatePanel();

			if (!isOpen)
			{
				return;
			}

			skipBtn.interactable = BataroundGameManager.Instance.CurrentBataroundGroup.PlayersLeftToBat.Count == 1 ? false : true;

			var batterToPlay = BataroundGameManager.Instance.CurrentMinigame.CurrentBatter;
			batterName.text = batterToPlay.screenName + "'S TURN";
		}

		private void OnSkipClick()
		{
			BataroundGameManager.Instance.OnBatterSkipped?.Invoke();
			UpdatePanel();
		}

		private void OnPlayClick()
		{
			BataroundGameManager.Instance.OnPlayerTurnStart?.Invoke();
			Close();
		}
	}
}

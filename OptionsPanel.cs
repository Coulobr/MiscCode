namespace HitTrax.Bataround
{
	public class OptionsPanel : Panel<OptionsPanel>
	{
		public SmartButton pauseBtn;
		public SmartButton exitBtn;

		public override void SetupPanel()
		{
			base.SetupPanel();

			pauseBtn.onClick.RemoveAllListeners();
			pauseBtn.onClick.AddListener(() => BataroundGameManager.Instance.TogglePause());

			exitBtn.onClick.RemoveAllListeners();
			exitBtn.onClick.AddListener(() =>
			{
				BataroundSessionsManager.Instance.LoadSession<GameModeSelectSession>();
			});
		}

		protected override void OnClosed()
		{
			base.OnClosed();
		}

		protected override void OnOpened()
		{
			base.OnOpened();
		}
	}
}

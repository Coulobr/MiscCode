namespace HitTrax.Bataround
{
	public class GameModeSelectSession : BataroundSession<GameModeSelectSession>
	{
		protected override void LoadSession()
		{
			base.LoadSession();
			OB.DisableEverything();
			BataroundMenuManager.Instance.CloseAllMenus();
			NumPlayersUI.Open();
		}

		public override void UnloadSession()
		{
			base.UnloadSession();
			BataroundMenuManager.Instance.CloseAllMenus();
			//BataroundSessionsManager.Instance.LoadSession<BataroundSplashSession>();
		}
	}
}

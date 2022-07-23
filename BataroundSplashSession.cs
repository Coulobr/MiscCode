namespace HitTrax.Bataround
{
	public class BataroundSplashSession : BataroundSession<BataroundSplashSession>
	{
		protected override void LoadSession()
		{
			base.LoadSession();
			var gameManager = BataroundGameManager.Instance;

			gameManager.CurrentlyInAroundTheWorld = false;
			gameManager.CurrentlyInLinas = false;
			gameManager.CurrentlyInLaserShow = false;
			gameManager.CurrentlyInSmallBall = false;

			gameManager.CurrentBataroundGroup = new BataroundGroup();
			gameManager.NewGame = true;
			gameManager.UsersLoggedIn = false;

			BataroundMenuManager.Instance.CloseAllMenus();
			BataroundLandingUI.Open();
		}

		public override void UnloadSession()
		{
			base.UnloadSession();
			BataroundMenuManager.Instance.CloseAllMenus();
			//BataroundGameManager.Instance.CloseBatAround();
		}
	}
}

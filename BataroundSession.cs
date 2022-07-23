namespace HitTrax.Bataround
{
	public abstract class BataroundSession<T> : BataroundSession where T : BataroundSession<T>
	{
		public static T Instance { get; private set; }

		protected override void Awake()
		{
			Instance = (T)this;
			base.Awake();
		}
	}

	public class BataroundSession : Session
	{
		public virtual bool PlayInProgress { get; set; }
		public bool ShowFielders { get; set; }
		public Play ActivePlay { get; set; }
		public int Score { get; set; }

		public const int MaxPoints = 1000;

		private Objects ob;
		public Objects OB
		{
			get
			{
				if (ob == null)
				{
					ob = Objects.Instance;
				}
				return ob;
			}
		}

		protected override void LoadSession()
		{
			base.LoadSession();

			Objects.Instance.natTrack().OnEnterPlay += EnablePlayInProgress;
			Objects.Instance.fielders().OnPlayReset += DisablePlayInProgress;
		}

		public override void UnloadSession()
		{
			Objects.Instance.natTrack().OnEnterPlay -= EnablePlayInProgress;
			Objects.Instance.fielders().OnPlayReset -= DisablePlayInProgress;
			base.UnloadSession();
		}

		protected void EnablePlayInProgress(Play play = null)
		{
			PlayInProgress = true;
		}

		protected void DisablePlayInProgress(Play play = null)
		{
			PlayInProgress = false;
		}
	}
}
namespace HitTrax.Bataround
{
	public class AverageVeloBySkill
	{
		public static int GetAvgVelo(int level)
		{
			switch (level)
			{
				case GameType.LEVEL_BASEBALL_10U: return 35;
				case GameType.LEVEL_BASEBALL_12U: return 40;
				case GameType.LEVEL_BASEBALL_13U: return 45;
				case GameType.LEVEL_BASEBALL_15U: return 50;
				case GameType.LEVEL_BASEBALL_HIGH_SCHOOL: return 70;
				case GameType.LEVEL_BASEBALL_COLLEGE: return 80;
				case GameType.LEVEL_BASEBALL_MAJOR: return 90;
				default: return 90; //GameType.LEVEL_BASEBALL_MAJOR
			}
		}
	}
}


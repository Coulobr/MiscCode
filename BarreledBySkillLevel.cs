namespace HitTrax.Bataround
{
	public static class BarreledBySkillLevel
	{
		public static int GetBarreledMinimunVel(int skillLevel)
		{
			switch (skillLevel)
			{
				case GameType.LEVEL_BASEBALL_10U: return 40;
				case GameType.LEVEL_BASEBALL_12U: return 50;
				case GameType.LEVEL_BASEBALL_13U: return 70;
				case GameType.LEVEL_BASEBALL_15U: return 75;
				case GameType.LEVEL_BASEBALL_HIGH_SCHOOL: return 80;
				case GameType.LEVEL_BASEBALL_COLLEGE: return 90;
				case GameType.LEVEL_BASEBALL_MAJOR: return 98;
				default: return 98; //GameType.LEVEL_BASEBALL_MAJOR
			}
		}
	}
}

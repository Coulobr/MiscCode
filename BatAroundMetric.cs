namespace HitTrax.Bataround
{
	public class BatAroundMetric
	{       
		// BAM
		public int OverallUserBAM { get; set; } = 0;
		public int BarrelsBAM { get; set; } = 0;
		public int AroundTheWorldBAM { get; set; } = 0;
		public int WalkOffBAM { get; set; } = 0;
		public int LaserShowBAM { get; set; } = 0;
		public int SmallBallBAM { get; set; } = 0;
		public int BataroundBAM { get; set; } = 0;

		// Game Stats		
		public BataroundMinigame.MinigameDifficulty MinigameDifficulty { get; set; }
		
		/// <summary>
		/// Overall score for the experience
		/// </summary>
		public int TotalGameScore { get; set; }
		public int TotalBataroundSessionEXP { get; set; }
		public int TotalBataroundBonusPoints { get; set; }
		public int TotalBataroundBonusHits { get; set; }
		public int TotalBataroundRoundsWon { get; set; }
		public int TotalATWScore { get; set; }
		public int TotalLinasScore { get; set; }
		public int TotalSmallBallScore { get; set; }
		public int TotalLaserShowScore { get; set; }
		
		/// <summary>
		/// Overall score for the Bat Around minigame
		/// </summary>
		public int TotalBataroundMinigameScore { get; set; }
		public int TotalWalkOffScore { get; set; }

		public int GetTotalAllGames()
		{
			return TotalLaserShowScore + TotalWalkOffScore
				+ TotalBataroundMinigameScore + TotalSmallBallScore
				+ TotalLinasScore + TotalATWScore;
		}
	}
}


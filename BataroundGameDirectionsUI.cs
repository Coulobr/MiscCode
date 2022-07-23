using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HitTrax;
using TMPro;

namespace HitTrax.Bataround
{
	public class BataroundGameDirectionsUI : Menu<BataroundGameDirectionsUI>
	{
		public SmartButton playBtn;
		public TextMeshProUGUI directions;
		public TextMeshProUGUI gameTitle;

		public const string LinasDesc = "This challenge will test your ability to consistently get the barrel to the ball by hitting line drives to the center of the field." +
			"\n\n Bonus points are awarded for exceeding exit velocity thresholds that vary by difficulty.";

		public const string ATWDesc = "\nThis challenge will test your batting accuracy - hit two balls to right field, two to center field, and two to left field. " +
			"\n\nComplete all the wedges to earn a bonus hit!";

		public const string LaserShowDesc = "This challenge will push your hitting skills by requiring hits to exceed the target velocity." +
			"\n\nBonus points are awarded by hitting within a percentile of your max velocity from recent minigames.";
		
		public const string SmallBallDesc = "Bat the runner home based on the situation.\n\nBonus points are awarded for getting the batter on-base";

		public const string BatAroundDesc = "A time-based baseball game. Score as many runs as you can before your time runs out!" +
			"\nEach run awards one point and batters change after every hit.";

		public const string WalkOffDesc = "It's the bottom of the 9th, bases are loaded with two outs.\nBe the hero and send your team home a winner!\nAccumulated bonus hits are spent here.";

		protected override void OnOpened()
		{
			base.OnOpened();
			playBtn.onClick.AddListener(OnPlayButtonClick);

			var gameManager = BataroundGameManager.Instance;
			if (gameManager.CurrentlyInLinas)
			{
				directions.text = LinasDesc;
				gameTitle.text = "Liñas";
			}
			else if (gameManager.CurrentlyInAroundTheWorld)
			{
				directions.text = ATWDesc;
				gameTitle.text = "Around the World";
			}
			else if (gameManager.CurrentlyInLaserShow)
			{
				directions.text = LaserShowDesc;
				gameTitle.text = "Laser Show";
			}
			else if (gameManager.CurrentlyInSmallBall)
			{
				directions.text = SmallBallDesc;
				gameTitle.text = "Small Ball";
			}
			else if (gameManager.CurrentlyInWalkOff)
			{
				directions.text = WalkOffDesc;
				gameTitle.text = "Walk Off";
			}
			else if (gameManager.CurrentlyInBataround)
			{
				directions.text = BatAroundDesc;
				gameTitle.text = "Bat Around";
			}
		}

		protected override void OnClosed()
		{
			base.OnClosed();
			playBtn.onClick.RemoveAllListeners();
		}

		private void OnPlayButtonClick()
		{
			var gameManager = BataroundGameManager.Instance;
			
			if (gameManager.CurrentlyInLinas)
			{			
				BarrelsInGameUI.Open();	
			}
			else if (gameManager.CurrentlyInAroundTheWorld)
			{
				AroundTheWorldInGameUI.Open();
			}
			else if (gameManager.CurrentlyInLaserShow)
			{
				LaserShowInGameUI.Open();
			}
			else if (gameManager.CurrentlyInSmallBall)
			{
				SmallBallInGameUI.Open();
			}
			else if (gameManager.CurrentlyInBataround)
			{
				BataroundInGameUI.Open();
			}
			else if (gameManager.CurrentlyInWalkOff)
			{
				WalkOffInGameUI.Open();
			}

			Close();
		}
	}
}

using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HitTrax.Bataround
{
	public class BataroundInGameStatsPanel : Panel<BataroundInGameStatsPanel>
	{
		public TextMeshProUGUI skillText;

		[Header("Exit Velo")] public Transform exitVeloContainer;
		public Image exitVeloResultImg;
		public TextMeshProUGUI exitVeloTitle;
		public TextMeshProUGUI exitVeloValue;
		public TextMeshProUGUI exitVeloRange;
		public Transform exitVeloBonusContainer;
		public TextMeshProUGUI exitVeloBonusValue;

		[Header("Launch Angle")] public Transform launchAngleContainer;
		public Image launchAngleResultImg;
		public TextMeshProUGUI launchAngleTitle;
		public TextMeshProUGUI launchAngleValue;
		public TextMeshProUGUI launchAngleRange;
		public Transform launchAngleBonusContainer;
		public TextMeshProUGUI launchAngleBonusValue;

		[Header("Direction")] public Transform directionContainer;
		public Image directionResultImg;
		public TextMeshProUGUI directionTitle;
		public TextMeshProUGUI directionValue;
		public TextMeshProUGUI directionRange;
		public Transform directionBonusContainer;
		public TextMeshProUGUI directionBonusValue;

		[Header("Distance")] public Transform distanceContainer;
		public Image distanceResultImg;
		public TextMeshProUGUI distanceTitle;
		public TextMeshProUGUI distanceValue;
		public TextMeshProUGUI distanceRange;
		public Transform distanceBonusContainer;
		public TextMeshProUGUI distanceBonusValue;

		public override void SetupPanel()
		{
			base.SetupPanel();

			Objects.Instance.natTrack().OnEnterPlay -= OnEnterPlay;
			Objects.Instance.natTrack().OnEnterPlay += OnEnterPlay;

			var gameManager = BataroundGameManager.Instance;

			if (gameManager.CurrentlyInLinas)
			{
				// Set Skill
				skillText.text = $"Skill: {gameManager.CurrentMinigame.CurrentBatter.BAM.MinigameDifficulty}";

				// Enable & Disable Stat Containers
				exitVeloContainer.gameObject.SetActive(true);
				launchAngleContainer.gameObject.SetActive(true);
				directionContainer.gameObject.SetActive(true);
				distanceContainer.gameObject.SetActive(true);

				// Enable & Disable Bonuses
				exitVeloBonusContainer.gameObject.SetActive(true);
				launchAngleBonusContainer.gameObject.SetActive(false);
				directionBonusContainer.gameObject.SetActive(false);
				distanceBonusContainer.gameObject.SetActive(false);

				// Set Requirements
				exitVeloBonusValue.text = BataroundGameManager.Instance.CurrentMinigame.BonusMphThreshold + "+";
				launchAngleRange.text = $"{BataroundGameManager.Instance.CurrentMinigame.MinLaunchAngle} <color=white> TO </color> {BataroundGameManager.Instance.CurrentMinigame.MaxLaunchAngle}";
				distanceRange.text = $"{BataroundGameManager.Instance.CurrentMinigame.MinDistance}+";
				directionRange.text = $"-15 <color=white> TO </color> 15";
			}
			else if (gameManager.CurrentlyInAroundTheWorld)
			{
				exitVeloContainer.gameObject.SetActive(true);
			}
			else if (gameManager.CurrentlyInLaserShow)
			{
				exitVeloContainer.gameObject.SetActive(true);
			}
			else if (gameManager.CurrentlyInBataround)
			{

			}
			else if (gameManager.CurrentlyInWalkOff)
			{

			}
			else if (gameManager.CurrentlyInSmallBall)
			{

			}
		}

		private void OnEnterPlay(Play play)
		{
			if (exitVeloBonusContainer)
			{
				exitVeloValue.text = Globals.FormatVel(play.exitBallVel.magnitude, false, false);
			}

			if (launchAngleBonusContainer)
			{
				var LA = Mathf.Round(play.elevation * 10.0f) * 0.1f; // round to one decimal
				launchAngleValue.text = LA.ToString();
			}

			if (directionBonusContainer)
			{
				var dir = Mathf.Round(play.horizontalAngle * 10.0f) * 0.1f; // round to one decimal
				directionValue.text = dir.ToString();
			}

			if (distanceBonusContainer)
			{
				var dist = Mathf.Round(play.distance * 10.0f) * 0.1f; // round to one decimal
				distanceValue.text = dist.ToString();
			}
		}
	}
}


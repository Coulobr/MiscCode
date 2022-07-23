using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HitTrax.Bataround
{
	public class SpeedometerPanel : Panel<SpeedometerPanel>
	{
		public const float MAX_VELO = 100; // in mps
		public Image currentHit;
		public Image bonusThreshold;
		public Image pointThreshold;
		public TextMeshProUGUI velocity;
		public TextMeshProUGUI requirementText;
		public TextMeshProUGUI userMaxText;

		public Color bonusColor;
		public Color pointColor;
		public Color failColor;

		public override void SetupPanel()
		{
			base.SetupPanel();	
		}

		protected override void OnOpened()
		{
			base.OnOpened(); 
			Objects.Instance.fielders().OnPlayReset += OnPlayReset;
			BataroundGameManager.Instance.OnHitterChanged += OnHitterChanged;

			ResetPanel();
			SetThresholds();
		}

		protected override void OnClosed()
		{
			base.OnClosed();
			Objects.Instance.fielders().OnPlayReset -= OnPlayReset;
			BataroundGameManager.Instance.OnHitterChanged -= OnHitterChanged;
		}

		public override void UpdatePanel()
		{
			base.UpdatePanel();
			ResetPanel();
			SetThresholds();
		}

		public override void ResetPanel()
		{
			base.ResetPanel();
			currentHit.fillAmount = 0;
			velocity.text = "00";
		}

		public void OnEnterPlay(Play play, bool launchAngleSuccess)
		{
			currentHit.DOFillAmount(CalcHitFillAmount(play), 1.5f).SetEase(Ease.Linear);
			StartCoroutine(Co_VelocityText(play.exitBallVel.magnitude, 1f, play, launchAngleSuccess));
			userMaxText.text = "BEST: " + Globals.FormatVel((float)play.user.stats.maxExitVel, false, false);
		}

		private void OnPlayReset(Play play)
		{
			ResetPanel();
		}

		private void OnHitterChanged(User user)
		{
			UpdatePanel();
		}

		private IEnumerator Co_VelocityText(float endValue, float totalTime, Play play, bool launchAngleSuccess)
		{
			var exitVelo = Globals.MPS2MHP(play.exitBallVel.magnitude);
			var wait = new WaitForSeconds(totalTime / exitVelo);
			float currentValue = 0;

			if (launchAngleSuccess || BataroundGameManager.Instance.CurrentlyInLaserShow)
			{
				ShowResultText(play);
			}

			while (currentValue < exitVelo)
			{
				currentValue += 1f;
				velocity.text = currentValue.ToString();
				yield return wait;
			}

			velocity.text = currentValue.ToString();
		}

		private float CalcHitFillAmount(Play play)
		{
			var mph = Globals.MPS2MHP(play.exitBallVel.magnitude);
			float fill = (mph / MAX_VELO) - 0.1f; // subtract by 0.1 to adjust for the image mask that hides the end 10% of the speedometer
			return fill;
		}

		private void SetThresholds()
		{
			if (BataroundGameManager.Instance.CurrentlyInLaserShow)
			{
				var reqFill = (MAX_VELO - BataroundGameManager.Instance.CurrentMinigame.MinVelocity);
				pointThreshold.fillAmount = (reqFill / 100) + 0.1f; // add .1 to adjust for the image mask that hides the end 10% of the speedometer

				var bonusFill = (MAX_VELO - BataroundGameManager.Instance.CurrentMinigame.BonusMphThreshold);
				bonusThreshold.fillAmount = (bonusFill / 100) + 0.1f; // add .1 to adjust for the image mask that hides the end 10% of the speedometer

				requirementText.text = $"<color=#26C320FF>REQ: {BataroundGameManager.Instance.CurrentMinigame.MinVelocity}</color> | <color=#FFD800FF>BONUS:{BataroundGameManager.Instance.CurrentMinigame.BonusMphThreshold}</color>";
			}
			else if (BataroundGameManager.Instance.CurrentlyInAroundTheWorld)
			{
				bonusThreshold.fillAmount = 0;
				pointThreshold.fillAmount = 0;
			}
			else if (BataroundGameManager.Instance.CurrentlyInLinas)
			{
				var bonusFill = (MAX_VELO - BataroundGameManager.Instance.CurrentMinigame.BonusMphThreshold);
				bonusThreshold.fillAmount = (bonusFill / 100) + 0.1f; // add .1 to adjust for the image mask that hides the end 10% of the speedometer
				pointThreshold.fillAmount = 0;
				requirementText.text = $"<color=#FFD800FF>BONUS: {BataroundGameManager.Instance.CurrentMinigame.BonusMphThreshold}</color>";
			}
		}

		private void ShowResultText(Play play)
		{
			var fill = CalcHitFillAmount(play);

			if ((BataroundGameManager.Instance.CurrentMinigame.MinVelocity == 0 && BataroundGameManager.Instance.CurrentMinigame.BonusMphThreshold == 0) || BataroundGameManager.Instance.CurrentlyInAroundTheWorld)
			{
				return;
			}

			if (fill >= 1 - pointThreshold.fillAmount)
			{

			}
			else if (fill >= 1 - bonusThreshold.fillAmount)
			{

			}
			else if (fill < 1 - pointThreshold.fillAmount && BataroundGameManager.Instance.CurrentlyInLaserShow)
			{
				BataroundGameManager.Instance.OnHitTooSlow?.Invoke();
			}
		}
	}
}

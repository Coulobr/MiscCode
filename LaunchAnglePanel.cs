using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HitTrax.Bataround
{
	public class LaunchAnglePanel : Panel<LaunchAnglePanel>
	{
		public Transform meter;
		public Image minAngleImg;
		public TextMeshProUGUI launchAngle;
		public TextMeshProUGUI requirement;

		public Color bonusColor;
		public Color pointColor;
		public Color failColor;

		protected override void OnOpened()
		{
			base.OnOpened();
			Objects.Instance.fielders().OnPlayReset += OnPlayReset;
			BataroundGameManager.Instance.OnHitterChanged += OnHitterChanged; 
			ResetPanel();
			UpdatePanel();		
		}

		protected override void OnClosed()
		{
			base.OnClosed();
			BataroundGameManager.Instance.OnHitterChanged -= OnHitterChanged;
			Objects.Instance.fielders().OnPlayReset -= OnPlayReset;
		}

		public override void SetupPanel()
		{
			base.SetupPanel();
		}

		public override void ResetPanel()
		{
			base.ResetPanel();
			meter.rotation = Quaternion.Euler(0, 0, 0);
			launchAngle.text = "0<font=BataroundFebroteskExtNoOutline>°";
			SetRequirementText();
		}

		public override void UpdatePanel()
		{
			base.UpdatePanel();
			SetRequiredAngleThreshold();
			SetRequirementText();
		}

		private void SetRequirementText()
		{
			requirement.text = $"Min: {BataroundGameManager.Instance.CurrentMinigame.MinLaunchAngle}<font=BataroundFebroteskExtNoOutline>°</font> " +
				$"| Max: {BataroundGameManager.Instance.CurrentMinigame.MaxLaunchAngle}<font=BataroundFebroteskExtNoOutline>°</font>";
		}

		public void OnEnterPlay(Play play, out bool success)
		{
			StartCoroutine(Co_AngleText(play.elevation, 1.5f));
			meter.DORotate(new Vector3(0, 0, play.elevation), 1.5f).From(Vector3.zero);

			if (play.elevation <= BataroundGameManager.Instance.CurrentMinigame.MaxLaunchAngle && play.elevation >= BataroundGameManager.Instance.CurrentMinigame.MinLaunchAngle)
			{
				success = true;
			}
			else
			{
				success = false;
			}
		}

		private void OnPlayReset(Play play)
		{
			ResetPanel();
		}

		private void OnHitterChanged(User user)
		{
			UpdatePanel();
		}

		private void ShowResultText(float endValue)
		{
			if ((BataroundGameManager.Instance.CurrentMinigame.MinLaunchAngle == 0 && BataroundGameManager.Instance.CurrentMinigame.MaxLaunchAngle == 0) || BataroundGameManager.Instance.CurrentlyInAroundTheWorld)
			{
				return;
			}

			if (endValue <= BataroundGameManager.Instance.CurrentMinigame.MaxLaunchAngle && endValue >= BataroundGameManager.Instance.CurrentMinigame.MinLaunchAngle)
			{
			}
			else if (endValue > BataroundGameManager.Instance.CurrentMinigame.MaxLaunchAngle)
			{
				BataroundGameManager.Instance.OnLATooHigh?.Invoke();
			}
			else if (endValue < BataroundGameManager.Instance.CurrentMinigame.MinLaunchAngle)
			{
				BataroundGameManager.Instance.OnLATooLow?.Invoke();
			}
		}

		private IEnumerator Co_AngleText(float endValue, float totalTime)
		{
			var angle = float.Parse(Globals.FormatInt(endValue));
			var wait = new WaitForSeconds(totalTime / Mathf.Abs(angle));
			float currentValue = 0;
			ShowResultText(endValue);

			if (currentValue > angle)
			{
				while (currentValue > angle)
				{
					currentValue -= 1f;
					this.launchAngle.text = currentValue.ToString() + "<font=BataroundFebroteskExtNoOutline>°";
					yield return wait;
				}
			}
			else
			{
				while (currentValue < angle)
				{
					currentValue += 1f;
					this.launchAngle.text = currentValue.ToString() + "<font=BataroundFebroteskExtNoOutline>°";
					yield return wait;
				}
			}

			this.launchAngle.text = currentValue.ToString() + "<font=BataroundFebroteskExtNoOutline>°";
		}

		private void SetRequiredAngleThreshold()
		{
			if (BataroundGameManager.Instance.CurrentMinigame)
			{
				var maxAngle = BataroundGameManager.Instance.CurrentMinigame.MaxLaunchAngle;
				var minAngle = BataroundGameManager.Instance.CurrentMinigame.MinLaunchAngle;

				var percentageOf90 = ((maxAngle - minAngle) / 90f);
				minAngleImg.fillAmount = percentageOf90 * 0.25f; // Set fill threshhold
				minAngleImg.transform.rotation = Quaternion.Euler(0, 0, maxAngle - 90); // rotate to set max angle
			}	
		}
	}
}

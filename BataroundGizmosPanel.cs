using System;
using TMPro;
using UnityEngine;

namespace HitTrax.Bataround
{
	public class BataroundGizmosPanel : Panel<BataroundGizmosPanel>
	{
		private SpeedometerPanel SpeedometerPanel { get; set; }
		private LaunchAnglePanel LaunchAnglePanel { get; set; }
		private BataroundSprayChartPanel SprayChartPanel { get; set; }
		private BaseDisplayPanel BaseDisplayPanel { get; set; }

		public Transform layoutGroup;

		public override void SetupPanel()
		{
			base.SetupPanel();

			if (SpeedometerPanel == null)
			{
				SpeedometerPanel = SpeedometerPanel.Create(layoutGroup);
				SpeedometerPanel.SetupPanel();
			}

			if (LaunchAnglePanel == null)
			{
				LaunchAnglePanel = LaunchAnglePanel.Create(layoutGroup);
				LaunchAnglePanel.SetupPanel();
			}

			if (SprayChartPanel == null)
			{
				SprayChartPanel = BataroundSprayChartPanel.Create(layoutGroup);
				SprayChartPanel.SetupPanel();
			}

			if (BaseDisplayPanel == null)
			{
				BaseDisplayPanel = BaseDisplayPanel.Create(layoutGroup);
				BaseDisplayPanel.SetupPanel();
			}
		}

		protected override void OnOpened()
		{
			base.OnOpened();

			Objects.Instance.natTrack().OnEnterPlay += OnEnterPlay;
			Objects.Instance.fielders().OnPlayReset += OnPlayReset;
			BataroundGameManager.Instance.OnHitterChanged += OnChangeHitter;
			BataroundGameManager.Instance.OnEndMinigame += OnEndMinigame;

			if (BataroundGameManager.Instance.CurrentlyInLinas)
			{
				SpeedometerPanel.Open();		
				LaunchAnglePanel.Open();
				SprayChartPanel.Open();		
			}
			else if (BataroundGameManager.Instance.CurrentlyInAroundTheWorld)
			{
				SpeedometerPanel.Close();
				LaunchAnglePanel.Close();
				SprayChartPanel.Open();
			}
			else if (BataroundGameManager.Instance.CurrentlyInLaserShow)
			{
				SpeedometerPanel.Open();
				LaunchAnglePanel.Close();
				SprayChartPanel.Close();
			}
			else if (BataroundGameManager.Instance.CurrentlyInSmallBall)
			{
				SpeedometerPanel.Close();
				LaunchAnglePanel.Close();
				SprayChartPanel.Close();
				BaseDisplayPanel.Open();
			}
			else if (BataroundGameManager.Instance.CurrentlyInBataround)
			{
				SpeedometerPanel.Close();
				LaunchAnglePanel.Close();
				SprayChartPanel.Close();
				BaseDisplayPanel.Open();
			}
			else if (BataroundGameManager.Instance.CurrentlyInWalkOff)
			{
				SpeedometerPanel.Close();
				LaunchAnglePanel.Close();
				SprayChartPanel.Close();
				BaseDisplayPanel.Open();
			}
		}

		protected override void OnClosed()
		{
			base.OnClosed();

			BataroundGameManager.Instance.OnHitterChanged -= OnChangeHitter;
			Objects.Instance.natTrack().OnEnterPlay -= OnEnterPlay;
			BataroundGameManager.Instance.OnEndMinigame -= OnEndMinigame;

			SpeedometerPanel.Close();
			LaunchAnglePanel.Close();
			SprayChartPanel.Close();
			BaseDisplayPanel.Close();
		}


		private void OnEnterPlay(Play play)
		{
			if (transform.parent.gameObject.activeSelf)
			{
				bool laSuccess = false;

				if (LaunchAnglePanel.isOpen)
				{
					LaunchAnglePanel?.OnEnterPlay(play, out laSuccess);
				}

				if (SpeedometerPanel.isOpen)
				{
					SpeedometerPanel?.OnEnterPlay(play, laSuccess);
				}
			}
		}

		private void OnPlayReset(Play play)
		{
			//if (BaseDisplayPanel.isOpen)
			//{
			//	BaseDisplayPanel?.OnPlayReset(play);
			//}
		}

		private void OnEndMinigame(BataroundMinigame obj)
		{
			Close();
		}

		private void OnChangeHitter(User user)
		{
			UpdatePanel();
		}
	}
}


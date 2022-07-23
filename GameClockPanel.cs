using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HitTrax;
using TMPro;
using System;

namespace HitTrax.Bataround
{
	public class GameClockPanel : Panel<GameClockPanel> 
	{
		public BataroundMinigame Minigame { get; set; }
		private int timeRemaining = 0;

		public TextMeshProUGUI minutesText;
		public TextMeshProUGUI secondsText;

		protected int minutes;
		protected int seconds;

		private IEnumerator countdownRoutine;
		private bool paused;

		public static Action OnTimerEnd;

		protected override void OnOpened()
		{
			base.OnOpened();

			Minigame = BataroundGameManager.Instance.CurrentMinigame;
			ResetPanel();

			if (BataroundGameManager.Instance.CurrentlyInBataround) 
			{
				BataroundGameManager.Instance.OnPlayerTurnStart += StartClock;
				BataroundGameManager.Instance.OnHitterChanged += OnHitterChanged;
				BataroundGameManager.Instance.OnUndoLastPlay += OnUndoLastPlay;
				Minigame.ThisGame.OnInningHalfFinished += ResetPanel;
				Objects.Instance.natTrack().OnEnterPlay += OnEnterPlay;
				Objects.Instance.fielders().OnPlayReset += OnPlayReset;
			}
		}

		protected override void OnClosed()
		{
			base.OnClosed();

			if (BataroundGameManager.Instance.CurrentlyInBataround)
			{
				BataroundGameManager.Instance.OnPlayerTurnStart -= StartClock;
				BataroundGameManager.Instance.OnHitterChanged -= OnHitterChanged;
				BataroundGameManager.Instance.OnUndoLastPlay -= OnUndoLastPlay;
				Minigame.ThisGame.OnInningHalfFinished -= ResetPanel;
				Objects.Instance.natTrack().OnEnterPlay -= OnEnterPlay;
			}
		}

		private void OnUndoLastPlay()
		{
			timeRemaining += 10;
		}

		private void OnPlayReset(Play obj)
		{
			ResumeClock();
		}

		private void OnEnterPlay(Play obj)
		{
			PauseClock();
		}

		private void OnHitterChanged(User user)
		{
			if (BataroundGameManager.Instance.IsFreeForAll)
			{
				ResetPanel();
			}
		}

		

		public void StartClock()
		{ 
			if (countdownRoutine != null)
			{
				paused = false;
				return;
			}

			countdownRoutine = null;
			countdownRoutine = Co_Countdown();
			StartCoroutine(countdownRoutine);
		}

		public void PauseClock()
		{
			paused = true;
		}
		public void ResumeClock()
		{
			paused = false;
		}

		public override void ResetPanel()
		{
			base.ResetPanel();

			if (countdownRoutine != null)
			{
				StopCoroutine(countdownRoutine);
				countdownRoutine = null;
			}

			timeRemaining = BataroundGameManager.Instance.IsFreeForAll ? Minigame.FFAGameTime : Minigame.TeamGameTime;
			UpdateTimer(timeRemaining);
		}

		private IEnumerator Co_Countdown()
		{
			var wait = new WaitForSeconds(1);

			while (timeRemaining > 0)
			{
				while (paused)
				{
					yield return new WaitForEndOfFrame();
				}

				timeRemaining -= 1;
				UpdateTimer(timeRemaining);
				yield return wait;
			}

			OnTimerEnd?.Invoke();
			countdownRoutine = null;
		}

		public void UpdateTimer(float timeLeft)
		{
			try
			{
				// Set up the numbers for the countdown itself
				minutes = (int)(timeLeft / 60f);
				seconds = (int)(timeLeft % 60f);

				minutesText.text = minutes.ToString("0");
				secondsText.text = seconds.ToString("00");
			}
			catch (Exception e)
			{
				Debug.LogError("Could not update the bataround game timer... \n" + e);
			}
		}
	}
}

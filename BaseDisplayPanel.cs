using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HitTrax;
using System;

namespace HitTrax.Bataround
{
	public class BaseDisplayPanel : Panel<BaseDisplayPanel>
	{
		public Suite.TeamBattleBaseDisplay gameBaseDisplay;

		protected override void OnOpened()
		{
			base.OnOpened();
			BataroundGameManager.Instance.OnHitterChanged += OnHitterChanged;
			BataroundGameManager.Instance.OnUndoLastPlay += UndoLastPlay;
			BataroundGameManager.Instance.OnUpdateBaseDisplayPanel += UpdatePanel;

			if (BataroundGameManager.Instance.CurrentMinigame.ThisGame != null)
			{
				gameBaseDisplay.UpdateElement(BataroundGameManager.Instance.CurrentMinigame.ThisGame);
			}
		}

		protected override void OnClosed()
		{
			base.OnClosed();
			BataroundGameManager.Instance.OnHitterChanged -= OnHitterChanged;
			BataroundGameManager.Instance.OnUndoLastPlay -= UndoLastPlay;
			BataroundGameManager.Instance.OnUpdateBaseDisplayPanel -= UpdatePanel;
		}

		private void UndoLastPlay()
		{
			var game = BataroundGameManager.Instance.CurrentMinigame.ThisGame;
			gameBaseDisplay.UpdateElement(game);
		}

		private void OnHitterChanged(User user)
		{
			var game = BataroundGameManager.Instance.CurrentMinigame.ThisGame;
			gameBaseDisplay.UpdateElement(game);
		}

		public void OnPlayReset(Play play)
		{
			var game = BataroundGameManager.Instance.CurrentMinigame.ThisGame;
			if (play != null && game != null)
			{
				if (BataroundGameManager.Instance.CurrentlyInWalkOff)
				{
					game.currentHalf.bases[1] = 1;
					game.currentHalf.bases[2] = 1;
					game.currentHalf.bases[3] = 1;
				}

				gameBaseDisplay.UpdateElement(game);
			}
		}

		public void UpdatePanel()
		{
			var game = BataroundGameManager.Instance.CurrentMinigame.ThisGame;

			if (game != null)
			{
				if (BataroundGameManager.Instance.CurrentlyInWalkOff)
				{
					game.currentHalf.bases[1] = 1;
					game.currentHalf.bases[2] = 1;
					game.currentHalf.bases[3] = 1;
				}

				gameBaseDisplay.UpdateElement(game);
			}
		}
	}
}

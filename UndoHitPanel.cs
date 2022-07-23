using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using HitTrax;
using UnityEngine.UI;
using System;

namespace HitTrax.Bataround
{
	public class UndoHitPanel : Panel<UndoHitPanel>
	{
		public Button undoBtn;
		private bool canUndo;

		public override void SetupPanel()
		{
			base.SetupPanel();

			if (undoBtn)
			{
				undoBtn.onClick.RemoveAllListeners();
				undoBtn.onClick.AddListener(OnClick);
			}

			Objects.Instance.natTrack().OnEnterPlay -= OnEnterPlay;
			Objects.Instance.natTrack().OnEnterPlay += OnEnterPlay;
		}

		private void OnEnterPlay(Play play)
		{
			if (play != null)
			{
				canUndo = true;
			}
		}

		private void OnClick()
		{
			BataroundGameManager.Instance.RemoveLastPlay();
			canUndo = false;
		}
	}
}

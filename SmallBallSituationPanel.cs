using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HitTrax;
using DG.Tweening;
using System;
using TMPro;

namespace HitTrax.Bataround
{
	public class SmallBallSituationPanel : Panel<SmallBallSituationPanel>
	{
		public TextMeshProUGUI description;
		public override void SetupPanel()
		{
			base.SetupPanel();
			SmallBallMinigame.SituationChanged -= OnSituationChanged;
			SmallBallMinigame.SituationChanged += OnSituationChanged;
		}

		private void OnSituationChanged(HittingDrill drill)
		{
			description.text = drill.description;
			//StartCoroutine(Animate());
		}

		private IEnumerator Animate()
		{
			gameObject.SetActive(true);
			var rectTransform = GetComponent<RectTransform>();

			rectTransform.DOAnchorPosY(-50, 1.33f);
			yield return new WaitForSeconds(4.5f);
			rectTransform.DOAnchorPosY(50, 1.33f);
		}
	}
}

using DG.Tweening;
using System;
using System.Collections;
using UnityEngine.UI;

namespace HitTrax.Bataround
{
	public class FadeTransitionUI : Menu<FadeTransitionUI>
	{
		public Image fadeImg;

		protected override void OnOpened()
		{
			base.OnOpened();
		}

		public void Transition(float inTime, float outTime, Action onComplete)
		{
			StartCoroutine(Co_Transition(inTime, outTime, onComplete));
		}

		private IEnumerator Co_Transition(float inTime, float outTime, Action onComplete)
		{
			fadeImg.DOFade(1, inTime).From(0)
				.OnComplete(() =>
				{
					onComplete?.Invoke();
					fadeImg.DOFade(0, outTime).From(1);
				});

			yield return null;
		}
	}
}

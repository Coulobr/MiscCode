using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HitTrax.Bataround
{
	public class BattingOrderPoolable : PoolableObject
	{
		private User User { get; set; }

		private RectTransform rectTransform;
		private RectTransform RectTransform
		{
			get
			{
				if (rectTransform == null)
				{
					rectTransform = GetComponent<RectTransform>();
				}
				return rectTransform;
			}
		}

		public TextMeshProUGUI name;
		public Image teamColor;
		public Image outline;
		private IEnumerator outlineBlinkRoutine;

		public override void SetupElement(ObjectPooler desiredParentObjectPooler)
		{
			base.SetupElement(desiredParentObjectPooler);

			BataroundGameManager.Instance.OnHitterChanged -= OnHitterChanged;
			BataroundGameManager.Instance.OnHitterChanged += OnHitterChanged;

			outlineBlinkRoutine = OutlineBlinkRoutine();
		}

		private void OnHitterChanged(User user)
		{
			if (User == user)
			{
				DOTween.To(() => GetComponent<LayoutElement>().preferredWidth, x => GetComponent<LayoutElement>().preferredWidth = x, 275f, .5f).OnComplete(() =>
				{
					StartCoroutine(outlineBlinkRoutine);
				});
			}
			else
			{
				if (GetComponent<LayoutElement>().preferredWidth != 250f)
				{
					DOTween.To(() => GetComponent<LayoutElement>().preferredWidth, x => GetComponent<LayoutElement>().preferredWidth = x, 250f, .5f);
					outlineBlinkRoutine = null;
					outlineBlinkRoutine = OutlineBlinkRoutine();
				}
			}
		}

		public void InitializeElement(User user)
		{
			User = user;
			this.name.text = user.screenName;

			if (user.BataroundTeam == 0)
			{
				teamColor.color = BataroundGameManager.BataroundTeamBlue;
			}
			else
			{
				teamColor.color = BataroundGameManager.BataroundTeamRed;
			}

			// Set first hitter as OnHitterChanged action is first called before this poolable is setup
			OnHitterChanged(BataroundGameManager.Instance.CurrentMinigame.CurrentBatter);
		}

		private IEnumerator OutlineBlinkRoutine()
		{
			var wait = new WaitForSeconds(2f);
			outline.enabled = true;

			while (outlineBlinkRoutine != null)
			{
				outline.DOFade(1, 1f).From(0).SetEase(Ease.InQuad)
					.OnComplete(() => outline.DOFade(0, 1f).From(1).SetEase(Ease.Linear));

				yield return wait;
			}
		}
	}
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HitTrax.Bataround
{
	public class BataroundPopupTextHandler : MonoBehaviour
	{
		public GameObject popupTemplate;
		public bool inPopup = false;
		public List<IEnumerator> popupQueue = new List<IEnumerator>();

		private void Start()
		{
			BataroundGameManager.Instance.OnEndMinigame -= OnEndMinigame;
			BataroundGameManager.Instance.OnEndMinigame += OnEndMinigame;
			BataroundGameManager.Instance.OnBonusProgress -= OnBonus;
			BataroundGameManager.Instance.OnBonusProgress += OnBonus;
			BataroundGameManager.Instance.OnSuccess -= OnSuccess;
			BataroundGameManager.Instance.OnSuccess += OnSuccess;
			BataroundGameManager.Instance.OnHitTooShort -= OnHitTooShort;
			BataroundGameManager.Instance.OnHitTooShort += OnHitTooShort;
			BataroundGameManager.Instance.OnLATooHigh -= OnLATooHigh;
			BataroundGameManager.Instance.OnLATooHigh += OnLATooHigh;
			BataroundGameManager.Instance.OnLATooLow -= OnLATooLow;
			BataroundGameManager.Instance.OnLATooLow += OnLATooLow;
			BataroundGameManager.Instance.OnHitTooSlow -= OnHitTooSlow;
			BataroundGameManager.Instance.OnHitTooSlow += OnHitTooSlow;
		}
			
		/// <summary>
		/// Checks the queue and displays a popup
		/// </summary>
		private void Update()
		{
			try
			{
				if (popupQueue.Count == 0)
				{
					return;
				}

				if (popupQueue.Count > 0 && !inPopup)
				{
					StartCoroutine(popupQueue[0]);
				}
			}
			catch (Exception e)
			{
				Utility.LogError(e);
			}
		}

		private void OnEndMinigame(BataroundMinigame minigame)
		{
			popupQueue.Clear();
		}

		private void OnLATooLow()
		{
			Popup("Too Low", blink: false, failed: true);
		}

		private void OnLATooHigh()
		{
			Popup("Too High", blink: false, failed: true);
		}

		private void OnHitTooShort()
		{
			Popup("Too Short", blink: false, failed: true);
		}

		private void OnHitTooSlow()
		{
			Popup("Too Slow", blink: false, failed: true);
		}

		private void OnSuccess()
		{
			Popup("Great!", blink: true, failed: false);
		}

		private void OnBonus()
		{
			if (!BataroundGameManager.Instance.CurrentlyInAroundTheWorld)
			{
				Popup("Bonus!", blink: true, failed: false);
				return;
			}

			if (BataroundGameManager.Instance.CurrentMinigame.CurrentBatter.BAM.TotalBataroundBonusPoints == 6)
			{
				Popup("Bonus!", blink: true, failed: false);
			}
		}


		public void Popup(string msg, bool blink, bool failed)
		{
			popupQueue.Add(Co_Popup(msg, blink, failed));
		}

		private IEnumerator Co_Popup(string msg, bool blink, bool failed)
		{
			var popup = Instantiate(popupTemplate);
			popup.GetComponent<BataroundPopup>().Initiate(msg, blink, this, failed);

			inPopup = true;
			while (inPopup)
			{
				yield return new WaitForEndOfFrame();
			}

			if (popupQueue != null && popupQueue.Count > 0)
			{
				popupQueue.RemoveAt(0);
			}

			inPopup = false;
		}
	}
}

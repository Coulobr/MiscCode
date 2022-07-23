using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HitTrax.Bataround
{
	public class BataroundBonusPanel : Panel<BataroundBonusPanel>
	{
		public ObjectPooler pooler;
		public TextMeshProUGUI bonusText;
		public Image bonusCompleteImg;
		//public Color bonusInProgressColor;
		//public Color bonusCompleteColor;
		public Sprite filledSprite;
		public Sprite emptySprite;
		public bool bonusFilled;

		private BataroundGameManager gameManager;
		public BataroundGameManager GameManager
		{
			get
			{
				if (gameManager == null)
				{
					gameManager = BataroundGameManager.Instance;
				}

				return gameManager;
			}
		}

		protected override void OnOpened()
		{
			base.OnOpened();
			GameManager.OnBonusProgress += UpdatePanel;
			GameManager.OnHitterChanged += OnHitterChanged;
			GameManager.OnEndMinigame += OnEndMinigame;
			GameManager.OnUndoLastPlay += OnUndoLastPlay;
			ResetPanel();
		}

		protected override void OnClosed()
		{
			base.OnClosed();
			GameManager.OnBonusProgress -= UpdatePanel;
			GameManager.OnHitterChanged -= OnHitterChanged;
			GameManager.OnEndMinigame -= OnEndMinigame;
			GameManager.OnUndoLastPlay -= OnUndoLastPlay;
		}

		public override void SetupPanel()
		{
			base.SetupPanel();
		}

		public override void ResetPanel()
		{
			base.ResetPanel();
			SetupTemplateBars();

			bonusText.DOScale(1, 0);
			bonusText.text = "Bonus";
			bonusCompleteImg.sprite = emptySprite;
			bonusFilled = false;

			// reset color to yellow
			for (int i = 0; i < pooler.usedContainer.childCount; i++)
			{
				pooler.usedContainer.GetChild(i).GetComponent<Image>().sprite = emptySprite;
				//pooler.usedContainer.GetChild(i).GetComponent<Image>().enabled = false;
			}
		}

		private void OnHitterChanged(User obj)
		{
			ResetPanel();
		}

		private void OnUndoLastPlay()
		{
			UpdatePanel();
		}

		public override void UpdatePanel()
		{
			base.UpdatePanel();
			ResetPanel();

			for (int i = 0; i < gameManager.CurrentMinigame.CurrentBatter.BAM.TotalBataroundBonusPoints; i++)
			{
				pooler.usedContainer.GetChild(i).GetComponent<Image>().sprite = filledSprite;
				
				if (i == pooler.usedContainer.childCount - 1)
				{
					BonusFilled();
				}

				//for (int j = 0; j < pooler.usedContainer.childCount; j++)
				//{
				//	pooler.usedContainer.GetChild(i).GetComponent<Image>().sprite = emptySprite;
				//	//bonusText.DOFontSize(58f, .5f)
				//	//	.OnComplete(() => bonusText.DOFontSize(50f, .5f));

				//	break;
				//	//if (!pooler.usedContainer.GetChild(j).GetComponent<Image>().IsActive())
				//	//{

				//	//}
				//}
			}		
		}

		private void OnEndMinigame(BataroundMinigame obj)
		{
			Close();
		}

		private void SetupTemplateBars()
		{
			pooler.ResetElement();
			pooler.SetupElement();

			if (GameManager.CurrentlyInLinas)
			{
				for (int i = 0; i < 4; i++)
				{
					var obj = pooler.ActivatePoolableObject(pooler.GetFreeObject());
				}
			}
			else if (GameManager.CurrentlyInAroundTheWorld)
			{
				for (int i = 0; i < 6; i++)
				{
					var obj = pooler.ActivatePoolableObject(pooler.GetFreeObject());
				}
			}
			else if (GameManager.CurrentlyInLaserShow)
			{
				for (int i = 0; i < 4; i++)
				{
					var obj = pooler.ActivatePoolableObject(pooler.GetFreeObject());
				}
			}
			else if (GameManager.CurrentlyInSmallBall)
			{
				for (int i = 0; i < 4; i++)
				{
					var obj = pooler.ActivatePoolableObject(pooler.GetFreeObject());
				}
			}
		}	

		private void BonusFilled()
		{
			bonusFilled = true;
			bonusCompleteImg.sprite = filledSprite;

			//for (int i = 0; i < pooler.usedContainer.childCount; i++)
			//{
			//	pooler.usedContainer.GetChild(i).GetComponent<Image>().color = bonusCompleteColor;
			//}

			//bonusText.text = "Complete!";
			//bonusText.DOFontSize(65f, .5f)
			//			.OnComplete(() => bonusText.DOFontSize(55f, .5f));
		}
	}
}

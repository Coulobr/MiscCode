using System;
using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

namespace HitTrax.Bataround
{
	public class FFALeaderboardPanel : Panel<FFALeaderboardPanel>
	{
		public FFALeaderboardUserList userList;
		public Transform columnContainer;
		public TextMeshProUGUI columnTextTemplate;

		public TextMeshProUGUI gameModeTitle;
		public TextMeshProUGUI description;
		public RectTransform descriptionContainer;
		public RectTransform gameModeContainer;
		public Button gameModeBtn;


		[HideInInspector] public bool listGenerated;

		protected override void OnOpened()
		{
			base.OnOpened(); 

			var gameManager = BataroundGameManager.Instance;
			Objects.Instance.natTrack().OnEnterPlay += OnEnterPlay;
			Objects.Instance.fielders().OnPlayReset += OnPlayReset;
			gameManager.OnHitterChanged += OnHitterChanged;
			gameManager.OnPlayerTurnStart += OnPlayerTurnStart;
			gameManager.OnEndMinigame += OnEndMinigame;
			gameManager.OnUndoLastPlay += OnUndoLastPlay;
			gameModeBtn.onClick.AddListener(OnClickDropdown);

			ResetPanel();
			AddDataSetColumnsHeaders();

			if (!listGenerated)
			{
				GenerateList();
			}

			SetGameModeName();
			SetGameDescription();
			gameModeContainer.SetAsLastSibling();
			ShowDescription();

			if (!BataroundGameManager.Instance.IsFreeForAll)
			{
				Close();
			}
		}



		protected override void OnClosed()
		{
			base.OnClosed(); 

			var gameManager = BataroundGameManager.Instance;
			Objects.Instance.fielders().OnPlayReset -= OnPlayReset;
			gameManager.OnHitterChanged -= OnHitterChanged;
			Objects.Instance.natTrack().OnEnterPlay -= OnEnterPlay;
			gameManager.OnPlayerTurnStart -= OnPlayerTurnStart;
			gameManager.OnEndMinigame -= OnEndMinigame;
			gameManager.OnUndoLastPlay -= OnUndoLastPlay;
			gameModeBtn.onClick.RemoveAllListeners();

			RemoveAllDataSetColumnHeaders();
		}

		public override void SetupPanel()
		{
			base.SetupPanel();
		
			if (userList)
			{
				userList.SetupElement();
				userList.ResetElement();
			}

			//ResetPanel();
			//RefreshPanel();
		}

		public override void ResetPanel()
		{
			base.ResetPanel();
			SetGameModeName();
			SetGameDescription();
			gameModeContainer.transform.SetAsLastSibling();

			userList.ResetElement();
			listGenerated = false;

			if (!BataroundGameManager.Instance.IsFreeForAll)
			{
				gameObject.SetActive(false);
			}
		}

		public override void RefreshPanel()
		{
			base.RefreshPanel();
			GenerateList();
			SetGameModeName();
			SetGameDescription();
			gameModeContainer.transform.SetAsLastSibling();
		}

		private void GenerateList()
		{
			try
			{
				userList.BuildList(() =>
				{
					AddDataSetColumns();
					listGenerated = true;
				});
			}
			catch (Exception e)
			{
				Utility.LogError(e, GetType());
			}
		}

		private void OnUndoLastPlay()
		{
			RefreshPanel();
		}

		private void OnEndMinigame(BataroundMinigame obj)
		{
			Close();
		}

		private void OnEnterPlay(Play play)
		{
			if (descriptionContainer.anchoredPosition.y > 0)
			{
				HideDescription();
			}
		}

		private void OnPlayReset(Play play)
		{
			RefreshPanel();
		}

		private void OnPlayerTurnStart()
		{
			HideDescription();
		}

		private void OnHitterChanged(User user)
		{
			SetGameDescription();
			ShowDescription();
		}

		private void ShowDescription()
		{
			descriptionContainer.DOAnchorPosY(-47f, .66f);
		}
		
		private void HideDescription()
		{
			descriptionContainer.DOAnchorPosY(0f, .66f);
		}

		private void OnClickDropdown()
		{
			SetGameDescription();
			if (descriptionContainer.anchoredPosition.y < 0)
			{
				HideDescription();
			}
			else
			{
				ShowDescription();
			}
		}

		private void SetGameDescription()
		{
			var gameManager = BataroundGameManager.Instance;

			if (gameManager.CurrentlyInLinas)
			{
				description.text = "Hit Line Drives";
			}
			else if (gameManager.CurrentlyInAroundTheWorld)
			{
				description.text = "Hit in the Wedges";
			}
			else if (gameManager.CurrentlyInLaserShow)
			{
				description.text = "Hit it Hard";
			}
			else if (gameManager.CurrentlyInSmallBall)
			{
				description.text = "Bat the Runner Home";
			}
			else if (gameManager.CurrentlyInBataround)
			{
				description.text = "Score Runs";
			}
			else if (gameManager.CurrentlyInWalkOff)
			{
				description.text = "Score a Run";
			}
		}

		private void SetGameModeName()
		{
			var gameManager = BataroundGameManager.Instance;

			if (gameManager.CurrentlyInLinas)
			{
				gameModeTitle.text = "Liñas";
			}
			else if (gameManager.CurrentlyInAroundTheWorld)
			{
				gameModeTitle.text = "Around the World";
			}
			else if (gameManager.CurrentlyInLaserShow)
			{
				gameModeTitle.text = "Laser Show";
			}
			else if (gameManager.CurrentlyInSmallBall)
			{
				gameModeTitle.text = "Small Ball";
			}
			else if (gameManager.CurrentlyInBataround)
			{
				gameModeTitle.text = "Bat Around";
			}
			else if (gameManager.CurrentlyInWalkOff)
			{
				gameModeTitle.text = "Walk Off";
			}
		}

		private void AddDataSetColumns()
		{
			foreach (var item in userList.usedProListItems)
			{
				item.AddColumn("N/A", Color.white, null, null, null, 50);
				item.gameObject.SetActive(true);
			}
		}

		private void RemoveAllDataSetColumnHeaders()
		{
			userList.RemoveAllColumnHeaders();
		}

		private void AddDataSetColumnsHeaders()
		{
			userList.AddColumnHeader(ProColumnHeaderData.Generate("Player", Color.white, ColumnHeaderType.TextOnly)
				.SetCustomSize(100f, 50f)
				.AddTitleTextTemplate(columnTextTemplate)
				.SetCustomAlignment(TextAlignmentOptions.Left)
				.Construct(columnContainer, userList));

			userList.AddColumnHeader(ProColumnHeaderData.Generate("Order", Color.white, ColumnHeaderType.TextOnly)
				.SetCustomSize(60f, 50f)
				.AddTitleTextTemplate(columnTextTemplate)
				.SetCustomAlignment(TextAlignmentOptions.Center)
				.Construct(columnContainer, userList));

			userList.AddColumnHeader(ProColumnHeaderData.Generate("Score", Color.white, ColumnHeaderType.TextOnly)
				.SetCustomSize(60f, 50f)
				.AddTitleTextTemplate(columnTextTemplate)
				.SetCustomAlignment(TextAlignmentOptions.Center)
				.Construct(columnContainer, userList));

			userList.AddColumnHeader(ProColumnHeaderData.Generate("Total", new Color(1, 216 / 255f, 0), ColumnHeaderType.TextOnly)
				.SetCustomSize(60f, 50f)
				.AddTitleTextTemplate(columnTextTemplate)
				.SetCustomAlignment(TextAlignmentOptions.Center)
				.Construct(columnContainer, userList));
		}
	}
}


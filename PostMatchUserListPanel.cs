using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using HitTrax.Pro;

namespace HitTrax.Bataround
{
	public class PostMatchUserListPanel : Panel<PostMatchUserListPanel>
	{
		public PostMatchUserList userList;
		public Transform columnContainer;
		public TextMeshProUGUI columnTextTemplate;

		[HideInInspector] public bool listGenerated;

		protected override void OnOpened()
		{
			base.OnOpened();
			AddDataSetColumnsHeaders();
			SetupPanel();

			if (!listGenerated)
			{
				GenerateList();
			}
		}

		protected override void OnClosed()
		{
			base.OnClosed();
			RemoveAllDataSetColumnHeaders();
		}

		public override void SetupPanel()
		{
			base.SetupPanel();

			if (userList)
			{
				userList.SetupElement();
				userList.ResetElement();
				userList.OnProListItemToggled += OnProListItemToggled;
			}
		}

		private void OnProListItemToggled(ProListItem item, bool newState, List<ProListItem> selectedPlaysList)
		{
			
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
				.SetCustomSize(250f, 50f)
				//.AddSubtitleTextTemplate(columnTextTemplate)
				.AddTitleTextTemplate(columnTextTemplate)
				.SetCustomAlignment(TextAlignmentOptions.Left)
				.Construct(columnContainer, userList));

			//userList.AddColumnHeader(ProColumnHeaderData.Generate("Order", Color.white, ColumnHeaderType.TextOnly)
			//   .SetCustomSize(250f, 50f)
			//   //.AddSubtitleTextTemplate(columnTextTemplate)
			//   .AddTitleTextTemplate(columnTextTemplate)
			//   .SetCustomAlignment(TextAlignmentOptions.Center)
			//   .Construct(columnContainer, userList));

			userList.AddColumnHeader(ProColumnHeaderData.Generate("Liñas", BataroundGameManager.Instance.CurrentlyInLinas ? new Color(66 / 255f, 208 / 255f, 120 / 255f) : Color.white, ColumnHeaderType.TextOnly)
				.SetCustomSize(250f, 50f)
				// .AddSubtitleTextTemplate(columnTextTemplate)
				.AddTitleTextTemplate(columnTextTemplate)
				.SetCustomAlignment(TextAlignmentOptions.Center)
				.Construct(columnContainer, userList));

			userList.AddColumnHeader(ProColumnHeaderData.Generate("ATW", BataroundGameManager.Instance.CurrentlyInAroundTheWorld ? new Color(66 / 255f, 208 / 255f, 120 / 255f) : Color.white, ColumnHeaderType.TextOnly)
				.SetCustomSize(250f, 50f)
				//  .AddSubtitleTextTemplate(columnTextTemplate)
				.AddTitleTextTemplate(columnTextTemplate)
				.SetCustomAlignment(TextAlignmentOptions.Center)
				.Construct(columnContainer, userList));

			userList.AddColumnHeader(ProColumnHeaderData.Generate("Small Ball", BataroundGameManager.Instance.CurrentlyInSmallBall ? new Color(66 / 255f, 208 / 255f, 120 / 255f) : Color.white, ColumnHeaderType.TextOnly)
				.SetCustomSize(250f, 50f)
				// .AddSubtitleTextTemplate(columnTextTemplate)
				.AddTitleTextTemplate(columnTextTemplate)
				.SetCustomAlignment(TextAlignmentOptions.Center)
				.Construct(columnContainer, userList));

			userList.AddColumnHeader(ProColumnHeaderData.Generate("LS", BataroundGameManager.Instance.CurrentlyInLaserShow ? new Color(66 / 255f, 208 / 255f, 120 / 255f) : Color.white, ColumnHeaderType.TextOnly)
				.SetCustomSize(250f, 50f)
				// .AddSubtitleTextTemplate(columnTextTemplate)
				.AddTitleTextTemplate(columnTextTemplate)
				.SetCustomAlignment(TextAlignmentOptions.Center)
				.Construct(columnContainer, userList));

			userList.AddColumnHeader(ProColumnHeaderData.Generate("Walk Off", Color.white, ColumnHeaderType.TextOnly)
				.SetCustomSize(250f, 50f)
				// .AddSubtitleTextTemplate(columnTextTemplate)
				.AddTitleTextTemplate(columnTextTemplate)
				.SetCustomAlignment(TextAlignmentOptions.Center)
				.Construct(columnContainer, userList));

			userList.AddColumnHeader(ProColumnHeaderData.Generate("Bat Around", Color.white, ColumnHeaderType.TextOnly)
				.SetCustomSize(250f, 50f)
				// .AddSubtitleTextTemplate(columnTextTemplate)
				.AddTitleTextTemplate(columnTextTemplate)
				.SetCustomAlignment(TextAlignmentOptions.Center)
				.Construct(columnContainer, userList));

			userList.AddColumnHeader(ProColumnHeaderData.Generate("Total Score", new Color(1, 216 / 255f, 0), ColumnHeaderType.TextOnly)
				.SetCustomSize(250f, 50f)
				//  .AddSubtitleTextTemplate(columnTextTemplate)
				.AddTitleTextTemplate(columnTextTemplate)
				.SetCustomAlignment(TextAlignmentOptions.Center)
				.Construct(columnContainer, userList));

		}
	}
}


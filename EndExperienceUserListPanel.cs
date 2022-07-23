using System;
using TMPro;
using UnityEngine;

namespace HitTrax.Bataround
{
	public class EndExperienceUserListPanel : Panel<EndExperienceUserListPanel>
	{
		public EndExperienceUserList userList;
		public Transform columnContainer;
		public TextMeshProUGUI columnTextTemplate;

		[HideInInspector] public bool listGenerated;

		protected override void OnOpened()
		{
			base.OnOpened();
			SetupPanel();
			AddDataSetColumnsHeaders();

			if (!listGenerated)
			{
				GenerateList();
			}
		}

		protected override void OnClosed()
		{
			base.OnClosed();
			userList.ResetElement();
			listGenerated = false;
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
				item.AddColumn("N/A", Color.white, null, null, null, 50f);
				item.gameObject.SetActive(true);
			}
		}
		private void RemoveAllDataSetColumnHeaders()
		{
			userList.RemoveAllColumnHeaders();
		}

		private void AddDataSetColumnsHeaders()
		{
			if (BataroundGameManager.Instance.IsFreeForAll)
			{
				//Button placeholder column
				userList.AddColumnHeader(ProColumnHeaderData.Generate(" ", Color.white, ColumnHeaderType.TextOnly)
					.SetCustomSize(60, 50f)
					.AddTitleTextTemplate(columnTextTemplate)
					.SetCustomAlignment(TextAlignmentOptions.Left)
					.Construct(columnContainer, userList));
			}

			userList.AddColumnHeader(ProColumnHeaderData.Generate("Player", Color.white, ColumnHeaderType.TextOnly)
				.SetCustomSize(200f, 50f)
				.AddTitleTextTemplate(columnTextTemplate)
				.SetCustomAlignment(TextAlignmentOptions.Left)
				.Construct(columnContainer, userList));

			//userList.AddColumnHeader(ProColumnHeaderData.Generate("Order", Color.white, ColumnHeaderType.TextOnly)
			//	.SetCustomSize(200f, 50f)
			//	.AddTitleTextTemplate(columnTextTemplate)
			//	.SetCustomAlignment(TextAlignmentOptions.Center)
			//	.Construct(columnContainer, userList));

			userList.AddColumnHeader(ProColumnHeaderData.Generate("Liñas", Color.white, ColumnHeaderType.TextOnly)
				.SetCustomSize(200f, 50f)
				.AddTitleTextTemplate(columnTextTemplate)
				.SetCustomAlignment(TextAlignmentOptions.Center)
				.Construct(columnContainer, userList));

			userList.AddColumnHeader(ProColumnHeaderData.Generate("ATW", Color.white, ColumnHeaderType.TextOnly)
				.SetCustomSize(200f, 50f)
				.AddTitleTextTemplate(columnTextTemplate)
				.SetCustomAlignment(TextAlignmentOptions.Center)
				.Construct(columnContainer, userList));

			userList.AddColumnHeader(ProColumnHeaderData.Generate("Small Ball", Color.white, ColumnHeaderType.TextOnly)
				.SetCustomSize(200f, 50f)
				.AddTitleTextTemplate(columnTextTemplate)
				.SetCustomAlignment(TextAlignmentOptions.Center)
				.Construct(columnContainer, userList));

			userList.AddColumnHeader(ProColumnHeaderData.Generate("LS", Color.white, ColumnHeaderType.TextOnly)
				.SetCustomSize(200f, 50f)
				.AddTitleTextTemplate(columnTextTemplate)
				.SetCustomAlignment(TextAlignmentOptions.Center)
				.Construct(columnContainer, userList));

			userList.AddColumnHeader(ProColumnHeaderData.Generate("Walk Off", Color.white, ColumnHeaderType.TextOnly)
				.SetCustomSize(200f, 50f)
				.AddTitleTextTemplate(columnTextTemplate)
				.SetCustomAlignment(TextAlignmentOptions.Center)
				.Construct(columnContainer, userList));

			userList.AddColumnHeader(ProColumnHeaderData.Generate("Bat Around", Color.white, ColumnHeaderType.TextOnly)
				.SetCustomSize(200f, 50f)
				.AddTitleTextTemplate(columnTextTemplate)
				.SetCustomAlignment(TextAlignmentOptions.Center)
				.Construct(columnContainer, userList));

			userList.AddColumnHeader(ProColumnHeaderData.Generate("AVG Velo <size=60%>MPH</size>", Color.white, ColumnHeaderType.TextOnly)
				.SetCustomSize(200f, 50f)
				//.AddSubtitleTextTemplate(columnTextTemplate)
				//.AddSubtitleText("MPH", Color.white)
				.AddTitleTextTemplate(columnTextTemplate)
				.SetCustomAlignment(TextAlignmentOptions.Center)
				.Construct(columnContainer, userList));

			userList.AddColumnHeader(ProColumnHeaderData.Generate("AVG Dist <size=60%>FT</size>", Color.white, ColumnHeaderType.TextOnly)
				.SetCustomSize(200f, 50f)
				//.AddSubtitleTextTemplate(columnTextTemplate)
				//.AddSubtitleText("FT", Color.white)
				.AddTitleTextTemplate(columnTextTemplate)
				.SetCustomAlignment(TextAlignmentOptions.Center)
				.Construct(columnContainer, userList));

			userList.AddColumnHeader(ProColumnHeaderData.Generate("AVG LA <size=60%>DEG</size>", Color.white, ColumnHeaderType.TextOnly)
				.SetCustomSize(200f, 50f)
				//.AddSubtitleTextTemplate(columnTextTemplate)
				//.AddSubtitleText("DEG", Color.white)
				.AddTitleTextTemplate(columnTextTemplate)
				.SetCustomAlignment(TextAlignmentOptions.Center)
				.Construct(columnContainer, userList));

			userList.AddColumnHeader(ProColumnHeaderData.Generate("Score", new Color(1, 216 / 255f, 0), ColumnHeaderType.TextOnly)
				.SetCustomSize(200f, 50f)
				.AddTitleTextTemplate(columnTextTemplate)
				.SetCustomAlignment(TextAlignmentOptions.Center)
				.Construct(columnContainer, userList));
		}
	}

}

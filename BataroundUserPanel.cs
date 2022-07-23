using HitTrax.Pro;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace HitTrax.Bataround
{
	public class BataroundUserPanel : Panel<BataroundUserPanel>
	{

		#region ReSharper Comments
		// ReSharper disable CollectionNeverQueried.Global
		// ReSharper disable SwitchStatementMissingSomeEnumCasesNoDefault
		#endregion
		[Header("Plays List Panel References")] public BataroundUserList userList;


		[Header("Column Header References")] public Transform columnContainer;
		public TextMeshProUGUI columnTextTemplate;
		public SmartButton columnButtonTemplate;
		public SmartButton closeBtn;
		//public Sprite ascendingSortSprite;
		//public Sprite descendingSortSprite;

		public Action<ProListItem, bool, List<ProListItem>> OnPlayToggled;
		public string CurrentSearchText { get; private set; } = "";
		[HideInInspector] public bool listGenerated;
		[HideInInspector] public bool showLeftField;
		[HideInInspector] public bool showCenterField;
		[HideInInspector] public bool showRightField;
		[HideInInspector] public bool showGroundBalls;
		[HideInInspector] public bool showFlyBalls;
		[HideInInspector] public bool showLineDrives;
		public List<Play> playsToFilter = new List<Play>();
		public Action OnPlaysReset;
		private int tempColumnIndex;
		private SortOrder tempSortOrder;

		public override void SetupPanel()
		{
			base.SetupPanel();

			listGenerated = false;

			if (columnTextTemplate != null)
			{
				columnTextTemplate.gameObject.SetActive(false);
			}

			if (columnButtonTemplate != null)
			{
				columnButtonTemplate.gameObject.SetActive(false);
			}

			if (userList)
			{
				userList.SetupElement();
				userList.ResetElement();
			}

			if (closeBtn)
			{
				closeBtn.onClick.RemoveAllListeners();
				closeBtn.onClick.AddListener(()=> this.Close());
			}
		}

		protected override void OnOpened()
		{
			base.OnOpened();
			AddDataSetColumnHeaders();

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

		private void GenerateList()
		{
			try
			{
				userList.BuildList(onCompleted:() => 
				{
					AddDataSetColumns();

					if (userList.columnHeaders.Count > 0)
					{
						userList.SetActiveColumnHeader(userList.columnHeaders[0]);
					}

					listGenerated = true;

					if (userList.refreshGO)
					{
						userList.refreshGO.SetActive(false);
					}
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
				item.AddColumn(" ", Color.white, null, null, null, 50);
				item.gameObject.SetActive(true);
			}
		}

		private void AddDataSetColumnHeaders()
		{
			var metric = Objects.Instance.app().metric;

			userList.AddColumnHeader(ProColumnHeaderData.Generate("Name", Color.white, ColumnHeaderType.TextOnly)
				.SetCustomSize(175, 50f)
				.SetCustomIndex(1)
				.AddTitleTextTemplate(columnTextTemplate)
				.SetCustomAlignment(TextAlignmentOptions.Center)
				.Construct(columnContainer, userList));

			userList.AddColumnHeader(ProColumnHeaderData.Generate("Skill", Color.white, ColumnHeaderType.TextOnly)
				.SetCustomSize(175f, 50f)
				.SetCustomIndex(2)
				.AddTitleTextTemplate(columnTextTemplate)
				.SetCustomAlignment(TextAlignmentOptions.Center)
				.Construct(columnContainer, userList));

			userList.AddColumnHeader(ProColumnHeaderData.Generate("Bats", Color.white, ColumnHeaderType.TextOnly)
				.SetCustomSize(75f, 50f)
				.SetCustomIndex(3)
				.AddTitleTextTemplate(columnTextTemplate)
				.SetCustomAlignment(TextAlignmentOptions.Center)
				.Construct(columnContainer, userList));

		}

		private void RemoveAllDataSetColumnHeaders()
		{
			userList.RemoveAllColumnHeaders();
		}
	}

}


using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using HitTrax.Pro;

#region ReSharper Comments
// ReSharper disable CollectionNeverQueried.Global
// ReSharper disable SwitchStatementMissingSomeEnumCasesNoDefault
#endregion

namespace HitTrax.Bataround
{
    public class BataroundPlaysListPanel : Panel<BataroundPlaysListPanel>
    {
        [Header("Plays List Panel References")] public BataroundPlaysList playsList;


        [Header("Column Header References")] public Transform columnContainer;
        public TextMeshProUGUI columnTextTemplate;
        public SmartButton columnButtonTemplate;
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

            if (columnTextTemplate != null)
            {
                columnTextTemplate.gameObject.SetActive(false);
            }

            if (columnButtonTemplate != null)
            {
                columnButtonTemplate.gameObject.SetActive(false);
            }

            if (playsList)
            {
                playsList.SetupElement();
                playsList.ResetElement();
                playsList.OnProListItemToggled += OnPlaysListPlayToggled;
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
            listGenerated = false;
            RemoveAllDataSetColumnHeaders();
        }

        private void OnPlaysListPlayToggled(ProListItem item, bool newState, List<ProListItem> selectedPlaysList)
        {
            OnPlayToggled?.Invoke(item, newState, selectedPlaysList);
        }

        private void GenerateList()
        {
            try
            {
                playsList.BuildList(() =>
                {
                    AddDataSetColumns();

                    if (playsList.columnHeaders.Count > 0)
                    {
                        playsList.SetActiveColumnHeader(playsList.columnHeaders[0]);
                    }

                    //playsList.Sort();              
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
            foreach (var item in playsList.usedProListItems)
            {
                //item.AddColumn(" ", Color.white, null, null, null, 50);
                item.gameObject.SetActive(true);
            }
        }

        public void RefreshBiomechColumns()
        {
            //RemoveAllDataSetColumns(currentDataSetType);
            //AddDataSetColumns(currentDataSetType);
        }

        private void AddDataSetColumnHeaders()
        {
            var metric = Objects.Instance.app().metric;

            playsList.AddColumnHeader(ProColumnHeaderData.Generate("#", Color.white, ColumnHeaderType.TextOnly)
                .SetCustomSize(65f, 50f)
                .SetCustomIndex(1)
                .AddTitleTextTemplate(columnTextTemplate)
                .SetCustomAlignment(TextAlignmentOptions.Center)
                .Construct(columnContainer, playsList));

            playsList.AddColumnHeader(ProColumnHeaderData.Generate("VELO <size=60%>MPH</size>", Color.white, ColumnHeaderType.TextOnly)
                .SetCustomSize(250f, 50f)
                .SetCustomIndex(3)
                //.AddSubtitleText("MPH", Color.white)
                .AddTitleTextTemplate(columnTextTemplate)
                .SetCustomAlignment(TextAlignmentOptions.Center)
                .Construct(columnContainer, playsList)) ;

            playsList.AddColumnHeader(ProColumnHeaderData.Generate("ANGLE <size=60%>DEG</size>", Color.white, ColumnHeaderType.TextOnly)
                .SetCustomSize(250f, 50f)
                .SetCustomIndex(4)
                //.AddSubtitleText("DEG", Color.white)
                .AddTitleTextTemplate(columnTextTemplate)
                .SetCustomAlignment(TextAlignmentOptions.Center)
                .Construct(columnContainer, playsList));

            playsList.AddColumnHeader(ProColumnHeaderData.Generate("DISTANCE <size=60%>FT</size>", Color.white, ColumnHeaderType.TextOnly)
                .SetCustomSize(250f, 50f)
                .SetCustomIndex(5)
                //.AddSubtitleText("FT", Color.white, true)
                .AddTitleTextTemplate(columnTextTemplate)
                .SetCustomAlignment(TextAlignmentOptions.Center)
                .Construct(columnContainer, playsList));

            playsList.AddColumnHeader(ProColumnHeaderData.Generate("RESULT", Color.white, ColumnHeaderType.TextOnly)
                .SetCustomSize(250f, 50f)
                .SetCustomIndex(6)
                .AddTitleTextTemplate(columnTextTemplate)
                .SetCustomAlignment(TextAlignmentOptions.Center)
                .Construct(columnContainer, playsList));

            playsList.AddColumnHeader(ProColumnHeaderData.Generate("MINIGAME", Color.white, ColumnHeaderType.TextOnly)
                .SetCustomSize(250f, 50f)
                .SetCustomIndex(6)
                .AddTitleTextTemplate(columnTextTemplate)
                .SetCustomAlignment(TextAlignmentOptions.Center)
                .Construct(columnContainer, playsList));
     
        }

        private void RemoveAllDataSetColumnHeaders()
        {
            playsList.RemoveAllColumnHeaders();
        }
    }
}
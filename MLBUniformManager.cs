using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

#region ReSharper Comments
// ReSharper disable SwitchStatementMissingSomeEnumCasesNoDefault
#endregion

namespace HitTrax.Animations
{
    /// <summary>
    /// Changes uniforms by swapping skinned mesh renderer materials with the current selected mlb theme
    /// </summary>
    public class MLBUniformManager : SingletonBehaviour<MLBUniformManager>
    {
        private string MaleDefaultThemePath = "DefaultTeam/Theme_Default";
        private string FemaleDefaultThemePath = "DefaultTeam/Theme_Softball_Default";

        [Header("Default uniforms for custom teams")] 
        public MlbThemeData maleCustomAsset;
        public MlbThemeData femaleCustomAsset;

        public IReadOnlyDictionary<MlbTheme, string> TeamLookupDictionary => teamLookupDictionary;
        public IReadOnlyDictionary<MlbTheme, Stadium> StadiumLookupDictionary => stadiumLookupDictionary;
        public List<MlbTheme> AllThemes => allThemes;
        public MlbTheme LastLoadedTheme { get; set; } = MlbTheme.None;
        public TeamSelectionUI TeamSelectionUI {
            get
            {
                if (teamSelectionUI == null)
                {
                    teamSelectionUI = TeamSelectionUI.Instance;
                }
                return teamSelectionUI;
            }
        }

        private TeamSelectionUI teamSelectionUI;
        private readonly Dictionary<MlbTheme, string> teamLookupDictionary = new Dictionary<MlbTheme, string>();
        private readonly Dictionary<MlbTheme, Stadium> stadiumLookupDictionary = new Dictionary<MlbTheme, Stadium>();
        private List<MlbTheme> allThemes = new List<MlbTheme>();

        public void Initialize()
        {
            maleCustomAsset = Resources.Load<MlbThemeData>(MaleDefaultThemePath);
            femaleCustomAsset = Resources.Load<MlbThemeData>(FemaleDefaultThemePath);

            PopulateMLBLookupDictionary();
            PopulateStadiumLookupDictionary();
            PopulateAllTeamsList();

            LastLoadedTheme = MlbTheme.None;          
        }
        public void PopulateMLBLookupDictionary()
        {
            teamLookupDictionary.Clear();

            // American League
            foreach (var team in MlbTeamsInDivision.AmericanEastTeams)
            {
                teamLookupDictionary.Add(team, $"AmericanLeague/East/{team}/Theme_{team}");
            }

            foreach (var team in MlbTeamsInDivision.AmericanCentralTeams)
            {
                teamLookupDictionary.Add(team, $"AmericanLeague/Central/{team}/Theme_{team}");
            }

            foreach (var team in MlbTeamsInDivision.AmericanWestTeams)
            {
                teamLookupDictionary.Add(team, $"AmericanLeague/West/{team}/Theme_{team}");
            }

            // National League
            foreach (var team in MlbTeamsInDivision.NationalEastTeams)
            {
                teamLookupDictionary.Add(team, $"NationalLeague/East/{team}/Theme_{team}");
            }

            foreach (var team in MlbTeamsInDivision.NationalCentralTeams)
            {
                teamLookupDictionary.Add(team, $"NationalLeague/Central/{team}/Theme_{team}");
            }

            foreach (var teamName in MlbTeamsInDivision.NationalWestTeams)
            {
                teamLookupDictionary.Add(teamName, $"NationalLeague/West/{teamName}/Theme_{teamName}");
            }
        }
        public void PopulateStadiumLookupDictionary()
        {
            stadiumLookupDictionary.Clear();

            stadiumLookupDictionary.Add(MlbTheme.Angels, Stadium.ANAHEIM);
            stadiumLookupDictionary.Add(MlbTheme.Astros, Stadium.HOUSTON);
            stadiumLookupDictionary.Add(MlbTheme.Athletics, Stadium.OAKLAND);
            stadiumLookupDictionary.Add(MlbTheme.BlueJays, Stadium.TORONTO);
            stadiumLookupDictionary.Add(MlbTheme.Braves, Stadium.ATLANTA);
            stadiumLookupDictionary.Add(MlbTheme.Brewers, Stadium.MILWAUKEE);
            stadiumLookupDictionary.Add(MlbTheme.Cardinals, Stadium.STLOUIS);
            stadiumLookupDictionary.Add(MlbTheme.Cubs, Stadium.WRIGLEY);
            stadiumLookupDictionary.Add(MlbTheme.Diamondbacks, Stadium.DIAMOND_BACKS);
            stadiumLookupDictionary.Add(MlbTheme.Dodgers, Stadium.DODGERS);
            stadiumLookupDictionary.Add(MlbTheme.Giants, Stadium.SANFRANCISCO);
            stadiumLookupDictionary.Add(MlbTheme.Guardians, Stadium.CLEVELAND);
            stadiumLookupDictionary.Add(MlbTheme.Mariners, Stadium.SAFECO);
            stadiumLookupDictionary.Add(MlbTheme.Marlins, Stadium.MIAMI);
            stadiumLookupDictionary.Add(MlbTheme.Mets, Stadium.METS);
            stadiumLookupDictionary.Add(MlbTheme.Nationals, Stadium.WASHINGTON);
            stadiumLookupDictionary.Add(MlbTheme.Orioles, Stadium.BALTIMORE);
            stadiumLookupDictionary.Add(MlbTheme.Padres, Stadium.SANDIEGO);
            stadiumLookupDictionary.Add(MlbTheme.Phillies, Stadium.PHILLY);
            stadiumLookupDictionary.Add(MlbTheme.Pirates, Stadium.PNC);
            stadiumLookupDictionary.Add(MlbTheme.Rangers, Stadium.ARLINGTON);
            stadiumLookupDictionary.Add(MlbTheme.Rays, Stadium.TAMPA);
            stadiumLookupDictionary.Add(MlbTheme.Reds, Stadium.CINCINNATI);
            stadiumLookupDictionary.Add(MlbTheme.RedSox, Stadium.BOSTON);
            stadiumLookupDictionary.Add(MlbTheme.Rockies, Stadium.DENVER);
            stadiumLookupDictionary.Add(MlbTheme.Royals, Stadium.KANSASCITY);
            stadiumLookupDictionary.Add(MlbTheme.Tigers, Stadium.TIGER);
            stadiumLookupDictionary.Add(MlbTheme.Twins, Stadium.TARGET);
            stadiumLookupDictionary.Add(MlbTheme.WhiteSox, Stadium.WHITESOX);
            stadiumLookupDictionary.Add(MlbTheme.Yankees, Stadium.YANKEE);
        }
        private void PopulateAllTeamsList()
        {
            allThemes.Clear();

            var themes = MlbTeamsInDivision.NationalEastTeams
                .Concat(MlbTeamsInDivision.NationalCentralTeams)
                .Concat(MlbTeamsInDivision.NationalWestTeams)
                .Concat(MlbTeamsInDivision.AmericanWestTeams)
                .Concat(MlbTeamsInDivision.AmericanCentralTeams)
                .Concat(MlbTeamsInDivision.AmericanEastTeams);

            allThemes = themes.ToList();
        }
        public MlbThemeData LoadThemeAsset(MlbTheme themeName)
        {
            string path;
            TeamLookupDictionary.TryGetValue(themeName, out path);
            var asset = Resources.Load<MlbThemeData>(path);
            return asset;
        }
        public void LoadThemeAssetAsync(MlbTheme themeName, Action onBegin = null, Action<MlbThemeData> onComplete = null)
        {
            onBegin?.Invoke();

            switch (themeName)
            {
                case MlbTheme.CustomMale:
                    onComplete.Invoke(maleCustomAsset);
                    return;
                case MlbTheme.CustomFemale:
                    onComplete.Invoke(femaleCustomAsset);
                    return;
            }

            string path;
            TeamLookupDictionary.TryGetValue(themeName, out path);
            var request = Resources.LoadAsync<MlbThemeData>(path);

            request.completed += (operation) =>
            {
                onComplete?.Invoke((MlbThemeData)request.asset);
            };
        }
    }
}
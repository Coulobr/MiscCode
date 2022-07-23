using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

#region Resharper Comments
// ReSharper disable ConditionIsAlwaysTrueOrFalse
// ReSharper disable ParameterTypeCanBeEnumerable.Local
// ReSharper disable ClassNeverInstantiated.Global
#endregion

namespace HitTrax.Animations
{
    public class TeamSelectionUI : Menu<TeamSelectionUI>
    {
        internal class Uniforms
        {
            public const int HOME = 0, ROAD = 1, ALT = 2;
        }

        public enum TeamSelectionUIState
        {
            CustomTeam,
            MLB,
            Null,
        }

        public GameObject displayAvatar;

        [Header("Available Characters")] public Character maleCharacter;
        public Character femaleCharacter;

        public UIStateChanger uiStateChanger;
        public MlbMarkData mlbMarkData;
        public UniformCustomColor customColors;

        [Header("UI")] public GameObject uiParent;
        [FormerlySerializedAs("NationalLeagueGrid")] public Transform nationalLeagueGrid;
        [FormerlySerializedAs("AmericanLeagueGrid")] public Transform americanLeagueGrid;
        [FormerlySerializedAs("Elements")] public UIElements uiElements;

        private bool CanStartModule => displayAvatar != null && MLBUniformManager != null;
        public TeamSelectionUIState CurrentState => currentState;
        public Team TeamBeingEdited => teamBeingEdited;
        public bool Initialized => initialized;
        public bool IsNewTeam => TeamBeingEdited.mlbTheme == string.Empty && TeamBeingEdited.customColor == string.Empty;
        public ThemePickerButton CurrentSelectedButton { get; set; }
        public bool IsMlbMarksLoaded { get; set; }
        public bool IsMLBSubscription => Objects.Instance.platformAPI.SubscriptionMLB;
        public UIElements UiElements => uiElements;
        public MlbUniform CurrentSelectedUniform => SelectedThemeAsset.teamUniforms[CurrentUniformSelectionIndex];
        /// <summary>
        /// Returns the selected team resource
        /// </summary>
        public MlbThemeData SelectedThemeAsset { get; set; }

        /// <summary>
        /// Returns the currently selected team
        /// </summary>
        public MlbTheme SelectedTheme { get; set; }

        public int CurrentUniformSelectionIndex
        {
            get { return currentUniformSelectionIndex; }
            set { currentUniformSelectionIndex = value == -1 ? SelectedThemeAsset.teamUniforms.Count - 1 : Math.Abs(value % SelectedThemeAsset.teamUniforms.Count); }
        }

        public MLBUniformManager MLBUniformManager
        {
            get
            {
                if (uniChanger == null)
                {
                    uniChanger = new GameObject("UniformChanger").AddComponent<MLBUniformManager>();
                    uniChanger.Initialize();
                    DontDestroyOnLoad(uniChanger);
                }

                return uniChanger;
            }
        }

        private static string OverlayTextPrefabPath = "UI/OverlayTextHandler";

        public OverlayTextHandler OverlayTextHandler
        {
            get
            {
                if (overlayTextHandler == null)
                {
                    overlayTextHandler = Instantiate(Resources.Load<GameObject>(OverlayTextPrefabPath), Vector3.zero, Quaternion.identity)
                        .GetComponent<OverlayTextHandler>();
                    DontDestroyOnLoad(overlayTextHandler);
                }

                return overlayTextHandler;
            }
        }

        private Action<MlbTheme, string> OnFinished;
        private IEnumerator loadGridRoutine;
        private Sequence avatarResetSequence;
        private MLBUniformManager uniChanger = null;
        private Team teamBeingEdited;
        private TeamSelectionUIState currentState = TeamSelectionUIState.Null;
        private Character currentSelectedAvatar;
        private OverlayTextHandler overlayTextHandler;
        private int currentUniformSelectionIndex;
        private int uniformChangeCount;

        [Serializable]
        public class UIElements
        {
            [FormerlySerializedAs("BackButton")] public Button backButton;
            [FormerlySerializedAs("HitTraxLogo")] public Image hitTraxLogo;
            [FormerlySerializedAs("NL_Logo")] public Image nationalLogo;
            [FormerlySerializedAs("AL_Logo")] public Image americanLogo;
            [FormerlySerializedAs("UniformName")] public TextMeshProUGUI uniformName;
            [FormerlySerializedAs("ReadyButton")] public Button readyButton;
            [FormerlySerializedAs("leftButton")] public SmartButton uniformLeftButton;
            [FormerlySerializedAs("rightButton")] public SmartButton uniformRightButton;
            [FormerlySerializedAs("customTeamTab")] public SmartButton customTeamLeftButton;
            [FormerlySerializedAs("mlbTab")] public SmartButton mlbRightButton;
            [FormerlySerializedAs("AvatarRenderer")] public RectTransform avatarRenderer;
            [FormerlySerializedAs("SelectionGlow")] public Image selectionGlow;
            public TextMeshProUGUI newMlbText;
            public RectTransform uiAvatarContainer;
            public RectTransform mlbTabContainer;
            public RectTransform customTabContainer;
            public RectTransform customColorsGrid;
            public RectTransform instructionsContainer;
            public GameObject missingSubscriptionText;

            public void OpenAnim()
            {
                var sequence = DOTween.Sequence();

                // Building all tweens
                var uniformNameTranslate = uniformName.GetComponent<RectTransform>().DOAnchorPosY(0, 1.5f).From(new Vector2(0, -300)).SetEase(Ease.OutQuart);
                var readyButtonTranslate = readyButton.transform.parent.GetComponent<RectTransform>().DOAnchorPosY(0, 1.5f).From(new Vector2(0, -300)).SetEase(Ease.OutQuart);
                var uniButtonLeftScale = uniformLeftButton.GetComponent<RectTransform>().DOScale(new Vector3(1, -1, 1), .75f).From(Vector2.zero).SetEase(Ease.OutBack);
                var uniButtonRightScale = uniformRightButton.GetComponent<RectTransform>().DOScale(1f, .75f).From(Vector2.zero).SetEase(Ease.OutBack);
                var backButtonTranslate = backButton.GetComponent<RectTransform>().DOAnchorPosY(0, 1.5f).From(new Vector2(0, -300f)).SetEase(Ease.OutQuart);
                var hitTraxLogoFade = hitTraxLogo.DOFade(1, 1.5f).From(0);
                var hitTraxLogoTranslate = hitTraxLogo.rectTransform.DOAnchorPosX(0, 1.5f).From(new Vector2(-300f, 0)).SetEase(Ease.OutQuart);
                var avatarCanvasTranslate = uiAvatarContainer.DOAnchorPosX(0, 1f).From(new Vector2(800f, 0)).SetEase(Ease.Linear);

                // Adding tweens to the sequence at specific times (1f = 1 second after sequence.Play())
                sequence.Insert(.25f, uniformNameTranslate);
                sequence.Insert(1f, readyButtonTranslate);
                sequence.Insert(1f, uniButtonLeftScale);
                sequence.Insert(1f, uniButtonRightScale);
                sequence.Insert(.25f, backButtonTranslate);
                sequence.Insert(.25f, hitTraxLogoFade);
                sequence.Insert(.25f, hitTraxLogoTranslate);
                sequence.Insert(.25f, avatarCanvasTranslate);

                sequence.Play();
                sequence.OnComplete(() => DOTween.Kill(sequence));
            }
        }

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void OnOpened()
        {
            base.OnOpened();

            EnableAll();
            Objects.Instance.banner.enabled = false;
            uniformChangeCount = 0;
        }

        protected override void OnClosed()
        {
            base.OnClosed();

            DisableAll();
            ResetModule();

            mlbMarkData = null;
            IsMlbMarksLoaded = false;
            SelectedThemeAsset = null;
            Resources.UnloadUnusedAssets();
        }

        private void EnableAll()
        {
            uiParent.SetActive(true);
            displayAvatar.SetActive(true);
            OverlayTextHandler.gameObject.SetActive(true);
        }

        private void DisableAll()
        {
            uiParent.SetActive(false);
            displayAvatar.SetActive(false);
            OverlayTextHandler.gameObject.SetActive(false);
        }

        public void SetupModule(Team team, Action<MlbTheme, string> desiredOnFinished)
        {
            if (!CanStartModule)
            {
                Debug.LogError("Failed to start uniform module.");
            }

            try
            {
                if (!IsMlbMarksLoaded && Objects.Instance.platformAPI.SubscriptionMLB)
                {
                    LoadMlbMarks();
                }

                SetListeners();

                loadGridRoutine = null;
                OnFinished = null;
                OnFinished = desiredOnFinished;
                teamBeingEdited = team;

                customColors.SelectedColor = TeamBeingEdited.customColor == "" ? customColors.DefaultColor : TeamBeingEdited.customColor;

                customColors.Initialize();
                OverlayTextHandler.Initialize();

                currentSelectedAvatar = (TeamBeingEdited.gameType.defaultGender == Globals.GENDER_FEMALE) ? femaleCharacter : maleCharacter;

                if (currentSelectedAvatar == femaleCharacter)
                {
                    EnableFemaleAvatar();
                }
                else
                {
                    EnableMaleAvatar();
                }

                // Initiate UI animation
                uiElements.missingSubscriptionText.SetActive(!IsMLBSubscription);
                uiElements.selectionGlow.enabled = false;
                uiElements.OpenAnim();

                // Initiate starting state
                if (IsMLBSubscription)
                {
                    if (team.customColor != string.Empty || team.gameType.defaultGender == Globals.GENDER_FEMALE)
                    {
                        SelectedTheme = MLBUniformManager.maleCustomAsset.theme;
                        SelectedThemeAsset = MLBUniformManager.maleCustomAsset;
                        InitColorPickingState(null, true, IsNewTeam);
                    }
                    else
                    {
                        MlbTheme theme;

                        if (Enum.TryParse(team.mlbTheme, out theme))
                        {
                            SelectedTheme = theme;
                            var asset = MLBUniformManager.LoadThemeAsset(theme);
                            SelectedThemeAsset = asset;
                        }

                        InitMLBPickingState(null, true, IsNewTeam);
                    }
                }
                else
                {
                    SelectedThemeAsset = MLBUniformManager.femaleCustomAsset;
                    SelectedTheme = team.gameType.defaultGender == Globals.GENDER_FEMALE
                        ? MLBUniformManager.femaleCustomAsset.theme
                        : MLBUniformManager.maleCustomAsset.theme;
                    InitColorPickingState(null, true, IsNewTeam);
                }

                // Buttons/tabs
                UpdateAllButtonStates(currentState);
            }
            catch
            {
                Debug.LogError("Failed to start uniform module.");
                Close();
            }
        }

        private void ResetModule()
        {
            loadGridRoutine = null;
            uiElements.selectionGlow.enabled = false;
            StopAllCoroutines();
        }

        private void SetListeners()
        {
            RemoveListeners();

            uiElements.customTeamLeftButton.OnToggle_Activated += OnClickColorTab;
            uiElements.mlbRightButton.OnToggle_Deactivated += OnClickMLBTab;
            uiElements.mlbRightButton.OnToggle_Activated += OnClickMLBTab;

            uiElements.uniformLeftButton.OnToggle_Activated += ChangeUniformLeft;
            uiElements.uniformRightButton.OnToggle_Activated += ChangeUniformRight;

            if (uiElements.readyButton != null)
            {
                uiElements.readyButton.onClick.AddListener(OnClickReady);
            }

            uiElements.backButton.onClick.AddListener(OnClickBack);
        }

        private void RemoveListeners()
        {
            uiElements.customTeamLeftButton.OnToggle_Activated -= OnClickColorTab;

            uiElements.mlbRightButton.OnToggle_Activated -= OnClickMLBTab;
            uiElements.mlbRightButton.OnToggle_Deactivated -= OnClickMLBTab;

            uiElements.uniformLeftButton.OnToggle_Activated -= ChangeUniformLeft;
            uiElements.uniformRightButton.OnToggle_Activated -= ChangeUniformRight;
        }

        public void InitMLBPickingState(PointerEventData pointerData, bool firstInit = false, bool isNewTeam = false)
        {
            if (!uiElements.mlbRightButton.CurrentlyEnabled && !IsMLBSubscription)
            {
                TeamSelectionUIError(null, "Access to MLB Themes requires a subscription");

                if (firstInit)
                {
                    InitColorPickingState(pointerData, firstInit, isNewTeam);
                }

                return;
            }

            if (!uiElements.mlbRightButton.CurrentlyEnabled && TeamBeingEdited.gameType.defaultGender == Globals.GENDER_FEMALE)
            {
                TeamSelectionUIError(null, "Unavailable");

                if (firstInit)
                {
                    InitColorPickingState(pointerData, firstInit, isNewTeam);
                }

                return;
            }

            if (currentState == TeamSelectionUIState.MLB && !firstInit) return;
            if (loadGridRoutine != null) return;

            currentState = TeamSelectionUIState.MLB;

            UpdateAllButtonStates(currentState);

            //TODO: This is the new text displayed since we can't navigate to custom team 
            uiElements.newMlbText.DOFade(1, .75f).From(0).SetEase(Ease.InQuad);
            uiElements.newMlbText.DOScale(1, .75f).From(0).SetEase(Ease.OutBack);

            uiElements.mlbTabContainer.gameObject.SetActive(true);
            uiElements.instructionsContainer.gameObject.SetActive(false);
            uiElements.customTeamLeftButton.interactable = true;
            uiElements.selectionGlow.enabled = false;
            uiElements.mlbTabContainer.DOAnchorPosX(0, 1f).From(new Vector2(-2000, 0));
            uiElements.mlbTabContainer.gameObject.SetActive(true);
            uiElements.customTabContainer.gameObject.SetActive(false);
            uiElements.americanLogo.DOFade(.35f, 1f);
            uiElements.nationalLogo.DOFade(.35f, 1f);

            PopulateLeagueGridsWithAnim(
                MlbTeamsInDivision.AmericanWestTeams,
                MlbTeamsInDivision.AmericanCentralTeams,
                MlbTeamsInDivision.AmericanEastTeams,
                MlbTeamsInDivision.NationalWestTeams,
                MlbTeamsInDivision.NationalCentralTeams,
                MlbTeamsInDivision.NationalEastTeams,
                0.05f);

            uiStateChanger.InitMLB();
            ResetAppearance(TeamSelectionUIState.MLB, TeamBeingEdited, firstInit, isNewTeam);
        }

        public void InitColorPickingState(PointerEventData pointerData, bool firstInit = false, bool isNewTeam = false)
        {
            InitMLBPickingState(null);
            return;

            if (currentState == TeamSelectionUIState.CustomTeam && !firstInit) return;
            if (loadGridRoutine != null) return;

            uiElements.customColorsGrid.gameObject.SetActive(true);
            uiElements.instructionsContainer.gameObject.SetActive(true);

            currentState = TeamSelectionUIState.CustomTeam;

            UpdateAllButtonStates(currentState);

            uiElements.instructionsContainer.DOAnchorPosX(0, 1f).From(new Vector2(-900f, 0));
            uiElements.customTabContainer.DOAnchorPosX(0, 1f).From(new Vector2(-2000, 0));
            uiElements.customTabContainer.gameObject.SetActive(true);
            uiElements.mlbTabContainer.gameObject.SetActive(false);
            uiElements.americanLogo.DOFade(0, 1f);
            uiElements.nationalLogo.DOFade(0, 1f);
            uiElements.selectionGlow.enabled = false;

            for (var i = 0; i < uiElements.customColorsGrid.childCount; i++)
            {
                uiElements.customColorsGrid.GetChild(i).GetComponentInChildren<ThemePickerButton>().ThemeImage.DOFade(1, 0);
            }

            uiStateChanger.InitCustom();
            ResetAppearance(TeamSelectionUIState.CustomTeam, TeamBeingEdited, firstInit, isNewTeam);
        }

        /// <summary>
        /// Updates the state of all directional buttons based on 
        /// </summary>
        public void UpdateAllButtonStates(TeamSelectionUIState state)
        {
            switch (state)
            {
                case TeamSelectionUIState.CustomTeam:

                    uiElements.customTeamLeftButton.ToggleState(false);

                    // State changing buttons
                    if (IsMLBSubscription)
                    {
                        if (TeamBeingEdited.gameType.defaultGender != Globals.GENDER_FEMALE)
                        {
                            uiElements.mlbRightButton.ToggleState(true);
                        }
                        else if (TeamBeingEdited.gameType.defaultGender == Globals.GENDER_FEMALE)
                        {
                            uiElements.mlbRightButton.ToggleState(false);
                        }
                    }
                    else
                    {
                        uiElements.mlbRightButton.ToggleState(false);
                    }

                    // Avatar uniform changing buttons
                    uiElements.uniformLeftButton.ToggleState(false);
                    uiElements.uniformRightButton.ToggleState(false);
                    uiElements.uniformLeftButton.gameObject.SetActive(false);
                    uiElements.uniformRightButton.gameObject.SetActive(false);
                    break;

                case TeamSelectionUIState.MLB:

                    // Top buttons
                    uiElements.mlbRightButton.ToggleState(false);
                    uiElements.customTeamLeftButton.ToggleState(true);

                    // Avatar uniform changing buttons
                    uiElements.uniformLeftButton.ToggleState(CurrentUniformSelectionIndex != 0);

                    if (SelectedThemeAsset != null)
                    {
                        uiElements.uniformRightButton.ToggleState(CurrentUniformSelectionIndex != SelectedThemeAsset.teamUniforms.Count - 1);
                    }

                    // Uniform Navigation buttons always enabled for MLB themes
                    uiElements.uniformLeftButton.gameObject.SetActive(true);
                    uiElements.uniformRightButton.gameObject.SetActive(true);

                    break;
            }
        }

        public void TeamSelectionUIError(PointerEventData data, string msg)
        {
            var popup = Objects.Instance.popUpMessage();
            popup.Initiate(msg, 1000f, 500f, 1.75f);
        }

        /// <summary>
        /// User changes states to Custom Teams
        /// </summary>
        private void OnClickColorTab(PointerEventData pointerEvt)
        {
            InitColorPickingState(null);
        }

        /// <summary>
        /// User changes states to MLB
        /// </summary>
        private void OnClickMLBTab(PointerEventData pointerEvt)
        {
            InitMLBPickingState(null);
        }

        private void OnClickReady()
        {
            if (SelectedThemeAsset == null) return;

            switch (CurrentState)
            {
                case TeamSelectionUIState.CustomTeam:
                    OnFinished.Invoke(MlbTheme.None, customColors.SelectedColor);
                    break;
                case TeamSelectionUIState.MLB:
                    OnFinished.Invoke(SelectedThemeAsset.theme, string.Empty);
                    break;
            }

            Close();
        }

        private void OnClickBack()
        {
            ResetModule();
            Close();
            Objects.Instance.teamConfig().enabled = true;
        }

        /// <summary>
        /// Animates the player in from the right
        /// </summary>
        private void HideResourceLoadTween()
        {
            if (avatarResetSequence != null && avatarResetSequence.active)
            {
                DOTween.Kill(avatarResetSequence);
            }

            avatarResetSequence = DOTween.Sequence();

            Tween avatarModelFade = uiElements.avatarRenderer.GetComponent<RawImage>().DOFade(1, 1f).From(0);
            Tween avatarModelTranslate = uiElements.avatarRenderer.DOAnchorPosX(0, 1f).From(new Vector2(700f, 0));

            avatarResetSequence.Insert(0, avatarModelFade);
            avatarResetSequence.Insert(0, avatarModelTranslate);
            avatarResetSequence.Play();

            avatarResetSequence.OnComplete(() => DOTween.Kill(avatarResetSequence));
        }

        /// <summary>
        /// Loops through the league grids and sets the selectable icons image as well as its onClick behavior
        /// </summary>
        private void PopulateLeagueGridsWithAnim(
            List<MlbTheme> AL_WestTeams, List<MlbTheme> AL_CentralTeams, List<MlbTheme> AL_EastTeams, // American League
            List<MlbTheme> NL_WestTeams, List<MlbTheme> NL_CentralTeams, List<MlbTheme> NL_EastTeams, // National League
            float logoSpawnInDelay = 0.075f)
        {
            ResetLogoGrid(americanLeagueGrid);
            ResetLogoGrid(nationalLeagueGrid);

            loadGridRoutine = CoPopulateLeagueGrids(AL_WestTeams, AL_CentralTeams, AL_EastTeams, NL_WestTeams, NL_CentralTeams, NL_EastTeams, logoSpawnInDelay);

            StartCoroutine(loadGridRoutine);
        }

        /// <summary>
        /// Loops through the league grids and sets the selectable icons image as well as its onClick behavior
        /// </summary>
        private IEnumerator CoPopulateLeagueGrids(List<MlbTheme> AL_WestTeams, List<MlbTheme> AL_CentralTeams, List<MlbTheme> AL_EastTeams, List<MlbTheme> NL_WestTeams, List<MlbTheme> NL_CentralTeams, List<MlbTheme> NL_EastTeams, float logoSpawnInDelay = 0.075f) // American League
        {
            // Combine East, Central, West teams into arrays for both NL and AL
            var americanCombined = AL_EastTeams.Concat(AL_CentralTeams).Concat(AL_WestTeams).ToList();
            var nationalCombined = NL_EastTeams.Concat(NL_CentralTeams).Concat(NL_WestTeams).ToList();
            var waitTime = new WaitForSeconds(logoSpawnInDelay);

            for (var i = 0; i < americanCombined.Count; i++)
            {
                var themeName = americanCombined[i];
                var icon = americanLeagueGrid.GetChild(i).GetComponentInChildren<ThemePickerButton>();
                SetMLBMarkData(themeName, icon);
                yield return waitTime;
            }

            for (var i = 0; i < nationalCombined.Count; i++)
            {
                var themeName = nationalCombined[i];
                var icon = nationalLeagueGrid.GetChild(i).GetComponentInChildren<ThemePickerButton>();
                SetMLBMarkData(themeName, icon);
                yield return waitTime;
            }

            loadGridRoutine = null;
        }

        /// <summary>
        /// Loads the MLB mark textures into memory
        /// </summary>
        public void LoadMlbMarks()
        {
            mlbMarkData = Resources.Load<MlbMarkData>("MLBTextureData");
            IsMlbMarksLoaded = true;
        }

        /// <summary>
        /// Sets the sprite icon, onClick behavior, and onEnable behavior for target selectable icon
        /// </summary>
        private void SetMLBMarkData(MlbTheme theme, ThemePickerButton button)
        {
            var isSelected = SelectedTheme == theme;

            button.ThemeButton.interactable = false;
            button.ThemeImage.raycastTarget = false;
            button.CurrentTheme = theme;
            button.ThemeColor = "";

            // ---------------- OnEnable Sequence ---------------- \\
            var onEnableSequence = DOTween.Sequence();
            onEnableSequence.Insert(0, button.ThemeImage.DOFade(1, .5f).From(0));
            onEnableSequence.Insert(0, button.outline.DOFade(1, .5f).From(0));

            // Slace up then down & call onClickBehavior if the theme should be selected on startup
            onEnableSequence.Insert(.1f, button.transform.parent.DOScale(1.25f, .15f).SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    if (isSelected)
                    {
                        button.transform.parent.DOScale(1.1f, .35f).OnComplete(() => { button.OnClickBehavior(this, theme, visualOnly: true); });
                    }
                    else
                    {
                        button.transform.parent.DOScale(1, .35f);
                    }
                }));

            onEnableSequence.OnComplete(() =>
            {
                button.ThemeButton.interactable = true;
                button.ThemeImage.raycastTarget = true;
                DOTween.Kill(onEnableSequence);
            });

            onEnableSequence.Play();

            // ---------------- Init ---------------- \\
            button.Initialize(this, theme, onEnableSequence, MlbMarkData.GetOptionsByName(mlbMarkData, theme).mlbScreenSprite);
        }

        public void ChangeUniformRight(PointerEventData data)
        {
            if (SelectedThemeAsset == null)
            {
                return;
            }

            CurrentUniformSelectionIndex++;
            UpdateAvatarMaterials();
        }

        public void ChangeUniformLeft(PointerEventData data)
        {
            if (SelectedThemeAsset == null)
            {
                return;
            }

            CurrentUniformSelectionIndex--;
            UpdateAvatarMaterials();
        }

        /// <summary>
        /// Swaps out the avatars materials with the materials referenced in the current teams data
        /// </summary>
        public void UpdateAvatarMaterials(bool showHelmet = true)
        {
            if (SelectedThemeAsset == null)
            {
                return;
            }

            var isFemale = TeamBeingEdited.gameType.defaultGender == Globals.GENDER_FEMALE;

            try
            {
                if (CurrentState == TeamSelectionUIState.CustomTeam)
                {
                    uiElements.uniformName.SetText(customColors.SelectedColor);

                    if (isFemale)
                    {
                        EnableFemaleAvatar();

                        // set shirt, visor, and helmet color
                        femaleCharacter.GetComponent<CharacterFemale>().softBodyRenderer.materials[0].color = customColors.GetSelectedColor();
                        femaleCharacter.GetComponent<CharacterFemale>().softBodyRenderer.materials[1].color = customColors.GetSelectedColor();
                        femaleCharacter.GetComponent<CharacterFemale>().helmetRenderer.material.color = customColors.GetSelectedColor();
                    }
                    else
                    {
                        EnableMaleAvatar();

                        OverlayTextHandler.CurrentCamera.targetTexture = OverlayTextHandler.defaultRenderTexture;
                        OverlayTextHandler.rawImage.texture = OverlayTextHandler.colorJerseyTexture;

                        currentSelectedAvatar.helmet.material = MLBUniformManager.maleCustomAsset.teamUniforms[0].helmet;
                        currentSelectedAvatar.shirt.material = MLBUniformManager.maleCustomAsset.teamUniforms[0].jersey;
                        currentSelectedAvatar.pants.material = MLBUniformManager.maleCustomAsset.teamUniforms[0].pants;
                        currentSelectedAvatar.lShoe.material = MLBUniformManager.maleCustomAsset.teamUniforms[0].shoes;
                        currentSelectedAvatar.rShoe.material = MLBUniformManager.maleCustomAsset.teamUniforms[0].shoes;

                        OverlayTextHandler.rawImage.color = customColors.GetSelectedColor();
                        currentSelectedAvatar.helmet.material.color = customColors.GetSelectedColor();

                        OverlayTextHandler.SetTeamNameText(TeamBeingEdited.name, MLBUniformManager.maleCustomAsset.teamUniforms[0].font);
                    }
                }
                else
                {
                    uiElements.uniformName.SetText(SelectedThemeAsset.theme + " " + SelectedThemeAsset.teamUniforms[currentUniformSelectionIndex].uniformName);
                    OverlayTextHandler.rawImage.color = Color.white;

                    EnableMaleAvatar();

                    // Helmet & Cap 
                    if (!showHelmet && currentSelectedAvatar.cap != null && CurrentSelectedUniform.cap != null)
                    {
                        currentSelectedAvatar.cap.material = CurrentSelectedUniform.cap;
                        currentSelectedAvatar.helmet.gameObject.SetActive(false);
                    }

                    if (showHelmet && currentSelectedAvatar.helmet != null && CurrentSelectedUniform.helmet != null)
                    {
                        currentSelectedAvatar.helmet.material = CurrentSelectedUniform.helmet;

                        if (currentSelectedAvatar.cap != null)
                        {
                            currentSelectedAvatar.cap.gameObject.SetActive(false);
                        }
                    }

                    // Jersey
                    if (currentSelectedAvatar.shirt != null && CurrentSelectedUniform.jersey != null)
                    {
                        currentSelectedAvatar.shirt.material = CurrentSelectedUniform.jersey;
                    }

                    // Pants
                    if (currentSelectedAvatar.pants != null && CurrentSelectedUniform.pants != null)
                    {
                        currentSelectedAvatar.pants.material = CurrentSelectedUniform.pants;
                    }

                    // Shoes
                    if (currentSelectedAvatar.rShoe != null && CurrentSelectedUniform.shoes != null)
                    {
                        currentSelectedAvatar.rShoe.material = CurrentSelectedUniform.shoes;
                    }

                    if (currentSelectedAvatar.lShoe != null && CurrentSelectedUniform.shoes != null)
                    {
                        currentSelectedAvatar.lShoe.material = CurrentSelectedUniform.shoes;
                    }
                }
            }
            finally
            {
                UpdateAllButtonStates(CurrentState);
            }
        }

        /// <summary>
        /// Called when the user enters a new Picker State
        /// </summary>
        public void ResetAppearance(TeamSelectionUI.TeamSelectionUIState state, Team team, bool firstInit, bool newTeam)
        {
            var isFemale = team.gameType.defaultGender == Globals.GENDER_FEMALE;

            switch (state)
            {
                case TeamSelectionUIState.CustomTeam:
                    switch (isFemale)
                    {
                        case true:
                            EnableFemaleAvatar();
                            SelectTheme(MLBUniformManager.femaleCustomAsset);
                            break;
                        case false:
                            EnableMaleAvatar();
                            SelectTheme(MLBUniformManager.maleCustomAsset);
                            break;
                    }

                    if (customColors.SelectedColor == "")
                    {
                        customColors.colorButtons[0].ThemeButton.onClick.Invoke();
                    }
                    else
                    {
                        customColors.GetUserSelectedButton().ThemeButton.onClick.Invoke();
                    }

                    break;

                case TeamSelectionUIState.MLB:
                    if (team.mlbTheme != string.Empty && firstInit) // First init - saved data found
                    {
                        MlbTheme theme;
                        Enum.TryParse(TeamBeingEdited.mlbTheme, out theme);
                        SelectTheme(theme);
                    }
                    else if (!firstInit && MLBUniformManager.LastLoadedTheme != MlbTheme.None) // Previously loaded MLB theme - User has switched back to the MLB tab
                    {
                        SelectTheme(MLBUniformManager.LastLoadedTheme);
                    }
                    else // First init - no saved data
                    {
                        MlbTheme theme;
                        Enum.TryParse("BlueJays", out theme);
                        SelectTheme(theme);
                    }

                    break;
            }
        }

        private void ResetToHomeUniform()
        {
            if (SelectedThemeAsset == null)
            {
                Debug.LogError("currentSelected team is null - will not update materials ");
                return;
            }

            currentUniformSelectionIndex = 0;
            UpdateAvatarMaterials();
        }

        private void ResetLogoGrid(Transform grid)
        {
            var rectTransform = grid.GetComponent<RectTransform>();

            for (var i = 0; i < rectTransform.childCount; i++)
            {
                rectTransform.GetChild(i).GetComponentInChildren<ThemePickerButton>().ThemeImage.DOFade(0, 0);
                rectTransform.GetChild(i).GetComponentInChildren<ThemePickerButton>().outline.DOFade(0, 0);
            }
        }

        private void EnableFemaleAvatar()
        {
            currentSelectedAvatar = femaleCharacter;
            currentSelectedAvatar.gameObject.SetActive(false);
            femaleCharacter.gameObject.SetActive(true);
            maleCharacter.gameObject.SetActive(false);
        }

        private void EnableMaleAvatar()
        {
            currentSelectedAvatar = maleCharacter;
            currentSelectedAvatar.gameObject.SetActive(false);
            maleCharacter.gameObject.SetActive(true);
            femaleCharacter.gameObject.SetActive(false);
        }

        public void SelectTheme(MlbThemeData asset)
        {
            SelectedThemeAsset = asset;
            ResetToHomeUniform();
            UpdateAvatarMaterials();
        }

        public void SelectTheme(MlbTheme theme)
        {
            if (theme == MlbTheme.None)
            {
                theme = (TeamBeingEdited.gameType.defaultGender == Globals.GENDER_FEMALE) ? MlbTheme.CustomFemale : MlbTheme.CustomMale;
            }

            MLBUniformManager.LoadThemeAssetAsync(theme, onBegin: HideResourceLoadTween, onComplete: (mlbThemeAsset) =>
            {
                if (mlbThemeAsset == null)
                {
                    return;
                }

                SelectedThemeAsset = mlbThemeAsset;
                SelectedTheme = theme;

                if (SelectedTheme != MlbTheme.CustomMale && SelectedTheme != MlbTheme.CustomFemale)
                {
                    MLBUniformManager.LastLoadedTheme = SelectedTheme;
                }

                ResetToHomeUniform();
                UpdateAvatarMaterials();
                uniformChangeCount++;

                // Let's just check to see how many uniforms we've changed, and unload all of the unused after after we've changed 5 times
                // We don't want to call Resources.UnloadUnusedAssets() every time as it can be costly
                if (uniformChangeCount > 5)
                {
                    Resources.UnloadUnusedAssets();
                    uniformChangeCount = 0;
                }
            });
        }
    }
}
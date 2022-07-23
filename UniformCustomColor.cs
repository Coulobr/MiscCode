using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace HitTrax.Animations
{
    public class UniformCustomColor : MonoBehaviour
    {
        public TeamColorData colorData;
        public event Action<string> ChangeColor;

        public bool Initialized { get; set; }
        public GridLayoutGroup colorGrid { get; set; }
        public List<ThemePickerButton> colorButtons { get; set; }
        public string DefaultColor => colorData.teamColors[0].colorThemeName;
        public string SelectedColor { get; set; } = "Red"; //Default red

        public void Initialize()
        {
	        colorGrid = GetComponentInChildren<GridLayoutGroup>();
            colorButtons = GetComponentsInChildren<ThemePickerButton>().ToList();

            var uiInstance = TeamSelectionUI.Instance;

            if (colorData.teamColors.Count != colorButtons.Count)
            {
                Debug.LogError("The number of color buttons must match the number of available colors in TeamBattleColorData");
                Initialized = false;
            }

            // For each button we set its on click behavior and color
            for (var i = 0; i < colorButtons.Count; i++)
            {
                var colorName = colorData.teamColors[i].colorThemeName;
                var buttonTransform = colorButtons[i].GetComponent<RectTransform>();
                buttonTransform.parent.localScale = Vector3.one;
                var isFemale = uiInstance.TeamBeingEdited.gameType.defaultGender == Globals.GENDER_FEMALE;

                colorButtons[i].ThemeColor = colorName;
                colorButtons[i].CurrentTheme = isFemale ? MlbTheme.CustomFemale : MlbTheme.CustomMale;
                colorButtons[i].ThemeImage.color = GetColorByNameAndState(colorData, colorName);
                colorButtons[i].ThemeButton.onClick.AddListener(() => 
                { 
                    SelectNewColor(colorName); 
                });
                colorButtons[i].Initialize(uiInstance, uiInstance.MLBUniformManager.maleCustomAsset);
                colorButtons[i].ThemeImage.DOFade(1, 0);
            }

            Initialized = true;
        }

        /// <summary>
        /// When the user selects a new color in the custom team UI
        /// </summary>
        private void SelectNewColor(string color)
        {
            SelectedColor = color;
            TeamSelectionUI.Instance.UpdateAvatarMaterials();
        }

        public static Color GetColorByNameAndState(TeamColorData colData, string desiredName = "Red", TeamColorState desiredState = TeamColorState.Primary)
        {
            if (desiredName != "")
            {
                for (var i = 0; i < colData.teamColors.Count; ++i)
                {
                    if (desiredName == colData.teamColors[i].colorThemeName)
                    {
                        return TeamColorData.GetColorByState(colData.teamColors[i], desiredState);
                    }
                }
            }

            return TeamColorData.GetColorByState(colData.teamColors[0], desiredState);
        }

        public Color GetSelectedColor()
		{
            return GetColorByNameAndState(colorData, SelectedColor);
		}

        /// <summary>
        /// Loop through grid and assign colors
        /// </summary>
        /// <returns></returns>
        public ThemePickerButton GetUserSelectedButton()
        {
	        var ui = TeamSelectionUI.Instance;
	        foreach (var button in ui.customColors.colorButtons)
	        {
		        if (button.ThemeColor == SelectedColor)
		        {
			        return button;
		        }
	        }

	        return null;
        }
    }
}
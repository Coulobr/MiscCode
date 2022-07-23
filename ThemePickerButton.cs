using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

#region ReSharper Comments
// ReSharper disable UnusedMember.Global
#endregion

namespace HitTrax.Animations
{
    [RequireComponent(typeof(Image), typeof(Button))]
    public class ThemePickerButton : MonoBehaviour
    {
        public Sprite Sprite
        {
            get { return ThemeImage != null ? ThemeImage.sprite : null; }
            set { ThemeImage.sprite = value; }
        }

        public Button ThemeButton { get; set; }
        public Image ThemeImage { get; private set; }
        public string ThemeColor { get; set; } = null;
        public MlbTheme CurrentTheme { get; set; }
        public Image outline;

        private void Awake()
        {
            ThemeImage = GetComponent<Image>();
            ThemeButton = GetComponent<Button>();
        }

        void OnEnable()
        {
            ThemeImage.DOFade(0, 0);
        }

        /// <summary>
        /// Sets the icons spirte and onClick behavior
        /// </summary>
        public void Initialize(TeamSelectionUI module, MlbTheme themeName, Sequence onEnableSequence = null, Sprite logo = null)
        {
            if (logo != null)
            {
                SetImage(logo);
            }

            ThemeButton.onClick.AddListener(() => OnClickBehavior(module, themeName, onEnableSequence));
        }

        public void Initialize(TeamSelectionUI module, MlbThemeData data, Sequence onEnableSequence = null, Sprite logo = null)
        {
            if (logo != null)
            {
                SetImage(logo);
            }

            ThemeButton.onClick.AddListener(() => OnClickBehavior(module, data.theme, onEnableSequence));
        }

        private void SetImage(Sprite sprite) => Sprite = sprite;

        public void OnClickBehavior(TeamSelectionUI ui, MlbTheme themeName, Sequence onEnableSequence = null, bool visualOnly = false)
        {
            if (onEnableSequence != null && onEnableSequence.IsPlaying())
            {
                return;
            }

            // De-select the old button
            if (ui.CurrentSelectedButton != null)
            {
                ui.CurrentSelectedButton.transform.parent.localScale = Vector3.one;
            }

            // Select the new button we just clicked
            ui.CurrentSelectedButton = this;

            var parent = ThemeButton.transform.parent;
            parent.localScale = new Vector3(1.1f, 1.1f, 1f);

            var glow = ui.UiElements.selectionGlow.rectTransform;
            glow.GetComponent<Image>().enabled = true;
            glow.SetParent(parent);
            glow.SetAsFirstSibling();
            glow.localScale = Vector3.one;
            glow.localPosition = Vector2.zero;

            if (!visualOnly)
            {
                // change the avatar visual as well
                ui.SelectTheme(themeName);
            }
        }
    }
}
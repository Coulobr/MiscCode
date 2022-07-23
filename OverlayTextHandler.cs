using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace HitTrax.Animations
{
    public class OverlayTextHandler : SingletonBehaviour<OverlayTextHandler>
    {
        [FormerlySerializedAs("Texture")] public RawImage rawImage;
        [FormerlySerializedAs("DefaultRenderTexture")] public RenderTexture defaultRenderTexture;
        [FormerlySerializedAs("ColorJerseyTexture")] public Texture colorJerseyTexture;
        [FormerlySerializedAs("ColorJerseyTexture")] public Texture femaleJerseyTexture;

        private Camera currentCamera;
        [SerializeField] private TextMeshPro playerNameText;
        [SerializeField] private TextMeshPro playerNumberText;
        [SerializeField] private TextMeshPro teamNameTextLeft;
        [SerializeField] private TextMeshPro teamNameTextRight;

        public Camera CurrentCamera => currentCamera;

        private void Awake()
        {
            currentCamera = GetComponentInChildren<Camera>();
        }

        private void Start()
        {
            SetCameraRect();
        }

        public void Initialize()
        {
            rawImage.texture = colorJerseyTexture;
        }

        /// <summary>
        /// Returns a new render texture with the applied overlays
        /// </summary>
        public RenderTexture OverlayTextJersey(Renderer shirt, MlbUniform uniformData, Color teamColor)
        {
            var texture = shirt.material.mainTexture;

            //If the user has a custom color, set the jersey type to colored. Otherwise its an MLB uniform (no name shown)
            if (uniformData.isCustomColor)
            {
                texture = colorJerseyTexture;
                rawImage.color = teamColor;
            }
            else
            {
                uniformData.desiredTeamName = "";
                rawImage.color = Color.white;
            }

            // Set shirt
            SetMainTexture(texture);

            // Apply overlays & re-render
            var textureWithOverlays = GetTextureCopy(
                teamName: uniformData.desiredTeamName,
                number: Random.Range(1, 100).ToString(),
                playerName: null,
                fontData: uniformData.font,
                isCustomColor: uniformData.isCustomColor);

            // Set shirt again
            return textureWithOverlays;
        }

        public RenderTexture GetTextureCopy(TeamFontData fontData = null, string number = null, string playerName = null, string teamName = null, bool isCustomColor = false)
        {
            SetPlayerNumberText(number, fontData);
            SetPlayerNameText(playerName, fontData);
            SetTeamNameText(teamName, fontData);

            if (isCustomColor)
            {
                SetMainTexture(colorJerseyTexture);
            }
            else
            {
                rawImage.color = Color.white; // Reset to white for MLB team jersey
            }

            var renderTexture = new RenderTexture(1024, 1024, 8);
            currentCamera.targetTexture = renderTexture;
            currentCamera.Render();
            currentCamera.targetTexture = null;

            return renderTexture;
        }

        public void SetPlayerNameText(string text, TeamFontData fontData = null)
        {
            if (text == null)
            {
                text = string.Empty;
            }
            else if (fontData != null)
            {
                playerNameText.font = fontData.generalNameFontAsset;
            }

            playerNameText.text = text;
        }

        public void SetPlayerNumberText(string text, TeamFontData fontData = null)
        {
            if (text == null)
            {
                text = string.Empty;
            }
            else if (fontData != null)
            {
                playerNumberText.font = fontData.generalNumberFontAsset;
            }

            playerNumberText.text = text;
        }

        public void SetTeamNameText(string text, TeamFontData fontData = null)
        {
            if (text == null)
            {
                text = string.Empty;
            }
            else if (fontData != null)
            {
                teamNameTextLeft.font = fontData.generalNameFontAsset;
                teamNameTextRight.font = fontData.generalNameFontAsset;
            }

            var charCount = text.Length;

            if (charCount > 16)
            {
                Debug.LogWarning("Name too long");
            }
            else
            {
                var firstHalfCharCount = Mathf.RoundToInt(charCount / 2f);
                var rightPortion = text.Substring(0, firstHalfCharCount);
                var leftPortion = text.Substring(firstHalfCharCount, charCount - firstHalfCharCount);

                teamNameTextLeft.text = leftPortion;
                teamNameTextRight.text = rightPortion;
                teamNameTextLeft.fontSize = teamNameTextRight.fontSize;
            }
        }

        public void SetMainTexture(Texture texture)
        {
            rawImage.texture = texture;
            SetCameraRect();
        }

        private void SetCameraRect()
        {
            currentCamera.aspect = 1f;
            currentCamera.rect = new Rect(Vector2.zero, new Vector2(rawImage.mainTexture.width, rawImage.mainTexture.height));
        }
    }
}
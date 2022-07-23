using System;
using System.Collections.Generic;
using UnityEngine;

namespace HitTrax.Animations
{
    [CreateAssetMenu(menuName = "TeamTextureData")]
    public class MlbMarkData : ScriptableObject
    {
        [SerializeField] private List<TextureOptions> teams;

        [Serializable]
        public struct TextureOptions
        {
#if UNITY_EDITOR
            public string name;
#endif
            public MlbTheme theme;
            public Sprite mlbScreenSprite;
            public Texture teamSetupTexture;
        }

        public static TextureOptions GetOptionsByName(MlbMarkData data, MlbTheme theme)
        {
            foreach (var options in data.teams)
            {
                if (options.theme == theme)
                {
                    return options;
                }
            }

            return new TextureOptions();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            for (int i = 0; i < teams.Count; i++)
            {
                var newOptions = teams[i];
                newOptions.name = teams[i].theme.ToString();
                teams[i] = newOptions;
            }
        }
#endif
    }
}
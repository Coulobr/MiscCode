using System.Collections.Generic;
using UnityEngine;

namespace HitTrax.Animations
{
    [CreateAssetMenu(menuName = "UniformChanger/TeamSelectionData")]
    public class MlbThemeData : ScriptableObject
    {
        [Header("Generic Team Data")] public MlbTheme theme;
        [Header("Uniform Data")] public List<MlbUniform> teamUniforms;
    }
}
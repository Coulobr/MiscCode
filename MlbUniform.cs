using UnityEngine;
using UnityEngine.Serialization;

namespace HitTrax.Animations
{
	[CreateAssetMenu(menuName = "UniformChanger/Uniform")]
    public class MlbUniform : ScriptableObject
    {
        [FormerlySerializedAs("Name")] public string uniformName;
        [FormerlySerializedAs("Font")] public TeamFontData font;
        [FormerlySerializedAs("IsCustomColor")] [HideInInspector] public bool isCustomColor = false;

        [FormerlySerializedAs("DesiredTeamName")] [HideInInspector] public string desiredTeamName = "";

        [Header("Uniform Materials")]
        [FormerlySerializedAs("Helmet")] public Material helmet;
        [FormerlySerializedAs("Cap")] public Material cap;
        [FormerlySerializedAs("Jersey")] public Material jersey;
        [FormerlySerializedAs("Pants")] public Material pants;
        [FormerlySerializedAs("Shoes")] public Material shoes;
    }
}
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace HitTrax.Animations
{
	[CreateAssetMenu(menuName = "UniformChanger/TeamFontData")]
	public class TeamFontData : ScriptableObject {
		
		[Header("Font Data")] 
		[FormerlySerializedAs("GeneralNameFontAsset")] public TMP_FontAsset generalNameFontAsset;
		[FormerlySerializedAs("GeneralNumberFontAsset")] public TMP_FontAsset generalNumberFontAsset;
        
		[Header("Front Number Settings")]
		[FormerlySerializedAs("FrontNumberOffset")] public Vector2 frontNumberOffset;
		[FormerlySerializedAs("FrontNumberFontSize")] public float frontNumberFontSize;
		[FormerlySerializedAs("FrontNumberFontOverride")] public TMP_FontAsset frontNumberFontOverride;

		[Header("Front Name Settings")]
		[FormerlySerializedAs("FrontNameOffset")] public Vector2 frontNameOffset;
		[FormerlySerializedAs("FrontNameFontSize")] public float frontNameFontSize;
		[FormerlySerializedAs("FrontNameFontOverride")] public TMP_FontAsset frontNameFontOverride;

		[Header("Back Number Settings")]
		[FormerlySerializedAs("BackNumberOffset")] public Vector2 backNumberOffset;
		[FormerlySerializedAs("BackNumberFontSize")] public float backNumberFontSize;
		[FormerlySerializedAs("BackNumberFontOverride")] public TMP_FontAsset backNumberFontOverride;

		[Header("Back Name Settings")]
		[FormerlySerializedAs("BackNameOffset")] public Vector2 backNameOffset;
		[FormerlySerializedAs("BackNameFontSize")] public float backNameFontSize;
		[FormerlySerializedAs("BackNameFontOverride")] public TMP_FontAsset backNameFontOverride;
	}
}

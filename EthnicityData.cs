using UnityEngine;
using UnityEngine.Serialization;

namespace HitTrax.Animations
{
	[CreateAssetMenu(menuName = "UniformChanger/EthnicityData")]
	public class EthnicityData : ScriptableObject
	{ 
		[Header("Asian")]
		[FormerlySerializedAs("AsianHeadMesh")] public Mesh asianHeadMesh;
		[FormerlySerializedAs("AsianFaceMaterial")] public Material asianFaceMaterial;
		[FormerlySerializedAs("AsianArmsMaterial")] public Material asianArmsMaterial;

		[Header("Caucasian")]
		[FormerlySerializedAs("CaucasianHeadMesh")] public Mesh caucasianHeadMesh;
		[FormerlySerializedAs("CaucasianFaceMaterial")] public Material caucasianFaceMaterial;
		[FormerlySerializedAs("CaucasianArmsMaterial")] public Material caucasianArmsMaterial;

		[Header("AfricanAmerican")]
		[FormerlySerializedAs("AfricanAmericanHeadMesh")] public Mesh africanAmericanHeadMesh;
		[FormerlySerializedAs("AfricanAmericanFaceMaterial")] public Material africanAmericanFaceMaterial;
		[FormerlySerializedAs("AfricanAmericanArmsMaterial")] public Material africanAmericanArmsMaterial;

		[Header("Hispanic American")]
		[FormerlySerializedAs("HispanicAmericanHeadMesh")] public Mesh hispanicAmericanHeadMesh;
		[FormerlySerializedAs("HispanicAmericanFaceMaterial")] public Material hispanicAmericanFaceMaterial;
		[FormerlySerializedAs("HispanicAmericanArmsMaterial")] public Material hispanicAmericanArmsMaterial;
	}
}



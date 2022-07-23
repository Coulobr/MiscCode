using System.Collections.Generic;
using UnityEngine;

namespace HitTrax.Bataround
{
	public class MeshGenerator
	{
		public static void CreatePolyMesh(GameObject emptyObject, Material material, float radius, int numSides, Color col, Vector3 position, Quaternion rotation, int layer, bool addCollider = true)
		{
			var meshRenderer = emptyObject.AddComponent<MeshRenderer>();
			var meshFilter = emptyObject.AddComponent<MeshFilter>();

			Mesh mesh = new Mesh();
			meshFilter.mesh = mesh;

			emptyObject.transform.position = position;
			emptyObject.transform.rotation = rotation;
			emptyObject.layer = layer;

			// mesh data lists
			List<Vector3> verticiesList = new List<Vector3> { };
			List<int> trianglesList = new List<int> { };
			List<Vector3> normalsList = new List<Vector3> { };

			//verts
			float x;
			float y;
			for (int i = 0; i < numSides; i++)
			{
				x = radius * Mathf.Sin((2 * Mathf.PI * i) / numSides);
				y = radius * Mathf.Cos((2 * Mathf.PI * i) / numSides);
				verticiesList.Add(new Vector3(x, y, 0f));
			}
			Vector3[] verticies = verticiesList.ToArray();

			//triangles
			for (int i = 0; i < (numSides - 2); i++)
			{
				trianglesList.Add(0);
				trianglesList.Add(i + 1);
				trianglesList.Add(i + 2);
			}
			int[] triangles = trianglesList.ToArray();

			//normals
			for (int i = 0; i < verticies.Length; i++)
			{
				normalsList.Add(-Vector3.forward);
			}
			Vector3[] normals = normalsList.ToArray();

			//initialise
			mesh.vertices = verticies;
			mesh.triangles = triangles;
			mesh.normals = normals;

			if (addCollider)
			{
				var collider = meshFilter.gameObject.AddComponent<MeshCollider>();
				collider.convex = true;
				collider.isTrigger = true;
				collider.sharedMesh = meshFilter.mesh;
			}
		}

		public static void CreateSquareMesh(GameObject emptyObject, Material material, Color color, Vector3 position, Quaternion rotation, float scale, int layer, bool addCollider = true)
		{
			Mesh mesh = new Mesh();

			var meshRenderer = emptyObject.AddComponent<MeshRenderer>();
			var meshFilter = emptyObject.AddComponent<MeshFilter>();

			emptyObject.transform.position = position - new Vector3(scale / 2, 0, scale / 2);
			emptyObject.transform.rotation = rotation;
			emptyObject.transform.localScale = new Vector3(scale, scale, 1);
			emptyObject.layer = layer;

			Vector3[] verts = new Vector3[4];
			Vector2[] uv = new Vector2[4];
			int[] triangles = new int[6];

			verts[0] = new Vector3(0, 1);
			verts[1] = new Vector3(1, 1);
			verts[2] = new Vector3(0, 0);
			verts[3] = new Vector3(1, 0);

			uv[0] = new Vector2(0, 1);
			uv[1] = new Vector2(1, 1);
			uv[2] = new Vector2(0, 0);
			uv[3] = new Vector2(1, 0);

			triangles[0] = 0;
			triangles[1] = 1;
			triangles[2] = 2;
			triangles[3] = 2;
			triangles[4] = 1;
			triangles[5] = 3;

			mesh.vertices = verts;
			mesh.uv = uv;
			mesh.triangles = triangles;
			meshFilter.mesh = mesh;

			meshRenderer.material = material;
			meshRenderer.material.color = color;

			if (addCollider)
			{
				var collider = meshFilter.gameObject.AddComponent<MeshCollider>();
				collider.convex = true;
				collider.isTrigger = true;
				collider.sharedMesh = meshFilter.mesh;
			}
		}
	}
}
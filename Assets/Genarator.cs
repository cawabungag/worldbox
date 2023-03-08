using System.Collections.Generic;
using SimplexNoise;
using UnityEngine;
using UnityEngine.UI;

public class Genarator : MonoBehaviour
{
	public Button Generate;
	public int PlanetRadius;
	public int Spacing;
	public bool IsCalcBumps;
	public int Scale;
	public int PerlinRequire;
	byte[,,] Voxels;

	void Start()
	{
		Generate.onClick.AddListener(GeneratePlanet);
	}

	private void GeneratePlanet()
	{
		for (int x = -PlanetRadius; x < PlanetRadius; x++)
		{
			for (int y = -PlanetRadius; y < PlanetRadius; y++)
			{
				for (int z = -PlanetRadius; z < PlanetRadius; z++)
				{
					var isInnerPlanet = Mathf.Sqrt(Mathf.Pow(x, 2) + Mathf.Pow(y, 2) + Mathf.Pow(z, 2)) <= PlanetRadius;
					if (isInnerPlanet)
					{
						if (IsCalcBumps)
						{
							var w = Noise.CalcPixel3D(x, y, z, Scale);
							Debug.Log($"W: {w}");
							if (w < PerlinRequire)
								continue;
						}

						var position = new Vector3Int(x * Spacing, y * Spacing, z * Spacing);
						GenerateMesh(position);
					}
				}
			}
		}
	}
	
	void GenSquare(int x, int y, Vector2 texture){
  
		newVertices.Add( new Vector3 (x  , y  , 0 ));
		newVertices.Add( new Vector3 (x + 1 , y  , 0 ));
		newVertices.Add( new Vector3 (x + 1 , y-1 , 0 ));
		newVertices.Add( new Vector3 (x  , y-1 , 0 ));
  
		newTriangles.Add(squareCount*4);
		newTriangles.Add((squareCount*4)+1);
		newTriangles.Add((squareCount*4)+3);
		newTriangles.Add((squareCount*4)+1);
		newTriangles.Add((squareCount*4)+2);
		newTriangles.Add((squareCount*4)+3);
  
		newUV.Add(new Vector2 (tUnit * texture.x, tUnit * texture.y + tUnit));
		newUV.Add(new Vector2 (tUnit*texture.x+tUnit, tUnit*texture.y+tUnit));
		newUV.Add(new Vector2 (tUnit * texture.x + tUnit, tUnit * texture.y));
		newUV.Add(new Vector2 (tUnit * texture.x, tUnit * texture.y));
  
		squareCount++;
  
	}
	
	List<int> triangles = new();
	List<Vector3> verticies = new();
	List<Vector2> uv = new();
	private int squareCount;

	private void GenerateMesh(Vector3Int pos)
	{
		
		//
		// Vector3[] vertPos = {
		// 	new(-1, 1, -1), new(-1, 1, 1),
		// 	new(1, 1, 1), new(1, 1, -1),
		// 	new(-1, -1, -1), new(-1, -1, 1),
		// 	new(1, -1, 1), new(1, -1, -1),
		// };
		//
		// int[,] faces = {
		// 	{0, 1, 2, 3, 0, 1, 0, 0, 0}, //top
		// 	{7, 6, 5, 4, 0, -1, 0, 1, 0}, //bottom
		// 	{2, 1, 5, 6, 0, 0, 1, 1, 1}, //right
		// 	{0, 3, 7, 4, 0, 0, -1, 1, 1}, //left
		// 	{3, 2, 6, 7, 1, 0, 0, 1, 1}, //front
		// 	{1, 0, 4, 5, -1, 0, 0, 1, 1} //back
		// };
		//
		// var x = pos.x;
		// var y = pos.y;
		// var z = pos.z;
		//
		// for (int o = 0; o < 6; o++)
		// {
		// 	var facenum = o;
		// 	var v = verticies.Count;
		// 	for (int i = 0; i < 4; i++)
		// 	{
		// 		verticies.Add(new Vector3(x, y, z) + vertPos[faces[facenum, i]] / 2f);
		// 	}
		//
		// 	triangles.AddRange(new List<int> {v, v + 1, v + 2, v, v + 2, v + 3});
		// 	var bottomleft = new Vector2(faces[facenum, 7], faces[facenum, 8]) / 2f;
		//
		// 	uv.AddRange(new List<Vector2>
		// 	{
		// 		bottomleft + new Vector2(0, 0.5f), bottomleft + new Vector2(0.5f, 0.5f),
		// 		bottomleft + new Vector2(0.5f, 0), bottomleft
		// 	});
		// }

		Debug.LogError($"{verticies.Count}");
		Debug.LogError($"{triangles.Count}");
		Debug.LogError($"{uv.Count}");
		
		GetComponent<MeshFilter>().mesh = new Mesh
		{
			vertices = verticies.ToArray(),
			triangles = triangles.ToArray(),
			uv = uv.ToArray()
		};
	}
}
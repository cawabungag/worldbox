// using System.Collections.Generic;
// using Game.Services.MapGenerator;
// using Game.Services.MapGenerator.Impls;
// using UnityEngine;
// using UnityEngine.Rendering;
//
// public class VoxelWorld : MonoBehaviour
// {
// 	public int worldSize = 16;
// 	public float voxelSize = 1.0f;
//
// 	private Mesh voxelMesh;
// 	private List<Vector3> vertices = new();
// 	private List<int> triangles = new();
// 	private List<Vector2> textures = new();
// 	private int faceCount;
//
//
//
// 	void Start()
// 	{
// 		voxelMesh = new Mesh();
// 		GenerateVoxels();
// 		UpdateMesh();
// 	}
//
// 	void GenerateVoxels()
// 	{
// 		vertices.Clear();
// 		triangles.Clear();
// 		textures.Clear();
// 		faceCount = 0;
// 		
// 		// CreateCube(0, 0, 0, (byte) ETileType.Forest);
// 		// CreateCube(1, 0, 0, (byte) ETileType.Plain);
// 		// CreateCube(2, 0, 0, (byte) ETileType.Rock);
// 		// CreateCube(3, 0, 0, (byte) ETileType.Sand);
// 		// CreateCube(4, 0, 0, ETileType.Water);
// 		// CreateCube(5, 0, 0, (byte) ETileType.GroundWater);
//
// 		IMapGenerator mapGenerator = new MapGenerator();
// 		var ground = mapGenerator.GenerateGround(worldSize, worldSize);
// 		
// 		var width = worldSize;
// 		var halfWidth = width / 2;
// 		var height = worldSize;
// 		var halfHeight = height / 2;
// 		
// 		for (var i = 0; i < height; i++)
// 		{
// 			for (var j = 0; j < width; j++)
// 			{
// 				var x = j - halfWidth;
// 				var y = i - halfHeight;
// 				var tileType = ground[height * i + j];
// 				Debug.LogError($"CreateCube: x:{x},y:{y} tileType:{tileType}");
// 				for (int k = 0; k < (byte)tileType; k++)
// 				{
// 					CreateCube(x, k, y, tileType);
// 				}
// 			}
// 		}
// 	}
//
// 	void UpdateMesh()
// 	{
// 		voxelMesh.Clear();
// 		voxelMesh.indexFormat = IndexFormat.UInt32;
// 		voxelMesh.vertices = vertices.ToArray();
// 		voxelMesh.triangles = triangles.ToArray();
// 		voxelMesh.uv = textures.ToArray();
// 		voxelMesh.RecalculateNormals();
// 		voxelMesh.Optimize();
//
// 		var meshFilter = GetComponent<MeshFilter>();
// 		if (meshFilter == null)
// 		{
// 			meshFilter = gameObject.AddComponent<MeshFilter>();
// 		}
//
// 		meshFilter.mesh = voxelMesh;
// 	}
//
// 	private int cubeCount;
//
// 	void CreateCube(int x, int y, int z, VoxelType voxelType)
// 	{
// 		// Front
// 		if (IsTransparent(x, y, z + 1))
// 		{
// 			vertices.Add(new Vector3(x, y, z + 1) * voxelSize);
// 			vertices.Add(new Vector3(x + 1, y, z + 1) * voxelSize);
// 			vertices.Add(new Vector3(x + 1, y + 1, z + 1) * voxelSize);
// 			vertices.Add(new Vector3(x, y + 1, z + 1) * voxelSize);
//
// 			triangles.Add(faceCount * 4 + 0);
// 			triangles.Add(faceCount * 4 + 1);
// 			triangles.Add(faceCount * 4 + 2);
// 			triangles.Add(faceCount * 4 + 0);
// 			triangles.Add(faceCount * 4 + 2);
// 			triangles.Add(faceCount * 4 + 3);
//
// 			AddTexture(voxelType);
//
// 			faceCount++;
// 		}
//
// 		// Back
// 		if (IsTransparent(x, y, z - 1))
// 		{
// 			vertices.Add(new Vector3(x + 1, y, z) * voxelSize);
// 			vertices.Add(new Vector3(x, y, z) * voxelSize);
// 			vertices.Add(new Vector3(x, y + 1, z) * voxelSize);
// 			vertices.Add(new Vector3(x + 1, y + 1, z) * voxelSize);
//
// 			triangles.Add(faceCount * 4 + 0);
// 			triangles.Add(faceCount * 4 + 1);
// 			triangles.Add(faceCount * 4 + 2);
// 			triangles.Add(faceCount * 4 + 0);
// 			triangles.Add(faceCount * 4 + 2);
// 			triangles.Add(faceCount * 4 + 3);
//
// 			AddTexture(voxelType);
//
// 			faceCount++;
// 		}
//
// 		// Top
// 		if (IsTransparent(x, y + 1, z))
// 		{
// 			vertices.Add(new Vector3(x, y + 1, z) * voxelSize);
// 			vertices.Add(new Vector3(x + 1, y + 1, z) * voxelSize);
// 			vertices.Add(new Vector3(x + 1, y + 1, z + 1) * voxelSize);
// 			vertices.Add(new Vector3(x, y + 1, z + 1) * voxelSize);
//
// 			triangles.Add(faceCount * 4 + 0);
// 			triangles.Add(faceCount * 4 + 1);
// 			triangles.Add(faceCount * 4 + 2);
// 			triangles.Add(faceCount * 4 + 0);
// 			triangles.Add(faceCount * 4 + 2);
// 			triangles.Add(faceCount * 4 + 3);
//
// 			AddTexture(voxelType);
//
// 			faceCount++;
// 		}
//
// 		// Bottom
// 		if (IsTransparent(x, y - 1, z))
// 		{
// 			vertices.Add(new Vector3(x + 1, y, z) * voxelSize);
// 			vertices.Add(new Vector3(x, y, z) * voxelSize);
// 			vertices.Add(new Vector3(x, y, z + 1) * voxelSize);
// 			vertices.Add(new Vector3(x + 1, y, z + 1) * voxelSize);
//
// 			triangles.Add(faceCount * 4 + 0);
// 			triangles.Add(faceCount * 4 + 1);
// 			triangles.Add(faceCount * 4 + 2);
// 			triangles.Add(faceCount * 4 + 0);
// 			triangles.Add(faceCount * 4 + 2);
// 			triangles.Add(faceCount * 4 + 3);
//
// 			AddTexture(voxelType);
//
// 			faceCount++;
// 		}
//
// 		// Left
// 		if (IsTransparent(x - 1, y, z))
// 		{
// 			vertices.Add(new Vector3(x, y, z) * voxelSize);
// 			vertices.Add(new Vector3(x, y, z + 1) * voxelSize);
// 			vertices.Add(new Vector3(x, y + 1, z + 1) * voxelSize);
// 			vertices.Add(new Vector3(x, y + 1, z) * voxelSize);
//
// 			triangles.Add(faceCount * 4 + 0);
// 			triangles.Add(faceCount * 4 + 1);
// 			triangles.Add(faceCount * 4 + 2);
// 			triangles.Add(faceCount * 4 + 0);
// 			triangles.Add(faceCount * 4 + 2);
// 			triangles.Add(faceCount * 4 + 3);
//
// 			AddTexture(voxelType);
//
// 			faceCount++;
// 		}
//
// 		// Right
// 		if (IsTransparent(x + 1, y, z))
// 		{
// 			vertices.Add(new Vector3(x + 1, y, z + 1) * voxelSize);
// 			vertices.Add(new Vector3(x + 1, y, z) * voxelSize);
// 			vertices.Add(new Vector3(x + 1, y + 1, z) * voxelSize);
// 			vertices.Add(new Vector3(x + 1, y + 1, z + 1) * voxelSize);
// 			
// 			triangles.Add(faceCount * 4 + 0);
// 			triangles.Add(faceCount * 4 + 1);
// 			triangles.Add(faceCount * 4 + 2);
// 			triangles.Add(faceCount * 4 + 0);
// 			triangles.Add(faceCount * 4 + 2);
// 			triangles.Add(faceCount * 4 + 3);
//
// 			AddTexture(voxelType);
//
// 			faceCount++;
// 		}
// 	}
//
// 	void AddTexture(VoxelType textureIndex)
// 	{
// 		// var y = this.textureIndex / (float) textureAtlasSizeInBlocks;
// 		// var x = this.textureIndex - y * textureAtlasSizeInBlocks;
// 		//
// 		// x *= normalizedBlockTextureSize;
// 		// y *= normalizedBlockTextureSize;
// 		// y = 1f - y - normalizedBlockTextureSize;
// 		// textures.Add(new Vector2(x, y));
// 		// textures.Add(new Vector2(x + normalizedBlockTextureSize, y));
// 		// textures.Add(new Vector2(x + normalizedBlockTextureSize, y + normalizedBlockTextureSize));
// 		// textures.Add(new Vector2(x, y + normalizedBlockTextureSize));
//
// 		switch (textureIndex)
// 		{
// 			case VoxelType.GroundWater:
// 				textures.Add(groundA);
// 				textures.Add(groundb);
// 				textures.Add(groundc);
// 				textures.Add(groundd);
// 				break;
// 			case VoxelType.Water:
// 				textures.Add(waterA);
// 				textures.Add(waterb);
// 				textures.Add(waterc);
// 				textures.Add(waterd);
// 				break;
// 			case VoxelType.Sand:
// 				textures.Add(sandA);
// 				textures.Add(sandb);
// 				textures.Add(sandc);
// 				textures.Add(sandd);
// 				break;
// 			case VoxelType.Plain:
// 				textures.Add(grassA);
// 				textures.Add(grassb);
// 				textures.Add(grassc);
// 				textures.Add(grassd);
// 				break;
// 			case VoxelType.Forest:
// 				textures.Add(forestA);
// 				textures.Add(forestb);
// 				textures.Add(forestc);
// 				textures.Add(forestd);
// 				break;
// 			case VoxelType.Rock:
// 				textures.Add(stoneA);
// 				textures.Add(stoneb);
// 				textures.Add(stonec);
// 				textures.Add(stoned);
// 				break;
// 		}
// 		
// 	}
//
// 	bool IsTransparent(int x, int y, int z)
// 	{
// 		// if (x < 0 || x >= worldSize || y < 0 || y >= worldSize || z < 0 || z >= worldSize)
// 		{
// 			return true;
// 		}
//
// 		return false;
// 	}
// }
using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace.Utils;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace Game.Services.MapGenerator
{
	public class Generator : MonoBehaviour
	{
		[SerializeField]
		private MeshFilter _gameObject;

		private static FastNoise _noise = FastNoise.FromEncodedNodeTree(
			"EQACAAAAAAAgQBAAAAAAQBkAEwDD9Sg/DQAEAAAAAAAgQAkAAGZmJj8AAAAAPwEEAAAAAAAAAEBAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAM3MTD4AMzMzPwAAAAA/");

		private Vector3Int _noiseOffset = new(0, -WorldUtils.WORLD_SIZE / 2, 0);

		private float[,,] _noiseData;
		private float _noiseFreq = 0.005f;
		
		private readonly List<Vector2> _textures = new();
		private readonly List<Vector3> _vertices = new();
		private readonly List<int> _triangles = new();
		private int _faceCount = new();

		[SerializeField]
		private Button _button;

		public float _asd;
		
		Dictionary<Directions, Vector3Int> directionVectors = new Dictionary<Directions, Vector3Int>() {
			{Directions.Top, new Vector3Int(0, 1, 0)},
			{Directions.Bottom, new Vector3Int(0, -1, 0)},
			{Directions.Left, new Vector3Int(1, 0, 0)},
			{Directions.Right, new Vector3Int(-1, 0, 0)},
			{Directions.Forward, new Vector3Int(0, 0, 1)},
			{Directions.Backward, new Vector3Int(0, 0, -1)},
		};

		private void Start()
		{
			generateNoiseData();
			generate();
			_button.onClick.AddListener((() =>
			{
				_gameObject.mesh.Clear();
				generateNoiseData();
				generate();
			}));
		}

		void generateNoiseData()
		{
			_noiseData = new float[WorldUtils.WORLD_SIZE + 2, WorldUtils.HEIGHT + 2,
				WorldUtils.WORLD_SIZE + 2];
			
			for (int i = -1; i <= WorldUtils.WORLD_SIZE; i++)
			{
				for (int j = -1; j <= WorldUtils.WORLD_SIZE; j++)
				{
					for (int k = -1; k <= WorldUtils.HEIGHT; k++)
					{
						Vector3 noiseVec = new(i + 0, k, j + 0);
						noiseVec += _noiseOffset;
						noiseVec *= _noiseFreq;
						_noiseData[i + 1, k + 1, j + 1] =
							_noise.GenSingle3D(noiseVec.x, noiseVec.y, noiseVec.z, 0);
					}
				}
			}
		}

		VoxelType tovoxeltype(float n)
		{
			if (n >= 0.9)
			{
				return VoxelType.Rock;
			}
			
			if (n >= 0.8)
			{
				return VoxelType.Forest;
			}
			
			if (n >= 0.7)
			{
				return VoxelType.Plain;
			}
			
			if (n >= 0.6)
			{
				return VoxelType.Sand;
			}
			
			if (n >= 0.5)
			{
				return VoxelType.Water;
			}
			
				return VoxelType.GroundWater;
		}
		void generate()
		{
			int k = 0;

			for (int y = 0; y < WorldUtils.HEIGHT; y++)
			{
				for (int x = 0; x < WorldUtils.WORLD_SIZE; x++)
				{
					for (int z = 0; z < WorldUtils.WORLD_SIZE; z++)
					{
						var bpl = new Vector3Int(x, y, z);

						if (isBlock(bpl))
						{
							float n = _noiseData[bpl.x+1,bpl.y+1,bpl.z+1];
							var asd = tovoxeltype(n);
							CreateVoxelGeometry(bpl, asd, 1, 1);
						}
					}
				}
			}
			
			
			
			var mesh = new Mesh();
			mesh.indexFormat = IndexFormat.UInt32;
			var meshVertices = _vertices;
			var meshTriangles = _triangles;
			var meshUV = _textures;

			mesh.indexFormat = IndexFormat.UInt32;
			mesh.vertices = meshVertices.ToArray();
			mesh.triangles = meshTriangles.ToArray();
			mesh.uv = meshUV.ToArray();

			_gameObject.mesh = mesh;
		}

		IEnumerator Asd(Vector3Int asd)
		{
			yield return null;
			Instantiate(_gameObject, asd, Quaternion.identity);
		}
		
		bool isBlock(Vector3Int bpl)
		{
			try
			{
				float n = _noiseData[bpl.x+1,bpl.y+1,bpl.z+1];
				if (n < _asd)
				{
					return true;
				}

				return false;
			}
			catch (Exception e)
			{
				Debug.LogError($"{bpl}");
				throw;
			}
			
		}
		
		bool isBlockDir(Vector3Int bpl ,Directions dir)
		{
			return isBlock(bpl + directionVectors[dir]);
		}
		
		private void CreateVoxelGeometry(Vector3Int position, VoxelType voxelType, int voxelSize, int chunkId)
		{
			var x = position.x;
			var y = position.y;
			var z = position.z;

			// Front
				_vertices.Add(new Vector3(x, y, z + 1) * voxelSize);
				_vertices.Add(new Vector3(x + 1, y, z + 1) * voxelSize);
				_vertices.Add(new Vector3(x + 1, y + 1, z + 1) * voxelSize);
				_vertices.Add(new Vector3(x, y + 1, z + 1) * voxelSize);

				_triangles.Add(_faceCount * 4 + 0);
				_triangles.Add(_faceCount * 4 + 1);
				_triangles.Add(_faceCount * 4 + 2);
				_triangles.Add(_faceCount * 4 + 0);
				_triangles.Add(_faceCount * 4 + 2);
				_triangles.Add(_faceCount * 4 + 3);

				AddTexture(voxelType, chunkId);

				_faceCount += 1;

			// Back
				_vertices.Add(new Vector3(x + 1, y, z) * voxelSize);
				_vertices.Add(new Vector3(x, y, z) * voxelSize);
				_vertices.Add(new Vector3(x, y + 1, z) * voxelSize);
				_vertices.Add(new Vector3(x + 1, y + 1, z) * voxelSize);

				_triangles.Add(_faceCount * 4 + 0);
				_triangles.Add(_faceCount * 4 + 1);
				_triangles.Add(_faceCount * 4 + 2);
				_triangles.Add(_faceCount * 4 + 0);
				_triangles.Add(_faceCount * 4 + 2);
				_triangles.Add(_faceCount * 4 + 3);

				AddTexture(voxelType, chunkId);


				_faceCount += 1;

			// Top
				_vertices.Add(new Vector3(x, y + 1, z) * voxelSize);
				_vertices.Add(new Vector3(x + 1, y + 1, z) * voxelSize);
				_vertices.Add(new Vector3(x + 1, y + 1, z + 1) * voxelSize);
				_vertices.Add(new Vector3(x, y + 1, z + 1) * voxelSize);

				_triangles.Add(_faceCount * 4 + 0);
				_triangles.Add(_faceCount * 4 + 1);
				_triangles.Add(_faceCount * 4 + 2);
				_triangles.Add(_faceCount * 4 + 0);
				_triangles.Add(_faceCount * 4 + 2);
				_triangles.Add(_faceCount * 4 + 3);

				AddTexture(voxelType, chunkId);

				_faceCount += 1;

			// Bottom
				_vertices.Add(new Vector3(x + 1, y, z) * voxelSize);
				_vertices.Add(new Vector3(x, y, z) * voxelSize);
				_vertices.Add(new Vector3(x, y, z + 1) * voxelSize);
				_vertices.Add(new Vector3(x + 1, y, z + 1) * voxelSize);

				_triangles.Add(_faceCount * 4 + 0);
				_triangles.Add(_faceCount * 4 + 1);
				_triangles.Add(_faceCount * 4 + 2);
				_triangles.Add(_faceCount * 4 + 0);
				_triangles.Add(_faceCount * 4 + 2);
				_triangles.Add(_faceCount * 4 + 3);

				AddTexture(voxelType, chunkId);

				_faceCount += 1;

			// Left
				_vertices.Add(new Vector3(x, y, z) * voxelSize);
				_vertices.Add(new Vector3(x, y, z + 1) * voxelSize);
				_vertices.Add(new Vector3(x, y + 1, z + 1) * voxelSize);
				_vertices.Add(new Vector3(x, y + 1, z) * voxelSize);

				_triangles.Add(_faceCount * 4 + 0);
				_triangles.Add(_faceCount * 4 + 1);
				_triangles.Add(_faceCount * 4 + 2);
				_triangles.Add(_faceCount * 4 + 0);
				_triangles.Add(_faceCount * 4 + 2);
				_triangles.Add(_faceCount * 4 + 3);

				AddTexture(voxelType, chunkId);

				_faceCount += 1;

			// Right
				_vertices.Add(new Vector3(x + 1, y, z + 1) * voxelSize);
				_vertices.Add(new Vector3(x + 1, y, z) * voxelSize);
				_vertices.Add(new Vector3(x + 1, y + 1, z) * voxelSize);
				_vertices.Add(new Vector3(x + 1, y + 1, z + 1) * voxelSize);

				_triangles.Add(_faceCount * 4 + 0);
				_triangles.Add(_faceCount * 4 + 1);
				_triangles.Add(_faceCount * 4 + 2);
				_triangles.Add(_faceCount * 4 + 0);
				_triangles.Add(_faceCount * 4 + 2);
				_triangles.Add(_faceCount * 4 + 3);

				AddTexture(voxelType, chunkId);

				_faceCount += 1;
		}
		
		public enum Directions : byte
		{
			Top,
			Bottom,
			Left,
			Right,
			Forward,
			Backward
		}
		Directions[] listOfDirections = new Directions[]
		{
			Directions.Top, Directions.Bottom, Directions.Left, Directions.Right, Directions.Forward, Directions.Backward
		};

		private void AddTexture(VoxelType textureIndex, int chunkId)
		{
			switch (textureIndex)
			{
				case VoxelType.GroundWater:
					_textures.Add(VoxelUVUtils.groundA);
					_textures.Add(VoxelUVUtils.groundb);
					_textures.Add(VoxelUVUtils.groundc);
					_textures.Add(VoxelUVUtils.groundd);
					break;
				case VoxelType.Water:
					_textures.Add(VoxelUVUtils.waterA);
					_textures.Add(VoxelUVUtils.waterb);
					_textures.Add(VoxelUVUtils.waterc);
					_textures.Add(VoxelUVUtils.waterd);
					break;
				case VoxelType.Sand:
					_textures.Add(VoxelUVUtils.sandA);
					_textures.Add(VoxelUVUtils.sandb);
					_textures.Add(VoxelUVUtils.sandc);
					_textures.Add(VoxelUVUtils.sandd);
					break;
				case VoxelType.Plain:
					_textures.Add(VoxelUVUtils.grassA);
					_textures.Add(VoxelUVUtils.grassb);
					_textures.Add(VoxelUVUtils.grassc);
					_textures.Add(VoxelUVUtils.grassd);
					break;
				case VoxelType.Forest:
					_textures.Add(VoxelUVUtils.forestA);
					_textures.Add(VoxelUVUtils.forestb);
					_textures.Add(VoxelUVUtils.forestc);
					_textures.Add(VoxelUVUtils.forestd);
					break;
				case VoxelType.Rock:
					_textures.Add(VoxelUVUtils.stoneA);
					_textures.Add(VoxelUVUtils.stoneb);
					_textures.Add(VoxelUVUtils.stonec);
					_textures.Add(VoxelUVUtils.stoned);
					break;
				default:
					_textures.Add(VoxelUVUtils.groundA);
					_textures.Add(VoxelUVUtils.groundb);
					_textures.Add(VoxelUVUtils.groundc);
					_textures.Add(VoxelUVUtils.groundd);
					break;
			}
		}
	}
}
using System.Collections.Generic;
using DefaultNamespace.Utils;
using ECS.Components.Map;
using Game.Services.MapGenerator;
using Leopotam.EcsLite;
using Services.Map;
using UnityEngine;
using UnityEngine.Rendering;

namespace ECS.Systems
{
	public class UpdateChunksSystem : IEcsInitSystem, IEcsRunSystem
	{
		private readonly IMapService _mapService;
		private EcsWorld _world;
		private EcsPool<NeedUpdateChunkComponent> _poolNeedUpdateChunk;
		private EcsPool<ChunkViewCompoenent> _poolChunkView;
		private EcsPool<VoxelTypeComponent> _poolVoxelType;
		private EcsPool<VoxelPositionComponent> _poolVoxelPosition;
		private EcsPool<VoxelsInChunkComponent> _poolVoxelsInChunk;
		private EcsFilter _filterNeedUpdateChunk;

		private readonly Dictionary<int, List<Vector2>> _textures =
			new(WorldUtils.WORLD_SIZE * WorldUtils.WORLD_SIZE);
		private readonly Dictionary<int, List<Vector3>> _vertices =
			new(WorldUtils.WORLD_SIZE * WorldUtils.WORLD_SIZE);
		private readonly Dictionary<int, List<int>> _triangles = new(WorldUtils.WORLD_SIZE * WorldUtils.WORLD_SIZE);

		private readonly List<Vector2> _texturesBuffer = new(WorldUtils.WORLD_SIZE * WorldUtils.WORLD_SIZE);
		private readonly List<Vector3> _verticesBuffer = new(WorldUtils.WORLD_SIZE * WorldUtils.WORLD_SIZE);
		private readonly List<int> _triangleBuffer = new(WorldUtils.WORLD_SIZE * WorldUtils.WORLD_SIZE);

		private Dictionary<int, int> _faceCount = new();

		public UpdateChunksSystem(IMapService mapService)
			=> _mapService = mapService;

		public void Init(IEcsSystems systems)
		{
			_world = systems.GetWorld();
			_poolNeedUpdateChunk = _world.GetPool<NeedUpdateChunkComponent>();
			_poolChunkView = _world.GetPool<ChunkViewCompoenent>();
			_poolVoxelType = _world.GetPool<VoxelTypeComponent>();
			_poolVoxelPosition = _world.GetPool<VoxelPositionComponent>();
			_poolVoxelsInChunk = _world.GetPool<VoxelsInChunkComponent>();

			_filterNeedUpdateChunk = _world.Filter<NeedUpdateChunkComponent>()
				.Inc<ChunkViewCompoenent>()
				.End();
		}

		public void Run(IEcsSystems systems)
		{
			_vertices.Clear();
			_triangles.Clear();
			_textures.Clear();
			_faceCount.Clear();

			_verticesBuffer.Clear();
			_triangleBuffer.Clear();
			_texturesBuffer.Clear();

			foreach (var entity in _filterNeedUpdateChunk)
			{
				var isNeedUpdate = _poolNeedUpdateChunk.Get(entity).Value;
				if (!isNeedUpdate)
					continue;

				if (!_faceCount.TryGetValue(entity, out _))
					_faceCount.Add(entity, 0);

				var voxelEntities = _poolVoxelsInChunk.Get(entity).Value;
				foreach (var voxelEntity in voxelEntities)
				{
					var position = _poolVoxelPosition.Get(voxelEntity).Value;
					var voxelType = _poolVoxelType.Get(voxelEntity).Value;

					for (int y = 0; y < (int) voxelType; y++)
					{
						var x = position.x;
						var z = position.y;
						CreateVoxelGeometry(new Vector3Int(x, y, z), y.ToVoxelType(), entity);
					}
				}

				var chunkView = _poolChunkView.Get(entity).Value;
				Debug.LogError($"chunkView: {chunkView.gameObject.name}");
				var meshVertices = _vertices[entity].ToArray();
				var meshTriangles = _triangles[entity].ToArray();
				var meshUV = _textures[entity].ToArray();

				var mesh = new Mesh();
				mesh.indexFormat = IndexFormat.UInt32;
				mesh.vertices = meshVertices;
				mesh.triangles = meshTriangles;
				mesh.uv = meshUV;

				mesh.RecalculateNormals();
				mesh.Optimize();
				chunkView.MeshFilter.mesh = mesh;

				_poolNeedUpdateChunk.Get(entity).Value = false;
			}
		}

		private void CreateVoxelGeometry(Vector3Int position, VoxelType voxelType, int chunkId)
		{
			var x = position.x;
			var y = position.y;
			var z = position.z;

			// Front
			if (_mapService.IsTransparent(x, y, z + 1))
			{
				_vertices.AddOrCreateValue(chunkId, new Vector3(x, y, z + 1), _verticesBuffer);
				_vertices.AddOrCreateValue(chunkId, new Vector3(x + 1, y, z + 1), _verticesBuffer);
				_vertices.AddOrCreateValue(chunkId, new Vector3(x + 1, y + 1, z + 1), _verticesBuffer);
				_vertices.AddOrCreateValue(chunkId, new Vector3(x, y + 1, z + 1), _verticesBuffer);

				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4, _triangleBuffer);
				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4 + 1, _triangleBuffer);
				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4 + 2, _triangleBuffer);
				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4, _triangleBuffer);
				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4 + 2, _triangleBuffer);
				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4 + 3, _triangleBuffer);

				AddTexture(voxelType, chunkId);
				_faceCount[chunkId] += 1;
			}

			// Back
			if (_mapService.IsTransparent(x, y, z - 1))
			{
				_vertices.AddOrCreateValue(chunkId, new Vector3(x + 1, y, z), _verticesBuffer);
				_vertices.AddOrCreateValue(chunkId, new Vector3(x, y, z), _verticesBuffer);
				_vertices.AddOrCreateValue(chunkId, new Vector3(x, y + 1, z), _verticesBuffer);
				_vertices.AddOrCreateValue(chunkId, new Vector3(x + 1, y + 1, z), _verticesBuffer);

				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4, _triangleBuffer);
				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4 + 1, _triangleBuffer);
				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4 + 2, _triangleBuffer);
				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4, _triangleBuffer);
				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4 + 2, _triangleBuffer);
				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4 + 3, _triangleBuffer);

				AddTexture(voxelType, chunkId);
				_faceCount[chunkId] += 1;
			}

			// Top
			if (_mapService.IsTransparent(x, y + 1, z))
			{
				_vertices.AddOrCreateValue(chunkId, new Vector3(x, y + 1, z), _verticesBuffer);
				_vertices.AddOrCreateValue(chunkId, new Vector3(x + 1, y + 1, z), _verticesBuffer);
				_vertices.AddOrCreateValue(chunkId, new Vector3(x + 1, y + 1, z + 1), _verticesBuffer);
				_vertices.AddOrCreateValue(chunkId, new Vector3(x, y + 1, z + 1), _verticesBuffer);

				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4, _triangleBuffer);
				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4 + 1, _triangleBuffer);
				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4 + 2, _triangleBuffer);
				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4, _triangleBuffer);
				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4 + 2, _triangleBuffer);
				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4 + 3, _triangleBuffer);

				AddTexture(voxelType, chunkId);
				_faceCount[chunkId] += 1;
			}

			// Bottom
			if (_mapService.IsTransparent(x, y - 1, z))
			{
				_vertices.AddOrCreateValue(chunkId, new Vector3(x + 1, y, z), _verticesBuffer);
				_vertices.AddOrCreateValue(chunkId, new Vector3(x, y, z), _verticesBuffer);
				_vertices.AddOrCreateValue(chunkId, new Vector3(x, y, z + 1), _verticesBuffer);
				_vertices.AddOrCreateValue(chunkId, new Vector3(x + 1, y, z + 1), _verticesBuffer);

				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4, _triangleBuffer);
				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4 + 1, _triangleBuffer);
				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4 + 2, _triangleBuffer);
				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4, _triangleBuffer);
				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4 + 2, _triangleBuffer);
				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4 + 3, _triangleBuffer);

				AddTexture(voxelType, chunkId);
				_faceCount[chunkId] += 1;
			}

			// Left
			if (_mapService.IsTransparent(x - 1, y, z))
			{
				_vertices.AddOrCreateValue(chunkId, new Vector3(x, y, z), _verticesBuffer);
				_vertices.AddOrCreateValue(chunkId, new Vector3(x, y, z + 1), _verticesBuffer);
				_vertices.AddOrCreateValue(chunkId, new Vector3(x, y + 1, z + 1), _verticesBuffer);
				_vertices.AddOrCreateValue(chunkId, new Vector3(x, y + 1, z), _verticesBuffer);

				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4, _triangleBuffer);
				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4 + 1, _triangleBuffer);
				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4 + 2, _triangleBuffer);
				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4, _triangleBuffer);
				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4 + 2, _triangleBuffer);
				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4 + 3, _triangleBuffer);

				AddTexture(voxelType, chunkId);
				_faceCount[chunkId] += 1;
			}

			// Right
			if (_mapService.IsTransparent(x + 1, y, z))
			{
				_vertices.AddOrCreateValue(chunkId, new Vector3(x + 1, y, z + 1), _verticesBuffer);
				_vertices.AddOrCreateValue(chunkId, new Vector3(x + 1, y, z), _verticesBuffer);
				_vertices.AddOrCreateValue(chunkId, new Vector3(x + 1, y + 1, z), _verticesBuffer);
				_vertices.AddOrCreateValue(chunkId, new Vector3(x + 1, y + 1, z + 1), _verticesBuffer);

				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4, _triangleBuffer);
				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4 + 1, _triangleBuffer);
				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4 + 2, _triangleBuffer);
				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4, _triangleBuffer);
				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4 + 2, _triangleBuffer);
				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4 + 3, _triangleBuffer);

				AddTexture(voxelType, chunkId);
				_faceCount[chunkId] += 1;
			}
		}

		private void AddTexture(VoxelType textureIndex, int chunkId)
		{
			switch (textureIndex)
			{
				case VoxelType.GroundWater:
					_textures.AddOrCreateValue(chunkId, VoxelUVUtils.groundA, _texturesBuffer);
					_textures.AddOrCreateValue(chunkId, VoxelUVUtils.groundb, _texturesBuffer);
					_textures.AddOrCreateValue(chunkId, VoxelUVUtils.groundc, _texturesBuffer);
					_textures.AddOrCreateValue(chunkId, VoxelUVUtils.groundd, _texturesBuffer);
					break;
				case VoxelType.Water:
					_textures.AddOrCreateValue(chunkId, VoxelUVUtils.waterA, _texturesBuffer);
					_textures.AddOrCreateValue(chunkId, VoxelUVUtils.waterb, _texturesBuffer);
					_textures.AddOrCreateValue(chunkId, VoxelUVUtils.waterc, _texturesBuffer);
					_textures.AddOrCreateValue(chunkId, VoxelUVUtils.waterd, _texturesBuffer);
					break;
				case VoxelType.Sand:
					_textures.AddOrCreateValue(chunkId, VoxelUVUtils.sandA, _texturesBuffer);
					_textures.AddOrCreateValue(chunkId, VoxelUVUtils.sandb, _texturesBuffer);
					_textures.AddOrCreateValue(chunkId, VoxelUVUtils.sandc, _texturesBuffer);
					_textures.AddOrCreateValue(chunkId, VoxelUVUtils.sandd, _texturesBuffer);
					break;
				case VoxelType.Plain:
					_textures.AddOrCreateValue(chunkId, VoxelUVUtils.grassA, _texturesBuffer);
					_textures.AddOrCreateValue(chunkId, VoxelUVUtils.grassb, _texturesBuffer);
					_textures.AddOrCreateValue(chunkId, VoxelUVUtils.grassc, _texturesBuffer);
					_textures.AddOrCreateValue(chunkId, VoxelUVUtils.grassd, _texturesBuffer);
					break;
				case VoxelType.Forest:
					_textures.AddOrCreateValue(chunkId, VoxelUVUtils.forestA, _texturesBuffer);
					_textures.AddOrCreateValue(chunkId, VoxelUVUtils.forestb, _texturesBuffer);
					_textures.AddOrCreateValue(chunkId, VoxelUVUtils.forestc, _texturesBuffer);
					_textures.AddOrCreateValue(chunkId, VoxelUVUtils.forestd, _texturesBuffer);
					break;
				case VoxelType.Rock:
					_textures.AddOrCreateValue(chunkId, VoxelUVUtils.stoneA, _texturesBuffer);
					_textures.AddOrCreateValue(chunkId, VoxelUVUtils.stoneb, _texturesBuffer);
					_textures.AddOrCreateValue(chunkId, VoxelUVUtils.stonec, _texturesBuffer);
					_textures.AddOrCreateValue(chunkId, VoxelUVUtils.stoned, _texturesBuffer);
					break;
				default:
					_textures.AddOrCreateValue(chunkId, VoxelUVUtils.groundA, _texturesBuffer);
					_textures.AddOrCreateValue(chunkId, VoxelUVUtils.groundb, _texturesBuffer);
					_textures.AddOrCreateValue(chunkId, VoxelUVUtils.groundc, _texturesBuffer);
					_textures.AddOrCreateValue(chunkId, VoxelUVUtils.groundd, _texturesBuffer);
					break;
			}
		}
	}
}
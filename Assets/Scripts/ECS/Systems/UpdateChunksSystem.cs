using System;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace.Utils;
using ECS.Components.Map;
using Game.Services.MapGenerator;
using Leopotam.EcsLite;
using Services.Map;
using UnityEngine;
using UnityEngine.Profiling;
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

		private readonly Dictionary<int, List<Vector2>> _textures = new(WorldUtils.WORLD_SIZE * WorldUtils.WORLD_SIZE);
		private readonly Dictionary<int, List<Vector3>> _vertices = new(WorldUtils.WORLD_SIZE * WorldUtils.WORLD_SIZE);
		private readonly Dictionary<int, List<int>> _triangles = new(WorldUtils.WORLD_SIZE * WorldUtils.WORLD_SIZE);

		private Dictionary<int, int> _faceCount = new();

		private readonly Dictionary<int, List<Vector2>> _texturesBuffer = new();
		private readonly Dictionary<int, List<Vector3>> _verticesBuffer = new();
		private readonly Dictionary<int, List<int>> _trianglesBuffer = new();
		private int _lastBufferIndex;

		public UpdateChunksSystem(IMapService mapService,
			EcsWorld world)
		{
			_mapService = mapService;
			_world = world;

			var maxVoxelType = Enum.GetValues(typeof(VoxelType)).Cast<byte>().Max();
			for (int i = 0; i < WorldUtils.CHUNK_SIZE * WorldUtils.CHUNK_SIZE * maxVoxelType; i++)
			{
				_texturesBuffer.Add(i, new List<Vector2>());
				_verticesBuffer.Add(i, new List<Vector3>());
				_trianglesBuffer.Add(i, new List<int>());
			}
		}

		public void Init(IEcsSystems systems)
		{
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
			Profiler.BeginSample("UpdateChunksSystem");
			foreach (var entity in _filterNeedUpdateChunk)
			{
				_vertices.Clear();
				_triangles.Clear();
				_textures.Clear();
				_faceCount.Clear();
				
				for (int i = 0; i < _lastBufferIndex; i++)
				{
					_verticesBuffer[i].Clear();
					_trianglesBuffer[i].Clear();
					_texturesBuffer[i].Clear();
				}
				
				_lastBufferIndex = 0;

				ref var isNeedUpdate = ref _poolNeedUpdateChunk.Get(entity).Value;
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
				var mesh = chunkView.MeshFilter.mesh;
				mesh.Clear();
				mesh.indexFormat = IndexFormat.UInt16;
				
				var newVertices = _vertices[entity];
				var newTriangles = _triangles[entity];
				var newTextures = _textures[entity];

				mesh.SetVertices(newVertices);
				mesh.SetTriangles(newTriangles, 0);
				mesh.SetUVs(0, newTextures);
				isNeedUpdate = false;
			}

			Profiler.EndSample();
		}

		private void CreateVoxelGeometry(Vector3Int position, VoxelType voxelType, int chunkId)
		{
			Profiler.BeginSample("CreateVoxelGeometry");

			var x = position.x;
			var y = position.y;
			var z = position.z;

			var verticesBuffer = _verticesBuffer[_lastBufferIndex];
			var trianglesBuffer = _trianglesBuffer[_lastBufferIndex];
			var texturesBuffer = _texturesBuffer[_lastBufferIndex];
			_lastBufferIndex++;

			// Front
			if (_mapService.IsTransparent(x, y, z + 1))
			{
				_vertices.AddOrCreateValue(chunkId, new Vector3(x, y, z + 1), verticesBuffer);
				_vertices.AddOrCreateValue(chunkId, new Vector3(x + 1, y, z + 1), verticesBuffer);
				_vertices.AddOrCreateValue(chunkId, new Vector3(x + 1, y + 1, z + 1), verticesBuffer);
				_vertices.AddOrCreateValue(chunkId, new Vector3(x, y + 1, z + 1), verticesBuffer);

				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4, trianglesBuffer);
				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4 + 1, trianglesBuffer);
				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4 + 2, trianglesBuffer);
				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4, trianglesBuffer);
				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4 + 2, trianglesBuffer);
				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4 + 3, trianglesBuffer);

				AddTexture(voxelType, chunkId, texturesBuffer);
				_faceCount[chunkId] += 1;
			}

			// Back
			if (_mapService.IsTransparent(x, y, z - 1))
			{
				_vertices.AddOrCreateValue(chunkId, new Vector3(x + 1, y, z), verticesBuffer);
				_vertices.AddOrCreateValue(chunkId, new Vector3(x, y, z), verticesBuffer);
				_vertices.AddOrCreateValue(chunkId, new Vector3(x, y + 1, z), verticesBuffer);
				_vertices.AddOrCreateValue(chunkId, new Vector3(x + 1, y + 1, z), verticesBuffer);

				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4, trianglesBuffer);
				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4 + 1, trianglesBuffer);
				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4 + 2, trianglesBuffer);
				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4, trianglesBuffer);
				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4 + 2, trianglesBuffer);
				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4 + 3, trianglesBuffer);

				AddTexture(voxelType, chunkId, texturesBuffer);
				_faceCount[chunkId] += 1;
			}

			// Top
			if (_mapService.IsTransparent(x, y + 1, z))
			{
				_vertices.AddOrCreateValue(chunkId, new Vector3(x, y + 1, z), verticesBuffer);
				_vertices.AddOrCreateValue(chunkId, new Vector3(x + 1, y + 1, z), verticesBuffer);
				_vertices.AddOrCreateValue(chunkId, new Vector3(x + 1, y + 1, z + 1), verticesBuffer);
				_vertices.AddOrCreateValue(chunkId, new Vector3(x, y + 1, z + 1), verticesBuffer);

				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4, trianglesBuffer);
				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4 + 1, trianglesBuffer);
				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4 + 2, trianglesBuffer);
				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4, trianglesBuffer);
				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4 + 2, trianglesBuffer);
				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4 + 3, trianglesBuffer);

				AddTexture(voxelType, chunkId, texturesBuffer);
				_faceCount[chunkId] += 1;
			}

			// Bottom
			if (_mapService.IsTransparent(x, y - 1, z))
			{
				_vertices.AddOrCreateValue(chunkId, new Vector3(x + 1, y, z), verticesBuffer);
				_vertices.AddOrCreateValue(chunkId, new Vector3(x, y, z), verticesBuffer);
				_vertices.AddOrCreateValue(chunkId, new Vector3(x, y, z + 1), verticesBuffer);
				_vertices.AddOrCreateValue(chunkId, new Vector3(x + 1, y, z + 1), verticesBuffer);

				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4, trianglesBuffer);
				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4 + 1, trianglesBuffer);
				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4 + 2, trianglesBuffer);
				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4, trianglesBuffer);
				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4 + 2, trianglesBuffer);
				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4 + 3, trianglesBuffer);

				AddTexture(voxelType, chunkId, texturesBuffer);
				_faceCount[chunkId] += 1;
			}

			// Left
			if (_mapService.IsTransparent(x - 1, y, z))
			{
				_vertices.AddOrCreateValue(chunkId, new Vector3(x, y, z), verticesBuffer);
				_vertices.AddOrCreateValue(chunkId, new Vector3(x, y, z + 1), verticesBuffer);
				_vertices.AddOrCreateValue(chunkId, new Vector3(x, y + 1, z + 1), verticesBuffer);
				_vertices.AddOrCreateValue(chunkId, new Vector3(x, y + 1, z), verticesBuffer);

				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4, trianglesBuffer);
				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4 + 1, trianglesBuffer);
				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4 + 2, trianglesBuffer);
				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4, trianglesBuffer);
				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4 + 2, trianglesBuffer);
				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4 + 3, trianglesBuffer);

				AddTexture(voxelType, chunkId, texturesBuffer);
				_faceCount[chunkId] += 1;
			}

			// Right
			if (_mapService.IsTransparent(x + 1, y, z))
			{
				_vertices.AddOrCreateValue(chunkId, new Vector3(x + 1, y, z + 1), verticesBuffer);
				_vertices.AddOrCreateValue(chunkId, new Vector3(x + 1, y, z), verticesBuffer);
				_vertices.AddOrCreateValue(chunkId, new Vector3(x + 1, y + 1, z), verticesBuffer);
				_vertices.AddOrCreateValue(chunkId, new Vector3(x + 1, y + 1, z + 1), verticesBuffer);

				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4, trianglesBuffer);
				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4 + 1, trianglesBuffer);
				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4 + 2, trianglesBuffer);
				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4, trianglesBuffer);
				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4 + 2, trianglesBuffer);
				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4 + 3, trianglesBuffer);

				AddTexture(voxelType, chunkId, texturesBuffer);
				_faceCount[chunkId] += 1;
			}
			
			Profiler.EndSample();
		}

		private void AddTexture(VoxelType textureIndex, int chunkId, List<Vector2> texturesBuffer)
		{
			Profiler.BeginSample("AddTexture");

			switch (textureIndex)
			{
				case VoxelType.GroundWater:
					_textures.AddOrCreateValue(chunkId, VoxelUVUtils.groundA, texturesBuffer);
					_textures.AddOrCreateValue(chunkId, VoxelUVUtils.groundb, texturesBuffer);
					_textures.AddOrCreateValue(chunkId, VoxelUVUtils.groundc, texturesBuffer);
					_textures.AddOrCreateValue(chunkId, VoxelUVUtils.groundd, texturesBuffer);
					break;
				case VoxelType.Water:
					_textures.AddOrCreateValue(chunkId, VoxelUVUtils.waterA, texturesBuffer);
					_textures.AddOrCreateValue(chunkId, VoxelUVUtils.waterb, texturesBuffer);
					_textures.AddOrCreateValue(chunkId, VoxelUVUtils.waterc, texturesBuffer);
					_textures.AddOrCreateValue(chunkId, VoxelUVUtils.waterd, texturesBuffer);
					break;
				case VoxelType.Sand:
					_textures.AddOrCreateValue(chunkId, VoxelUVUtils.sandA, texturesBuffer);
					_textures.AddOrCreateValue(chunkId, VoxelUVUtils.sandb, texturesBuffer);
					_textures.AddOrCreateValue(chunkId, VoxelUVUtils.sandc, texturesBuffer);
					_textures.AddOrCreateValue(chunkId, VoxelUVUtils.sandd, texturesBuffer);
					break;
				case VoxelType.Plain:
					_textures.AddOrCreateValue(chunkId, VoxelUVUtils.grassA, texturesBuffer);
					_textures.AddOrCreateValue(chunkId, VoxelUVUtils.grassb, texturesBuffer);
					_textures.AddOrCreateValue(chunkId, VoxelUVUtils.grassc, texturesBuffer);
					_textures.AddOrCreateValue(chunkId, VoxelUVUtils.grassd, texturesBuffer);
					break;
				case VoxelType.Forest:
					_textures.AddOrCreateValue(chunkId, VoxelUVUtils.forestA, texturesBuffer);
					_textures.AddOrCreateValue(chunkId, VoxelUVUtils.forestb, texturesBuffer);
					_textures.AddOrCreateValue(chunkId, VoxelUVUtils.forestc, texturesBuffer);
					_textures.AddOrCreateValue(chunkId, VoxelUVUtils.forestd, texturesBuffer);
					break;
				case VoxelType.Rock:
					_textures.AddOrCreateValue(chunkId, VoxelUVUtils.stoneA, texturesBuffer);
					_textures.AddOrCreateValue(chunkId, VoxelUVUtils.stoneb, texturesBuffer);
					_textures.AddOrCreateValue(chunkId, VoxelUVUtils.stonec, texturesBuffer);
					_textures.AddOrCreateValue(chunkId, VoxelUVUtils.stoned, texturesBuffer);
					break;
				default:
					_textures.AddOrCreateValue(chunkId, VoxelUVUtils.groundA, texturesBuffer);
					_textures.AddOrCreateValue(chunkId, VoxelUVUtils.groundb, texturesBuffer);
					_textures.AddOrCreateValue(chunkId, VoxelUVUtils.groundc, texturesBuffer);
					_textures.AddOrCreateValue(chunkId, VoxelUVUtils.groundd, texturesBuffer);
					break;
			}
			
			Profiler.EndSample();
		}
	}
}
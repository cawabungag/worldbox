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
		private EcsPool<ChunkComponent> _poolChunkBound;
		private EcsPool<VoxelTypeComponent> _poolVoxelType;
		private EcsPool<VoxelPositionComponent> _poolVoxelPosition;
		private EcsPool<VoxelsInChunkComponent> _poolVoxelsInChunk;
		private EcsFilter _filterNeedUpdateChunk;

		private readonly Dictionary<Vector4Int, List<Vector2>> _textures =
			new(WorldUtils.WORLD_SIZE * WorldUtils.WORLD_SIZE);
		private readonly Dictionary<Vector4Int, List<Vector3>> _vertices =
			new(WorldUtils.WORLD_SIZE * WorldUtils.WORLD_SIZE);
		private readonly Dictionary<Vector4Int, List<int>> _triangles = new(WorldUtils.WORLD_SIZE * WorldUtils.WORLD_SIZE);

		private readonly List<Vector2> _texturesBuffer = new(WorldUtils.WORLD_SIZE * WorldUtils.WORLD_SIZE);
		private readonly List<Vector3> _verticesBuffer = new(WorldUtils.WORLD_SIZE * WorldUtils.WORLD_SIZE);
		private readonly List<int> _triangleBuffer = new(WorldUtils.WORLD_SIZE * WorldUtils.WORLD_SIZE);

		private Dictionary<Vector4Int, int> _faceCount = new();

		public UpdateChunksSystem(IMapService mapService)
			=> _mapService = mapService;

		public void Init(IEcsSystems systems)
		{
			_world = systems.GetWorld();
			_poolNeedUpdateChunk = _world.GetPool<NeedUpdateChunkComponent>();
			_poolChunkView = _world.GetPool<ChunkViewCompoenent>();
			_poolChunkBound = _world.GetPool<ChunkComponent>();
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

				var chunkBound = _poolChunkBound.Get(entity).Value;

				if (!_faceCount.TryGetValue(chunkBound, out _))
					_faceCount.Add(chunkBound, 0);

				var voxelEntities = _poolVoxelsInChunk.Get(entity).Value;
				foreach (var voxelEntity in voxelEntities)
				{
					var position = _poolVoxelPosition.Get(voxelEntity).Value;
					var voxelType = _poolVoxelType.Get(voxelEntity).Value;

					for (int y = 0; y < (int) voxelType; y++)
					{
						var x = position.x;
						var z = position.y;
						CreateVoxelGeometry(new Vector3Int(x, y, z), y.ToVoxelType(), chunkBound);
					}
				}

				_poolNeedUpdateChunk.Get(entity).Value = false;
			}

			foreach (var entity in _filterNeedUpdateChunk)
			{
				var chunkBound = _poolChunkBound.Get(entity).Value;
				if (!_faceCount.TryGetValue(chunkBound, out _))
					continue;

				var chunkView = _poolChunkView.Get(entity).Value;
				var chunk = _poolChunkBound.Get(entity).Value;

				var mesh = new Mesh();
				mesh.indexFormat = IndexFormat.UInt32;
				_vertices[chunk].CopyTo(mesh.vertices);
				_triangles[chunk].CopyTo(mesh.triangles);
				_textures[chunk].CopyTo(mesh.uv);

				mesh.RecalculateNormals();
				mesh.Optimize();
				var meshFilterMesh = chunkView.MeshFilter.mesh;
				meshFilterMesh.Clear();
				chunkView.MeshFilter.mesh = mesh;
			}
		}

		private void CreateVoxelGeometry(Vector3Int position, VoxelType voxelType, Vector4Int chunkBounds)
		{
			var x = position.x;
			var y = position.y;
			var z = position.z;

			// Front
			if (_mapService.IsTransparent(x, y, z + 1))
			{
				_vertices.AddOrCreateValue(chunkBounds, new Vector3(x, y, z + 1), _verticesBuffer);
				_vertices.AddOrCreateValue(chunkBounds, new Vector3(x + 1, y, z + 1), _verticesBuffer);
				_vertices.AddOrCreateValue(chunkBounds, new Vector3(x + 1, y + 1, z + 1), _verticesBuffer);
				_vertices.AddOrCreateValue(chunkBounds, new Vector3(x, y + 1, z + 1), _verticesBuffer);

				_triangles.AddOrCreateValue(chunkBounds, _faceCount[chunkBounds] * 4, _triangleBuffer);
				_triangles.AddOrCreateValue(chunkBounds, _faceCount[chunkBounds] * 4 + 1, _triangleBuffer);
				_triangles.AddOrCreateValue(chunkBounds, _faceCount[chunkBounds] * 4 + 2, _triangleBuffer);
				_triangles.AddOrCreateValue(chunkBounds, _faceCount[chunkBounds] * 4, _triangleBuffer);
				_triangles.AddOrCreateValue(chunkBounds, _faceCount[chunkBounds] * 4 + 2, _triangleBuffer);
				_triangles.AddOrCreateValue(chunkBounds, _faceCount[chunkBounds] * 4 + 3, _triangleBuffer);

				AddTexture(voxelType, chunkBounds);
				_faceCount[chunkBounds] += 1;
			}

			// Back
			if (_mapService.IsTransparent(x, y, z - 1))
			{
				_vertices.AddOrCreateValue(chunkBounds, new Vector3(x + 1, y, z), _verticesBuffer);
				_vertices.AddOrCreateValue(chunkBounds, new Vector3(x, y, z), _verticesBuffer);
				_vertices.AddOrCreateValue(chunkBounds, new Vector3(x, y + 1, z), _verticesBuffer);
				_vertices.AddOrCreateValue(chunkBounds, new Vector3(x + 1, y + 1, z), _verticesBuffer);

				_triangles.AddOrCreateValue(chunkBounds, _faceCount[chunkBounds] * 4, _triangleBuffer);
				_triangles.AddOrCreateValue(chunkBounds, _faceCount[chunkBounds] * 4 + 1, _triangleBuffer);
				_triangles.AddOrCreateValue(chunkBounds, _faceCount[chunkBounds] * 4 + 2, _triangleBuffer);
				_triangles.AddOrCreateValue(chunkBounds, _faceCount[chunkBounds] * 4, _triangleBuffer);
				_triangles.AddOrCreateValue(chunkBounds, _faceCount[chunkBounds] * 4 + 2, _triangleBuffer);
				_triangles.AddOrCreateValue(chunkBounds, _faceCount[chunkBounds] * 4 + 3, _triangleBuffer);

				AddTexture(voxelType, chunkBounds);
				_faceCount[chunkBounds] += 1;
			}

			// Top
			if (_mapService.IsTransparent(x, y + 1, z))
			{
				_vertices.AddOrCreateValue(chunkBounds, new Vector3(x, y + 1, z), _verticesBuffer);
				_vertices.AddOrCreateValue(chunkBounds, new Vector3(x + 1, y + 1, z), _verticesBuffer);
				_vertices.AddOrCreateValue(chunkBounds, new Vector3(x + 1, y + 1, z + 1), _verticesBuffer);
				_vertices.AddOrCreateValue(chunkBounds, new Vector3(x, y + 1, z + 1), _verticesBuffer);

				_triangles.AddOrCreateValue(chunkBounds, _faceCount[chunkBounds] * 4, _triangleBuffer);
				_triangles.AddOrCreateValue(chunkBounds, _faceCount[chunkBounds] * 4 + 1, _triangleBuffer);
				_triangles.AddOrCreateValue(chunkBounds, _faceCount[chunkBounds] * 4 + 2, _triangleBuffer);
				_triangles.AddOrCreateValue(chunkBounds, _faceCount[chunkBounds] * 4, _triangleBuffer);
				_triangles.AddOrCreateValue(chunkBounds, _faceCount[chunkBounds] * 4 + 2, _triangleBuffer);
				_triangles.AddOrCreateValue(chunkBounds, _faceCount[chunkBounds] * 4 + 3, _triangleBuffer);

				AddTexture(voxelType, chunkBounds);
				_faceCount[chunkBounds] += 1;
			}

			// Bottom
			if (_mapService.IsTransparent(x, y - 1, z))
			{
				_vertices.AddOrCreateValue(chunkBounds, new Vector3(x + 1, y, z), _verticesBuffer);
				_vertices.AddOrCreateValue(chunkBounds, new Vector3(x, y, z), _verticesBuffer);
				_vertices.AddOrCreateValue(chunkBounds, new Vector3(x, y, z + 1), _verticesBuffer);
				_vertices.AddOrCreateValue(chunkBounds, new Vector3(x + 1, y, z + 1), _verticesBuffer);

				_triangles.AddOrCreateValue(chunkBounds, _faceCount[chunkBounds] * 4, _triangleBuffer);
				_triangles.AddOrCreateValue(chunkBounds, _faceCount[chunkBounds] * 4 + 1, _triangleBuffer);
				_triangles.AddOrCreateValue(chunkBounds, _faceCount[chunkBounds] * 4 + 2, _triangleBuffer);
				_triangles.AddOrCreateValue(chunkBounds, _faceCount[chunkBounds] * 4, _triangleBuffer);
				_triangles.AddOrCreateValue(chunkBounds, _faceCount[chunkBounds] * 4 + 2, _triangleBuffer);
				_triangles.AddOrCreateValue(chunkBounds, _faceCount[chunkBounds] * 4 + 3, _triangleBuffer);

				AddTexture(voxelType, chunkBounds);
				_faceCount[chunkBounds] += 1;
			}

			// Left
			if (_mapService.IsTransparent(x - 1, y, z))
			{
				_vertices.AddOrCreateValue(chunkBounds, new Vector3(x, y, z), _verticesBuffer);
				_vertices.AddOrCreateValue(chunkBounds, new Vector3(x, y, z + 1), _verticesBuffer);
				_vertices.AddOrCreateValue(chunkBounds, new Vector3(x, y + 1, z + 1), _verticesBuffer);
				_vertices.AddOrCreateValue(chunkBounds, new Vector3(x, y + 1, z), _verticesBuffer);

				_triangles.AddOrCreateValue(chunkBounds, _faceCount[chunkBounds] * 4, _triangleBuffer);
				_triangles.AddOrCreateValue(chunkBounds, _faceCount[chunkBounds] * 4 + 1, _triangleBuffer);
				_triangles.AddOrCreateValue(chunkBounds, _faceCount[chunkBounds] * 4 + 2, _triangleBuffer);
				_triangles.AddOrCreateValue(chunkBounds, _faceCount[chunkBounds] * 4, _triangleBuffer);
				_triangles.AddOrCreateValue(chunkBounds, _faceCount[chunkBounds] * 4 + 2, _triangleBuffer);
				_triangles.AddOrCreateValue(chunkBounds, _faceCount[chunkBounds] * 4 + 3, _triangleBuffer);

				AddTexture(voxelType, chunkBounds);
				_faceCount[chunkBounds] += 1;
			}

			// Right
			if (_mapService.IsTransparent(x + 1, y, z))
			{
				_vertices.AddOrCreateValue(chunkBounds, new Vector3(x + 1, y, z + 1), _verticesBuffer);
				_vertices.AddOrCreateValue(chunkBounds, new Vector3(x + 1, y, z), _verticesBuffer);
				_vertices.AddOrCreateValue(chunkBounds, new Vector3(x + 1, y + 1, z), _verticesBuffer);
				_vertices.AddOrCreateValue(chunkBounds, new Vector3(x + 1, y + 1, z + 1), _verticesBuffer);

				_triangles.AddOrCreateValue(chunkBounds, _faceCount[chunkBounds] * 4, _triangleBuffer);
				_triangles.AddOrCreateValue(chunkBounds, _faceCount[chunkBounds] * 4 + 1, _triangleBuffer);
				_triangles.AddOrCreateValue(chunkBounds, _faceCount[chunkBounds] * 4 + 2, _triangleBuffer);
				_triangles.AddOrCreateValue(chunkBounds, _faceCount[chunkBounds] * 4, _triangleBuffer);
				_triangles.AddOrCreateValue(chunkBounds, _faceCount[chunkBounds] * 4 + 2, _triangleBuffer);
				_triangles.AddOrCreateValue(chunkBounds, _faceCount[chunkBounds] * 4 + 3, _triangleBuffer);

				AddTexture(voxelType, chunkBounds);
				_faceCount[chunkBounds] += 1;
			}
		}

		private void AddTexture(VoxelType textureIndex, Vector4Int chunkBounds)
		{
			switch (textureIndex)
			{
				case VoxelType.GroundWater:
					_textures.AddOrCreateValue(chunkBounds, VoxelUVUtils.groundA, _texturesBuffer);
					_textures.AddOrCreateValue(chunkBounds, VoxelUVUtils.groundb, _texturesBuffer);
					_textures.AddOrCreateValue(chunkBounds, VoxelUVUtils.groundc, _texturesBuffer);
					_textures.AddOrCreateValue(chunkBounds, VoxelUVUtils.groundd, _texturesBuffer);
					break;
				case VoxelType.Water:
					_textures.AddOrCreateValue(chunkBounds, VoxelUVUtils.waterA, _texturesBuffer);
					_textures.AddOrCreateValue(chunkBounds, VoxelUVUtils.waterb, _texturesBuffer);
					_textures.AddOrCreateValue(chunkBounds, VoxelUVUtils.waterc, _texturesBuffer);
					_textures.AddOrCreateValue(chunkBounds, VoxelUVUtils.waterd, _texturesBuffer);
					break;
				case VoxelType.Sand:
					_textures.AddOrCreateValue(chunkBounds, VoxelUVUtils.sandA, _texturesBuffer);
					_textures.AddOrCreateValue(chunkBounds, VoxelUVUtils.sandb, _texturesBuffer);
					_textures.AddOrCreateValue(chunkBounds, VoxelUVUtils.sandc, _texturesBuffer);
					_textures.AddOrCreateValue(chunkBounds, VoxelUVUtils.sandd, _texturesBuffer);
					break;
				case VoxelType.Plain:
					_textures.AddOrCreateValue(chunkBounds, VoxelUVUtils.grassA, _texturesBuffer);
					_textures.AddOrCreateValue(chunkBounds, VoxelUVUtils.grassb, _texturesBuffer);
					_textures.AddOrCreateValue(chunkBounds, VoxelUVUtils.grassc, _texturesBuffer);
					_textures.AddOrCreateValue(chunkBounds, VoxelUVUtils.grassd, _texturesBuffer);
					break;
				case VoxelType.Forest:
					_textures.AddOrCreateValue(chunkBounds, VoxelUVUtils.forestA, _texturesBuffer);
					_textures.AddOrCreateValue(chunkBounds, VoxelUVUtils.forestb, _texturesBuffer);
					_textures.AddOrCreateValue(chunkBounds, VoxelUVUtils.forestc, _texturesBuffer);
					_textures.AddOrCreateValue(chunkBounds, VoxelUVUtils.forestd, _texturesBuffer);
					break;
				case VoxelType.Rock:
					_textures.AddOrCreateValue(chunkBounds, VoxelUVUtils.stoneA, _texturesBuffer);
					_textures.AddOrCreateValue(chunkBounds, VoxelUVUtils.stoneb, _texturesBuffer);
					_textures.AddOrCreateValue(chunkBounds, VoxelUVUtils.stonec, _texturesBuffer);
					_textures.AddOrCreateValue(chunkBounds, VoxelUVUtils.stoned, _texturesBuffer);
					break;
				default:
					_textures.AddOrCreateValue(chunkBounds, VoxelUVUtils.groundA, _texturesBuffer);
					_textures.AddOrCreateValue(chunkBounds, VoxelUVUtils.groundb, _texturesBuffer);
					_textures.AddOrCreateValue(chunkBounds, VoxelUVUtils.groundc, _texturesBuffer);
					_textures.AddOrCreateValue(chunkBounds, VoxelUVUtils.groundd, _texturesBuffer);
					break;
			}
		}
	}
}
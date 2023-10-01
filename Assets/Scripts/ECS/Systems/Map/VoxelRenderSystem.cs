using System;
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
	public class VoxelRenderSystem : IEcsInitSystem
	{
		private readonly IMapService _mapService;

		private EcsWorld _world;
		private EcsFilter _voxelEntities;

		private readonly Dictionary<int, List<Vector2>> _textures = new();
		private readonly Dictionary<int, List<Vector3>> _vertices = new();
		private readonly Dictionary<int, List<int>> _triangles = new();
		private Dictionary<int, int> _faceCount = new();

		public VoxelRenderSystem(IMapService mapService)
		{
			_mapService = mapService;
		}

		public void Init(IEcsSystems systems)
		{
			var timestamp1 = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

			_world = systems.GetWorld();
			var poolChunkComponent = _world.GetPool<ChunkComponent>();
			var poolChunkViewComponent = _world.GetPool<ChunkViewCompoenent>();
			var poolChunkEntityComponent = _world.GetPool<ChunkEntityComponent>();
			var poolVoxelPostionComponent = _world.GetPool<VoxelPositionComponent>();
			var poolVoxelTypeCompoenent = _world.GetPool<VoxelTypeComponent>();

			_voxelEntities = _world.Filter<VoxelPositionComponent>().Inc<VoxelTypeComponent>().End();
			if (_voxelEntities.GetEntitiesCount() == 0)
				return;

			var chunksEntities = _world.Filter<ChunkComponent>().End();
			if (chunksEntities.GetEntitiesCount() == 0)
				return;

			foreach (var entity in chunksEntities)
			{
				_faceCount.Add(entity, 0);
			}

			foreach (var entity in _voxelEntities)
			{
				var chunkEntity = poolChunkEntityComponent.Get(entity).Value;
				var position = poolVoxelPostionComponent.Get(entity).Value;
				var voxelType = poolVoxelTypeCompoenent.Get(entity).Value;

				for (int y = 0; y < (int) voxelType; y++)
				{
					var x = position.x;
					var z = position.y;
					CreateVoxelGeometry(new Vector3Int(x, y, z), y.ToVoxelType(), 1, chunkEntity);
				}
			}

			foreach (var entity in chunksEntities)
			{
				var chunkView = poolChunkViewComponent.Get(entity).Value;
				
				var mesh = new Mesh();
				mesh.indexFormat = IndexFormat.UInt32;
				var meshVertices = _vertices[entity].ToArray();
				var meshTriangles = _triangles[entity].ToArray();
				var meshUV = _textures[entity].ToArray();

				mesh.indexFormat = IndexFormat.UInt32;
				mesh.vertices = meshVertices;
				mesh.triangles = meshTriangles;
				mesh.uv = meshUV;

				chunkView.MeshFilter.mesh = mesh;

				if (chunkView.TryGetComponent<MeshCollider>(out var collider))
				{
					collider.sharedMesh = mesh;
				}
				else
				{
					var meshCollider = chunkView.gameObject.AddComponent<MeshCollider>();
					meshCollider.sharedMesh = mesh;
				}
			}

			var timestamp2 = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
			Debug.Log($"PERFOMANCE: VoxelRenderSystem: {timestamp2 - timestamp1}");
		}

		private void CreateVoxelGeometry(Vector3Int position, VoxelType voxelType, int voxelSize, int chunkId)
		{
			var x = position.x;
			var y = position.y;
			var z = position.z;

			// Front
			if (_mapService.IsTransparent(x, y, z + 1))
			{
				_vertices.AddOrCreateValue(chunkId, new Vector3(x, y, z + 1) * voxelSize);
				_vertices.AddOrCreateValue(chunkId, new Vector3(x + 1, y, z + 1) * voxelSize);
				_vertices.AddOrCreateValue(chunkId, new Vector3(x + 1, y + 1, z + 1) * voxelSize);
				_vertices.AddOrCreateValue(chunkId, new Vector3(x, y + 1, z + 1) * voxelSize);

				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4 + 0);
				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4 + 1);
				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4 + 2);
				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4 + 0);
				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4 + 2);
				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4 + 3);

				AddTexture(voxelType, chunkId);

				_faceCount[chunkId] += 1;
			}

			// Back
			if (_mapService.IsTransparent(x, y, z - 1))
			{
				_vertices.AddOrCreateValue(chunkId, new Vector3(x + 1, y, z) * voxelSize);
				_vertices.AddOrCreateValue(chunkId, new Vector3(x, y, z) * voxelSize);
				_vertices.AddOrCreateValue(chunkId, new Vector3(x, y + 1, z) * voxelSize);
				_vertices.AddOrCreateValue(chunkId, new Vector3(x + 1, y + 1, z) * voxelSize);

				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4 + 0);
				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4 + 1);
				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4 + 2);
				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4 + 0);
				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4 + 2);
				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4 + 3);

				AddTexture(voxelType, chunkId);


				_faceCount[chunkId] += 1;
			}

			// Top
			if (_mapService.IsTransparent(x, y + 1, z))
			{
				_vertices.AddOrCreateValue(chunkId, new Vector3(x, y + 1, z) * voxelSize);
				_vertices.AddOrCreateValue(chunkId, new Vector3(x + 1, y + 1, z) * voxelSize);
				_vertices.AddOrCreateValue(chunkId, new Vector3(x + 1, y + 1, z + 1) * voxelSize);
				_vertices.AddOrCreateValue(chunkId, new Vector3(x, y + 1, z + 1) * voxelSize);

				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4 + 0);
				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4 + 1);
				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4 + 2);
				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4 + 0);
				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4 + 2);
				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4 + 3);

				AddTexture(voxelType, chunkId);

				_faceCount[chunkId] += 1;
			}

			// Bottom
			if (_mapService.IsTransparent(x, y - 1, z))
			{
				_vertices.AddOrCreateValue(chunkId, new Vector3(x + 1, y, z) * voxelSize);
				_vertices.AddOrCreateValue(chunkId, new Vector3(x, y, z) * voxelSize);
				_vertices.AddOrCreateValue(chunkId, new Vector3(x, y, z + 1) * voxelSize);
				_vertices.AddOrCreateValue(chunkId, new Vector3(x + 1, y, z + 1) * voxelSize);

				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4 + 0);
				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4 + 1);
				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4 + 2);
				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4 + 0);
				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4 + 2);
				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4 + 3);

				AddTexture(voxelType, chunkId);

				_faceCount[chunkId] += 1;
			}

			// Left
			if (_mapService.IsTransparent(x - 1, y, z))
			{
				_vertices.AddOrCreateValue(chunkId, new Vector3(x, y, z) * voxelSize);
				_vertices.AddOrCreateValue(chunkId, new Vector3(x, y, z + 1) * voxelSize);
				_vertices.AddOrCreateValue(chunkId, new Vector3(x, y + 1, z + 1) * voxelSize);
				_vertices.AddOrCreateValue(chunkId, new Vector3(x, y + 1, z) * voxelSize);

				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4 + 0);
				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4 + 1);
				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4 + 2);
				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4 + 0);
				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4 + 2);
				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4 + 3);

				AddTexture(voxelType, chunkId);

				_faceCount[chunkId] += 1;
			}

			// Right
			if (_mapService.IsTransparent(x + 1, y, z))
			{
				_vertices.AddOrCreateValue(chunkId, new Vector3(x + 1, y, z + 1) * voxelSize);
				_vertices.AddOrCreateValue(chunkId, new Vector3(x + 1, y, z) * voxelSize);
				_vertices.AddOrCreateValue(chunkId, new Vector3(x + 1, y + 1, z) * voxelSize);
				_vertices.AddOrCreateValue(chunkId, new Vector3(x + 1, y + 1, z + 1) * voxelSize);

				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4 + 0);
				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4 + 1);
				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4 + 2);
				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4 + 0);
				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4 + 2);
				_triangles.AddOrCreateValue(chunkId, _faceCount[chunkId] * 4 + 3);

				AddTexture(voxelType, chunkId);

				_faceCount[chunkId] += 1;
			}
		}

		private void AddTexture(VoxelType textureIndex, int chunkId)
		{
			switch (textureIndex)
			{
				case VoxelType.GroundWater:
					_textures.AddOrCreateValue(chunkId, VoxelUVUtils.groundA);
					_textures.AddOrCreateValue(chunkId, VoxelUVUtils.groundb);
					_textures.AddOrCreateValue(chunkId, VoxelUVUtils.groundc);
					_textures.AddOrCreateValue(chunkId, VoxelUVUtils.groundd);
					break;
				case VoxelType.Water:
					_textures.AddOrCreateValue(chunkId, VoxelUVUtils.waterA);
					_textures.AddOrCreateValue(chunkId, VoxelUVUtils.waterb);
					_textures.AddOrCreateValue(chunkId, VoxelUVUtils.waterc);
					_textures.AddOrCreateValue(chunkId, VoxelUVUtils.waterd);
					break;
				case VoxelType.Sand:
					_textures.AddOrCreateValue(chunkId, VoxelUVUtils.sandA);
					_textures.AddOrCreateValue(chunkId, VoxelUVUtils.sandb);
					_textures.AddOrCreateValue(chunkId, VoxelUVUtils.sandc);
					_textures.AddOrCreateValue(chunkId, VoxelUVUtils.sandd);
					break;
				case VoxelType.Plain:
					_textures.AddOrCreateValue(chunkId, VoxelUVUtils.grassA);
					_textures.AddOrCreateValue(chunkId, VoxelUVUtils.grassb);
					_textures.AddOrCreateValue(chunkId, VoxelUVUtils.grassc);
					_textures.AddOrCreateValue(chunkId, VoxelUVUtils.grassd);
					break;
				case VoxelType.Forest:
					_textures.AddOrCreateValue(chunkId, VoxelUVUtils.forestA);
					_textures.AddOrCreateValue(chunkId, VoxelUVUtils.forestb);
					_textures.AddOrCreateValue(chunkId, VoxelUVUtils.forestc);
					_textures.AddOrCreateValue(chunkId, VoxelUVUtils.forestd);
					break;
				case VoxelType.Rock:
					_textures.AddOrCreateValue(chunkId, VoxelUVUtils.stoneA);
					_textures.AddOrCreateValue(chunkId, VoxelUVUtils.stoneb);
					_textures.AddOrCreateValue(chunkId, VoxelUVUtils.stonec);
					_textures.AddOrCreateValue(chunkId, VoxelUVUtils.stoned);
					break;
				default:
					_textures.AddOrCreateValue(chunkId, VoxelUVUtils.groundA);
					_textures.AddOrCreateValue(chunkId, VoxelUVUtils.groundb);
					_textures.AddOrCreateValue(chunkId, VoxelUVUtils.groundc);
					_textures.AddOrCreateValue(chunkId, VoxelUVUtils.groundd);
					break;
			}
		}
	}
}
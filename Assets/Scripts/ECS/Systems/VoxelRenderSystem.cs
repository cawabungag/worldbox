using System;
using System.Collections.Generic;
using DefaultNamespace.Utils;
using ECS.Components;
using Game.Services.MapGenerator;
using Leopotam.EcsLite;
using Services.Map;
using UI;
using UnityEngine;
using UnityEngine.Rendering;

namespace ECS.Systems
{
	public class VoxelRenderSystem : IEcsInitSystem
	{
		private readonly IMapService _mapService;
		private readonly VoxelRenderCounter _counter;
		
		private readonly Dictionary<Vector4, List<Vector2>> _textures = new();
		private readonly Dictionary<Vector4, List<Vector3>> _vertices = new();
		private readonly Dictionary<Vector4, List<int>> _triangles = new();
		private Dictionary<Vector4, int> _faceCount = new();
		private EcsWorld _world;
		private EcsFilter _voxelEntities;

		public VoxelRenderSystem(IMapService mapService, VoxelRenderCounter counter)
		{
			_mapService = mapService;
			_counter = counter;
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
				var chunk = poolChunkComponent.Get(entity).Value;
				_faceCount.Add(chunk, 0);
			}

			var count = 0;
			foreach (var entity in _voxelEntities)
			{
				var chunkEntity = poolChunkEntityComponent.Get(entity).Value;
				var chunkBounds = poolChunkComponent.Get(chunkEntity).Value;
				var position = poolVoxelPostionComponent.Get(entity).Value;
				var voxelType = poolVoxelTypeCompoenent.Get(entity).Value;

				for (int y = 0; y < (int) voxelType; y++)
				{
					var x = position.x;
					var z = position.y;
					CreateVoxelGeometry(new Vector3Int(x, y, z), y.ToVoxelType(), 1, chunkBounds);
				}

				count++;
				var entitiesCount = (float)count/_voxelEntities.GetEntitiesCount();
				_counter.SetText($"{entitiesCount * 100}%");
			}
			

			foreach (var entity in chunksEntities)
			{
				var chunkView = poolChunkViewComponent.Get(entity).Value;
				var chunk = poolChunkComponent.Get(entity).Value;

				var meshVertices = _vertices[chunk].ToArray();
				var meshTriangles = _triangles[chunk].ToArray();
				var meshUV = _textures[chunk].ToArray();

				var mesh = new Mesh();
				mesh.indexFormat = IndexFormat.UInt32;
				mesh.vertices = meshVertices;
				mesh.triangles = meshTriangles;
				mesh.uv = meshUV;

				mesh.RecalculateNormals();
				mesh.Optimize();
				chunkView.MeshFilter.mesh = mesh;
			}

			var timestamp2 = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
			Debug.Log($"PERFOMANCE: VoxelRenderSystem: {timestamp2 - timestamp1}");
		}

		private void CreateVoxelGeometry(Vector3Int position, VoxelType voxelType, int voxelSize, Vector4 chunkBounds)
		{
			Debug.LogError($"CreateVoxelGeometry: {chunkBounds}");
			var x = position.x;
			var y = position.y;
			var z = position.z;

			// Front
			if (_mapService.IsTransparent(x, y, z + 1))
			{
				_vertices.AddOrCreateValue(chunkBounds, new Vector3(x, y, z + 1) * voxelSize);
				_vertices.AddOrCreateValue(chunkBounds, new Vector3(x + 1, y, z + 1) * voxelSize);
				_vertices.AddOrCreateValue(chunkBounds, new Vector3(x + 1, y + 1, z + 1) * voxelSize);
				_vertices.AddOrCreateValue(chunkBounds, new Vector3(x, y + 1, z + 1) * voxelSize);

				_triangles.AddOrCreateValue(chunkBounds, _faceCount[chunkBounds] * 4 + 0);
				_triangles.AddOrCreateValue(chunkBounds, _faceCount[chunkBounds] * 4 + 1);
				_triangles.AddOrCreateValue(chunkBounds, _faceCount[chunkBounds] * 4 + 2);
				_triangles.AddOrCreateValue(chunkBounds, _faceCount[chunkBounds] * 4 + 0);
				_triangles.AddOrCreateValue(chunkBounds, _faceCount[chunkBounds] * 4 + 2);
				_triangles.AddOrCreateValue(chunkBounds, _faceCount[chunkBounds] * 4 + 3);

				AddTexture(voxelType, chunkBounds);

				_faceCount[chunkBounds] += 1;
			}

			// Back
			if (_mapService.IsTransparent(x, y, z - 1))
			{
				_vertices.AddOrCreateValue(chunkBounds, new Vector3(x + 1, y, z) * voxelSize);
				_vertices.AddOrCreateValue(chunkBounds, new Vector3(x, y, z) * voxelSize);
				_vertices.AddOrCreateValue(chunkBounds, new Vector3(x, y + 1, z) * voxelSize);
				_vertices.AddOrCreateValue(chunkBounds, new Vector3(x + 1, y + 1, z) * voxelSize);

				_triangles.AddOrCreateValue(chunkBounds, _faceCount[chunkBounds] * 4 + 0);
				_triangles.AddOrCreateValue(chunkBounds, _faceCount[chunkBounds] * 4 + 1);
				_triangles.AddOrCreateValue(chunkBounds, _faceCount[chunkBounds] * 4 + 2);
				_triangles.AddOrCreateValue(chunkBounds, _faceCount[chunkBounds] * 4 + 0);
				_triangles.AddOrCreateValue(chunkBounds, _faceCount[chunkBounds] * 4 + 2);
				_triangles.AddOrCreateValue(chunkBounds, _faceCount[chunkBounds] * 4 + 3);

				AddTexture(voxelType, chunkBounds);


				_faceCount[chunkBounds] += 1;
			}

			// Top
			if (_mapService.IsTransparent(x, y + 1, z))
			{
				_vertices.AddOrCreateValue(chunkBounds, new Vector3(x, y + 1, z) * voxelSize);
				_vertices.AddOrCreateValue(chunkBounds, new Vector3(x + 1, y + 1, z) * voxelSize);
				_vertices.AddOrCreateValue(chunkBounds, new Vector3(x + 1, y + 1, z + 1) * voxelSize);
				_vertices.AddOrCreateValue(chunkBounds, new Vector3(x, y + 1, z + 1) * voxelSize);

				_triangles.AddOrCreateValue(chunkBounds, _faceCount[chunkBounds] * 4 + 0);
				_triangles.AddOrCreateValue(chunkBounds, _faceCount[chunkBounds] * 4 + 1);
				_triangles.AddOrCreateValue(chunkBounds, _faceCount[chunkBounds] * 4 + 2);
				_triangles.AddOrCreateValue(chunkBounds, _faceCount[chunkBounds] * 4 + 0);
				_triangles.AddOrCreateValue(chunkBounds, _faceCount[chunkBounds] * 4 + 2);
				_triangles.AddOrCreateValue(chunkBounds, _faceCount[chunkBounds] * 4 + 3);

				AddTexture(voxelType, chunkBounds);

				_faceCount[chunkBounds] += 1;
			}

			// Bottom
			if (_mapService.IsTransparent(x, y - 1, z))
			{
				_vertices.AddOrCreateValue(chunkBounds, new Vector3(x + 1, y, z) * voxelSize);
				_vertices.AddOrCreateValue(chunkBounds, new Vector3(x, y, z) * voxelSize);
				_vertices.AddOrCreateValue(chunkBounds, new Vector3(x, y, z + 1) * voxelSize);
				_vertices.AddOrCreateValue(chunkBounds, new Vector3(x + 1, y, z + 1) * voxelSize);

				_triangles.AddOrCreateValue(chunkBounds, _faceCount[chunkBounds] * 4 + 0);
				_triangles.AddOrCreateValue(chunkBounds, _faceCount[chunkBounds] * 4 + 1);
				_triangles.AddOrCreateValue(chunkBounds, _faceCount[chunkBounds] * 4 + 2);
				_triangles.AddOrCreateValue(chunkBounds, _faceCount[chunkBounds] * 4 + 0);
				_triangles.AddOrCreateValue(chunkBounds, _faceCount[chunkBounds] * 4 + 2);
				_triangles.AddOrCreateValue(chunkBounds, _faceCount[chunkBounds] * 4 + 3);

				AddTexture(voxelType, chunkBounds);

				_faceCount[chunkBounds] += 1;
			}

			// Left
			if (_mapService.IsTransparent(x - 1, y, z))
			{
				_vertices.AddOrCreateValue(chunkBounds, new Vector3(x, y, z) * voxelSize);
				_vertices.AddOrCreateValue(chunkBounds, new Vector3(x, y, z + 1) * voxelSize);
				_vertices.AddOrCreateValue(chunkBounds, new Vector3(x, y + 1, z + 1) * voxelSize);
				_vertices.AddOrCreateValue(chunkBounds, new Vector3(x, y + 1, z) * voxelSize);

				_triangles.AddOrCreateValue(chunkBounds, _faceCount[chunkBounds] * 4 + 0);
				_triangles.AddOrCreateValue(chunkBounds, _faceCount[chunkBounds] * 4 + 1);
				_triangles.AddOrCreateValue(chunkBounds, _faceCount[chunkBounds] * 4 + 2);
				_triangles.AddOrCreateValue(chunkBounds, _faceCount[chunkBounds] * 4 + 0);
				_triangles.AddOrCreateValue(chunkBounds, _faceCount[chunkBounds] * 4 + 2);
				_triangles.AddOrCreateValue(chunkBounds, _faceCount[chunkBounds] * 4 + 3);

				AddTexture(voxelType, chunkBounds);

				_faceCount[chunkBounds] += 1;
			}

			// Right
			if (_mapService.IsTransparent(x + 1, y, z))
			{
				_vertices.AddOrCreateValue(chunkBounds, new Vector3(x + 1, y, z + 1) * voxelSize);
				_vertices.AddOrCreateValue(chunkBounds, new Vector3(x + 1, y, z) * voxelSize);
				_vertices.AddOrCreateValue(chunkBounds, new Vector3(x + 1, y + 1, z) * voxelSize);
				_vertices.AddOrCreateValue(chunkBounds, new Vector3(x + 1, y + 1, z + 1) * voxelSize);

				_triangles.AddOrCreateValue(chunkBounds, _faceCount[chunkBounds] * 4 + 0);
				_triangles.AddOrCreateValue(chunkBounds, _faceCount[chunkBounds] * 4 + 1);
				_triangles.AddOrCreateValue(chunkBounds, _faceCount[chunkBounds] * 4 + 2);
				_triangles.AddOrCreateValue(chunkBounds, _faceCount[chunkBounds] * 4 + 0);
				_triangles.AddOrCreateValue(chunkBounds, _faceCount[chunkBounds] * 4 + 2);
				_triangles.AddOrCreateValue(chunkBounds, _faceCount[chunkBounds] * 4 + 3);

				AddTexture(voxelType, chunkBounds);

				_faceCount[chunkBounds] += 1;
			}
		}

		private void AddTexture(VoxelType textureIndex, Vector4 chunkBounds)
		{
			switch (textureIndex)
			{
				case VoxelType.GroundWater:
					_textures.AddOrCreateValue(chunkBounds, VoxelUVUtils.groundA);
					_textures.AddOrCreateValue(chunkBounds, VoxelUVUtils.groundb);
					_textures.AddOrCreateValue(chunkBounds, VoxelUVUtils.groundc);
					_textures.AddOrCreateValue(chunkBounds, VoxelUVUtils.groundd);
					break;
				case VoxelType.Water:
					_textures.AddOrCreateValue(chunkBounds, VoxelUVUtils.waterA);
					_textures.AddOrCreateValue(chunkBounds, VoxelUVUtils.waterb);
					_textures.AddOrCreateValue(chunkBounds, VoxelUVUtils.waterc);
					_textures.AddOrCreateValue(chunkBounds, VoxelUVUtils.waterd);
					break;
				case VoxelType.Sand:
					_textures.AddOrCreateValue(chunkBounds, VoxelUVUtils.sandA);
					_textures.AddOrCreateValue(chunkBounds, VoxelUVUtils.sandb);
					_textures.AddOrCreateValue(chunkBounds, VoxelUVUtils.sandc);
					_textures.AddOrCreateValue(chunkBounds, VoxelUVUtils.sandd);
					break;
				case VoxelType.Plain:
					_textures.AddOrCreateValue(chunkBounds, VoxelUVUtils.grassA);
					_textures.AddOrCreateValue(chunkBounds, VoxelUVUtils.grassb);
					_textures.AddOrCreateValue(chunkBounds, VoxelUVUtils.grassc);
					_textures.AddOrCreateValue(chunkBounds, VoxelUVUtils.grassd);
					break;
				case VoxelType.Forest:
					_textures.AddOrCreateValue(chunkBounds, VoxelUVUtils.forestA);
					_textures.AddOrCreateValue(chunkBounds, VoxelUVUtils.forestb);
					_textures.AddOrCreateValue(chunkBounds, VoxelUVUtils.forestc);
					_textures.AddOrCreateValue(chunkBounds, VoxelUVUtils.forestd);
					break;
				case VoxelType.Rock:
					_textures.AddOrCreateValue(chunkBounds, VoxelUVUtils.stoneA);
					_textures.AddOrCreateValue(chunkBounds, VoxelUVUtils.stoneb);
					_textures.AddOrCreateValue(chunkBounds, VoxelUVUtils.stonec);
					_textures.AddOrCreateValue(chunkBounds, VoxelUVUtils.stoned);
					break;
				default:
					_textures.AddOrCreateValue(chunkBounds, VoxelUVUtils.groundA);
					_textures.AddOrCreateValue(chunkBounds, VoxelUVUtils.groundb);
					_textures.AddOrCreateValue(chunkBounds, VoxelUVUtils.groundc);
					_textures.AddOrCreateValue(chunkBounds, VoxelUVUtils.groundd);
					break;
			}
		}
	}
}
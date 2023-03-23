using System;
using System.Collections.Generic;
using DefaultNamespace.Chunk;
using DefaultNamespace.Utils;
using ECS.Components;
using Game.Services.MapGenerator;
using Leopotam.EcsLite;
using UnityEngine;
using UnityEngine.Rendering;

namespace ECS.Systems
{
	public class VoxelRenderSystem : IEcsInitSystem
	{
		readonly ChunkView.Pool _poolChunks;
		private readonly List<Vector2> _textures = new();
		private readonly List<Vector3> _vertices = new();
		private readonly List<int> _triangles = new();
		private int _faceCount;
		private EcsWorld _world;
		private EcsFilter _ecsFilter;
		private EcsPool<VoxelPositionComponent> _voxelPositionPool;
		private EcsPool<VoxelTypeComponent> _voxelTypePool;

		public VoxelRenderSystem(ChunkView.Pool poolChunks)
		{
			_poolChunks = poolChunks;
		}

		public void Init(IEcsSystems systems)
		{
			var timestamp1 = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
			_world = systems.GetWorld();
			_voxelPositionPool = _world.GetPool<VoxelPositionComponent>();
			_voxelTypePool = _world.GetPool<VoxelTypeComponent>();
			var entities = _world.Filter<VoxelPositionComponent>()
				.Inc<VoxelTypeComponent>()
				.End();

			_ecsFilter = entities;
			if (_ecsFilter.GetEntitiesCount() == 0)
				return;

			foreach (var entity in _ecsFilter)
			{
				var position = _voxelPositionPool.Get(entity).Value;
				var voxel = _voxelTypePool.Get(entity).Value;
				CreateVoxelGeometry(position, voxel, 1);
			}
			
			var mesh = new Mesh();
			mesh.Clear();
			mesh.indexFormat = IndexFormat.UInt32;
			mesh.vertices = _vertices.ToArray();
			mesh.triangles = _triangles.ToArray();
			mesh.uv = _textures.ToArray();
			mesh.RecalculateNormals();
			mesh.Optimize();
			var chunk = _poolChunks.Spawn();
			chunk.MeshFilter.mesh = mesh;
			
			var timestamp2 = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
			Debug.Log($"VoxelRenderSystem: {timestamp2 - timestamp1}");
		}

		private void CreateVoxelGeometry(Vector3Int position, VoxelType voxelType, int voxelSize)
		{
			var x = position.x;
			var y = position.y;
			var z = position.z;

			// Front
			if (IsTransparent(x, y, z + 1))
			{
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

				AddTexture(voxelType);

				_faceCount++;
			}

			// Back
			if (IsTransparent(x, y, z - 1))
			{
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

				AddTexture(voxelType);

				_faceCount++;
			}

			// Top
			if (IsTransparent(x, y + 1, z))
			{
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

				AddTexture(voxelType);

				_faceCount++;
			}

			// Bottom
			if (IsTransparent(x, y - 1, z))
			{
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

				AddTexture(voxelType);

				_faceCount++;
			}

			// Left
			if (IsTransparent(x - 1, y, z))
			{
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

				AddTexture(voxelType);

				_faceCount++;
			}

			// Right
			if (IsTransparent(x + 1, y, z))
			{
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

				AddTexture(voxelType);

				_faceCount++;
			}
		}

		void AddTexture(VoxelType textureIndex)
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
			}
		}

		bool IsTransparent(int x, int y, int z)
		{
			return true;
		}
	}
}
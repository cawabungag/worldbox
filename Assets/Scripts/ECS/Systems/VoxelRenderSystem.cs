using System;
using System.Collections.Generic;
using DefaultNamespace;
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
		private readonly WorldRoot worldRoot;
		private readonly List<Vector2> textures = new();
		private readonly List<Vector3> vertices = new();
		private readonly List<int> triangles = new();
		private int faceCount = 0;

		public VoxelRenderSystem(WorldRoot worldRoot)
		{
			this.worldRoot = worldRoot;
		}

		public void Init(IEcsSystems systems)
		{
			var timestamp1 = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
			var world = systems.GetWorld();
			var voxelPositionPool = world.GetPool<VoxelPositionComponent>();
			var voxelTypePool = world.GetPool<VoxelTypeComponent>();
			var entities = world.Filter<VoxelPositionComponent>()
				.Inc<VoxelTypeComponent>()
				.End();

			if (entities.GetEntitiesCount() == 0)
				return;

			var mesh = new Mesh();
			foreach (var entity in entities)
			{
				var position = voxelPositionPool.Get(entity).Value;
				var voxel = voxelTypePool.Get(entity).Value;
				CreateVoxelGeometry(position, voxel, 1);
			}
			
			mesh.Clear();
			mesh.indexFormat = IndexFormat.UInt32;
			mesh.vertices = vertices.ToArray();
			mesh.triangles = triangles.ToArray();
			mesh.uv = textures.ToArray();
			mesh.RecalculateNormals();
			mesh.Optimize();
			worldRoot.MeshFilter.mesh = mesh;
			
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
				vertices.Add(new Vector3(x, y, z + 1) * voxelSize);
				vertices.Add(new Vector3(x + 1, y, z + 1) * voxelSize);
				vertices.Add(new Vector3(x + 1, y + 1, z + 1) * voxelSize);
				vertices.Add(new Vector3(x, y + 1, z + 1) * voxelSize);

				triangles.Add(faceCount * 4 + 0);
				triangles.Add(faceCount * 4 + 1);
				triangles.Add(faceCount * 4 + 2);
				triangles.Add(faceCount * 4 + 0);
				triangles.Add(faceCount * 4 + 2);
				triangles.Add(faceCount * 4 + 3);

				AddTexture(voxelType);

				faceCount++;
			}

			// Back
			if (IsTransparent(x, y, z - 1))
			{
				vertices.Add(new Vector3(x + 1, y, z) * voxelSize);
				vertices.Add(new Vector3(x, y, z) * voxelSize);
				vertices.Add(new Vector3(x, y + 1, z) * voxelSize);
				vertices.Add(new Vector3(x + 1, y + 1, z) * voxelSize);

				triangles.Add(faceCount * 4 + 0);
				triangles.Add(faceCount * 4 + 1);
				triangles.Add(faceCount * 4 + 2);
				triangles.Add(faceCount * 4 + 0);
				triangles.Add(faceCount * 4 + 2);
				triangles.Add(faceCount * 4 + 3);

				AddTexture(voxelType);

				faceCount++;
			}

			// Top
			if (IsTransparent(x, y + 1, z))
			{
				vertices.Add(new Vector3(x, y + 1, z) * voxelSize);
				vertices.Add(new Vector3(x + 1, y + 1, z) * voxelSize);
				vertices.Add(new Vector3(x + 1, y + 1, z + 1) * voxelSize);
				vertices.Add(new Vector3(x, y + 1, z + 1) * voxelSize);

				triangles.Add(faceCount * 4 + 0);
				triangles.Add(faceCount * 4 + 1);
				triangles.Add(faceCount * 4 + 2);
				triangles.Add(faceCount * 4 + 0);
				triangles.Add(faceCount * 4 + 2);
				triangles.Add(faceCount * 4 + 3);

				AddTexture(voxelType);

				faceCount++;
			}

			// Bottom
			if (IsTransparent(x, y - 1, z))
			{
				vertices.Add(new Vector3(x + 1, y, z) * voxelSize);
				vertices.Add(new Vector3(x, y, z) * voxelSize);
				vertices.Add(new Vector3(x, y, z + 1) * voxelSize);
				vertices.Add(new Vector3(x + 1, y, z + 1) * voxelSize);

				triangles.Add(faceCount * 4 + 0);
				triangles.Add(faceCount * 4 + 1);
				triangles.Add(faceCount * 4 + 2);
				triangles.Add(faceCount * 4 + 0);
				triangles.Add(faceCount * 4 + 2);
				triangles.Add(faceCount * 4 + 3);

				AddTexture(voxelType);

				faceCount++;
			}

			// Left
			if (IsTransparent(x - 1, y, z))
			{
				vertices.Add(new Vector3(x, y, z) * voxelSize);
				vertices.Add(new Vector3(x, y, z + 1) * voxelSize);
				vertices.Add(new Vector3(x, y + 1, z + 1) * voxelSize);
				vertices.Add(new Vector3(x, y + 1, z) * voxelSize);

				triangles.Add(faceCount * 4 + 0);
				triangles.Add(faceCount * 4 + 1);
				triangles.Add(faceCount * 4 + 2);
				triangles.Add(faceCount * 4 + 0);
				triangles.Add(faceCount * 4 + 2);
				triangles.Add(faceCount * 4 + 3);

				AddTexture(voxelType);

				faceCount++;
			}

			// Right
			if (IsTransparent(x + 1, y, z))
			{
				vertices.Add(new Vector3(x + 1, y, z + 1) * voxelSize);
				vertices.Add(new Vector3(x + 1, y, z) * voxelSize);
				vertices.Add(new Vector3(x + 1, y + 1, z) * voxelSize);
				vertices.Add(new Vector3(x + 1, y + 1, z + 1) * voxelSize);

				triangles.Add(faceCount * 4 + 0);
				triangles.Add(faceCount * 4 + 1);
				triangles.Add(faceCount * 4 + 2);
				triangles.Add(faceCount * 4 + 0);
				triangles.Add(faceCount * 4 + 2);
				triangles.Add(faceCount * 4 + 3);

				AddTexture(voxelType);

				faceCount++;
			}
		}

		void AddTexture(VoxelType textureIndex)
		{
			switch (textureIndex)
			{
				case VoxelType.GroundWater:
					textures.Add(VoxelUVUtils.groundA);
					textures.Add(VoxelUVUtils.groundb);
					textures.Add(VoxelUVUtils.groundc);
					textures.Add(VoxelUVUtils.groundd);
					break;
				case VoxelType.Water:
					textures.Add(VoxelUVUtils.waterA);
					textures.Add(VoxelUVUtils.waterb);
					textures.Add(VoxelUVUtils.waterc);
					textures.Add(VoxelUVUtils.waterd);
					break;
				case VoxelType.Sand:
					textures.Add(VoxelUVUtils.sandA);
					textures.Add(VoxelUVUtils.sandb);
					textures.Add(VoxelUVUtils.sandc);
					textures.Add(VoxelUVUtils.sandd);
					break;
				case VoxelType.Plain:
					textures.Add(VoxelUVUtils.grassA);
					textures.Add(VoxelUVUtils.grassb);
					textures.Add(VoxelUVUtils.grassc);
					textures.Add(VoxelUVUtils.grassd);
					break;
				case VoxelType.Forest:
					textures.Add(VoxelUVUtils.forestA);
					textures.Add(VoxelUVUtils.forestb);
					textures.Add(VoxelUVUtils.forestc);
					textures.Add(VoxelUVUtils.forestd);
					break;
				case VoxelType.Rock:
					textures.Add(VoxelUVUtils.stoneA);
					textures.Add(VoxelUVUtils.stoneb);
					textures.Add(VoxelUVUtils.stonec);
					textures.Add(VoxelUVUtils.stoned);
					break;
			}
		}

		bool IsTransparent(int x, int y, int z)
		{
			// if (x < 0 || x >= worldSize || y < 0 || y >= worldSize || z < 0 || z >= worldSize)
			{
				return true;
			}

			return false;
		}
	}
}
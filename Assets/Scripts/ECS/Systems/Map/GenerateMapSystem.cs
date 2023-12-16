using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using DefaultNamespace.Systems.Save;
using DefaultNamespace.Utils;
using ECS.Components.Map;
using Game.Services.MapGenerator;
using Leopotam.EcsLite;
using UnityEngine;

namespace ECS.Systems
{
	public class GenerateMapSystem : IEcsInitSystem
	{
		private readonly IMapGenerator _mapGenerator;

		public GenerateMapSystem(IMapGenerator mapGenerator)
			=> _mapGenerator = mapGenerator;

		public void Init(IEcsSystems systems)
		{
			var world = systems.GetWorld();
			var voxelPositionPool = world.GetPool<VoxelPositionComponent>();
			var voxelTypePool = world.GetPool<VoxelTypeComponent>();
			
			var halfWidth = WorldUtils.WORLD_SIZE / 2;
			var halfHeight = WorldUtils.WORLD_SIZE / 2;
			var mapGraph = new GridGraph<MapNode>(WorldUtils.WORLD_SIZE, WorldUtils.WORLD_SIZE);
			
			if (File.Exists(Application.persistentDataPath + "/save.bin"))
			{
				var formatter = new BinaryFormatter();
				Stream readStream = new FileStream(Application.persistentDataPath + "/save.bin", FileMode.Open, FileAccess.Read, FileShare.Read);
				var saveData = (SaveData) formatter.Deserialize(readStream);
				var voxelsSaveData = saveData.VoxelsSaveData;
				foreach (var componentData in voxelsSaveData)
				{
					var voxelEntity = world.NewEntity();
					var cellPosiiton = new Vector2Int(componentData.Key.X, componentData.Key.Y);
					voxelPositionPool.Add(voxelEntity).Value = cellPosiiton;
					voxelTypePool.Add(voxelEntity).Value = componentData.Value;
					var mapNode = new MapNode(cellPosiiton, voxelEntity);
					mapGraph.SetEntity(cellPosiiton.x + halfWidth, cellPosiiton.y + halfHeight, mapNode);
				}
				
				readStream.Close();
				return;
			}

			var voxels = _mapGenerator.GenerateGround(WorldUtils.WORLD_SIZE, WorldUtils.WORLD_SIZE);

			for (var i = 0; i < WorldUtils.WORLD_SIZE; i++)
			{
				for (var j = 0; j < WorldUtils.WORLD_SIZE; j++)
				{
					var x = j - halfWidth;
					var z = i - halfHeight;
					var tileType = voxels[WorldUtils.WORLD_SIZE * i + j];

					var voxelEntity = world.NewEntity();
					var cellPosiiton = new Vector2Int(x, z);
					voxelPositionPool.Add(voxelEntity).Value = cellPosiiton;
					voxelTypePool.Add(voxelEntity).Value = tileType;
					var mapNode = new MapNode(cellPosiiton, voxelEntity);
					mapGraph.SetEntity(j, i, mapNode);
				}
			}

			var mapGraphEntity = world.NewEntity();
			world.GetPool<MapGraphComponent>().Add(mapGraphEntity).Value = mapGraph;
		}
	}
}
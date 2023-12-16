using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Db.Plant;
using DefaultNamespace.Components.Plant;
using DefaultNamespace.Components.Weather;
using DefaultNamespace.Systems.Save;
using DefaultNamespace.Utils;
using ECS.Components.Map;
using Leopotam.EcsLite;
using UnityEngine;

namespace ECS.Systems
{
	public class GeneratePlantSystem : IEcsInitSystem
	{
		private readonly PlantsData _plantsData;

		public GeneratePlantSystem(PlantsData plantsData)
			=> _plantsData = plantsData;

		public void Init(IEcsSystems systems)
		{
			var world = systems.GetWorld();
			var voxelTypePool = world.GetPool<VoxelTypeComponent>();
			var gridGraph = world.GetUnique<MapGraphComponent>().Value;
			var weatherGraph = world.GetUnique<MapWeatherComponent>().Value;
			var poolVoxelPositionComponent = world.GetPool<VoxelPositionComponent>();
			var poolPlantTypeComponent = world.GetPool<PlantTypeComponent>();
			var poolPlantPoolIndexComponent = world.GetPool<PlantPoolIndexComponent>();
			var poolPlantPositionComponent = world.GetPool<PlantPositionComponent>();
			
			if (File.Exists(Application.persistentDataPath + "/save.bin"))
			{
				var formatter = new BinaryFormatter();
				Stream readStream = new FileStream(Application.persistentDataPath + "/save.bin", FileMode.Open, FileAccess.Read, FileShare.Read);
				var saveData = (SaveData) formatter.Deserialize(readStream);
				var plantSaveData = saveData.PlantsaveData;
				foreach (var componentData in plantSaveData)
				{
					var newPlantEntity = world.NewEntity();
					var plantsData = _plantsData.GetPlant(componentData.Value);
					var posiiton = componentData.Key;
					poolPlantPositionComponent.Add(newPlantEntity).Value = new Vector3Int(posiiton.X, posiiton.Y, posiiton.Z);
					poolPlantTypeComponent.Add(newPlantEntity).Value = plantsData.Type;
					poolPlantPoolIndexComponent.Add(newPlantEntity).Value = plantsData.PoolIndex;
				}
				
				readStream.Close();
				return;
			}

			for (var i = 0; i < WorldUtils.WORLD_SIZE; i++)
			{
				for (var j = 0; j < WorldUtils.WORLD_SIZE; j++)
				{
					var mapEntity = gridGraph.GetValue(i, j).Entity;
					var voxelType = voxelTypePool.Get(mapEntity).Value;
					var temperature = weatherGraph.GetValue(i, j);
					var plantsData = _plantsData.GetPlantsData();

					foreach (var plantData in plantsData)
					{
						foreach (var type in plantData.VoxelType)
						{
							if (type != voxelType)
								continue;

							if (plantData.RequiredTemperature.x > temperature)
								continue;

							if (plantData.RequiredTemperature.y < temperature)
								continue;

							var position = poolVoxelPositionComponent.Get(mapEntity).Value;
							var plantPosition =
								new Vector3Int(position.x, (int) voxelType, position.y);
							var newPlantEntity = world.NewEntity();
							poolPlantPositionComponent.Add(newPlantEntity).Value = plantPosition;
							poolPlantTypeComponent.Add(newPlantEntity).Value = plantData.Type;
							poolPlantPoolIndexComponent.Add(newPlantEntity).Value =
								plantData.PoolIndex;
							break;
						}
					}
				}
			}
		}
	}
}
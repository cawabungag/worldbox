using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using DefaultNamespace.Components.Plant;
using ECS.Components.Map;
using Leopotam.EcsLite;
using UnityEngine;

namespace DefaultNamespace.Systems.Save
{
	public class SaveWorldSystem : IEcsInitSystem, IEcsRunSystem
	{
		private EcsFilter _filterVoxels;
		private EcsFilter _filterPlants;
		
		private EcsPool<VoxelTypeComponent> _voxelTypePool;
		private EcsPool<VoxelPositionComponent> _voxelPositionPool;
		private EcsPool<PlantTypeComponent> _plantTypePool;
		private EcsPool<PlantPositionComponent> _plantPositionPool;
		
		private readonly string _savePath = Application.persistentDataPath + "/save.bin";
		private IFormatter _formatter;
		private float _lastTimeSave;
		private const float DURATIONS_BETWEEN_SAVE = 30f;


		public void Init(IEcsSystems systems)
		{
			var world = systems.GetWorld();
			_filterVoxels = world.Filter<VoxelTypeComponent>().Inc<VoxelPositionComponent>().End();
			_filterPlants = world.Filter<PlantTypeComponent>().Inc<PlantPositionComponent>().End();

			_voxelTypePool = world.GetPool<VoxelTypeComponent>();
			_voxelPositionPool = world.GetPool<VoxelPositionComponent>();
			_plantTypePool = world.GetPool<PlantTypeComponent>();
			_plantPositionPool = world.GetPool<PlantPositionComponent>();
		}

		public void Run(IEcsSystems systems)
		{
			if (Time.realtimeSinceStartup < DURATIONS_BETWEEN_SAVE || Time.realtimeSinceStartup - _lastTimeSave < DURATIONS_BETWEEN_SAVE)
				return;
			
			var stamp1 = Time.realtimeSinceStartup;
			var saveData = new SaveData();
			foreach (var voxelEntity in _filterVoxels)
			{
				var voxelPos = _voxelPositionPool.Get(voxelEntity).Value.AsSerializable();
				var voxelType = _voxelTypePool.Get(voxelEntity).Value;
				saveData.VoxelsSaveData.Add(voxelPos, voxelType);
			}
			
			foreach (var plantEntity in _filterPlants)
			{
				var plantPos = _plantPositionPool.Get(plantEntity).Value.AsSerializable();
				var plantType = _plantTypePool.Get(plantEntity).Value;
				saveData.PlantsaveData.Add(plantPos, plantType);
			}

			if (saveData.IsEmpty)
				return;
			
			_formatter = new BinaryFormatter();
			Stream stream = new FileStream(_savePath, FileMode.Create, FileAccess.Write, FileShare.None);
			_formatter.Serialize(stream, saveData);
			stream.Close();
			var stamp2 = Time.realtimeSinceStartup;
			_lastTimeSave = Time.realtimeSinceStartup;
			Debug.Log($"SaveWorldSystem : {stamp2 - stamp1}");
		}
	}
}
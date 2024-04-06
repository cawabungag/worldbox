using System;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using DefaultNamespace.Components.Plant;
using DefaultNamespace.Db;
using ECS.Components.Map;
using Leopotam.EcsLite;
using UnityEngine;

namespace DefaultNamespace.Systems.Save
{
	public class SaveWorldSystem : IEcsInitSystem, IEcsRunSystem
	{
		private readonly ISaveModel _saveModel;
		private readonly GameSetting _gameSetting;

		private EcsFilter _filterVoxels;
		private EcsFilter _filterPlants;
		
		private EcsPool<VoxelTypeComponent> _voxelTypePool;
		private EcsPool<VoxelPositionComponent> _voxelPositionPool;
		private EcsPool<PlantTypeComponent> _plantTypePool;
		private EcsPool<PlantPositionComponent> _plantPositionPool;
		
		private IFormatter _formatter;
		private float _lastTimeSave;
		private const float DURATIONS_BETWEEN_SAVE = 10f;

		public SaveWorldSystem(ISaveModel saveModel, GameSetting gameSetting)
		{
			_saveModel = saveModel;
			_gameSetting = gameSetting;
		}

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
			if (!_gameSetting.IsSaveEnable)
				return;
            
			if (Time.realtimeSinceStartup < DURATIONS_BETWEEN_SAVE || Time.realtimeSinceStartup - _lastTimeSave < DURATIONS_BETWEEN_SAVE)
				return;
			
			var saveData = new SaveData();
			foreach (var voxelEntity in _filterVoxels)
			{
				var voxelPos = _voxelPositionPool.Get(voxelEntity).Value.AsSerializable();
				var voxelType = _voxelTypePool.Get(voxelEntity).Value;
				saveData.VoxelsSaveData.Add(new VoxelSaveData(voxelPos, voxelType));
			}
			
			foreach (var plantEntity in _filterPlants)
			{
				var plantPos = _plantPositionPool.Get(plantEntity).Value.AsSerializable();
				var plantType = _plantTypePool.Get(plantEntity).Value;
				saveData.PlantsaveData.Add(new PlantSaveData(plantPos, plantType));
			}

			saveData.SaveDateTime = DateTime.Now;
			_lastTimeSave = Time.realtimeSinceStartup;
			
			Debug.Log($"Start save : {_lastTimeSave}");
			
			lock (saveData)
			{
				Task.Run(() => _saveModel.SaveAsync(saveData));
			}
			
			PlayerPrefs.SetString(SaveModel.LAST_SAVE_PREFS_KEY, $"{saveData.SaveDateTime:yyyy-MM-dd:HH:mm:ss}");
		}
	}
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using ModestTree;
using UnityEngine;
using Zenject;

namespace DefaultNamespace.Systems.Save
{
	public class SaveModel : ISaveModel, IInitializable
	{
		public SaveData LastSave
		{
			get => null;
			private set { }
		}
		
		public bool IsSaveInProcess { get; private set; }
		public const string LAST_SAVE_PREFS_KEY = "lastsave";
		private string _persistentDataPath = Application.persistentDataPath;
		private string _lastSavePath;

		public async void Initialize()
		{
			var lastSave = PlayerPrefs.GetString(LAST_SAVE_PREFS_KEY, string.Empty);
			if (lastSave.IsEmpty())
				return;

			_lastSavePath = Path.Combine(_persistentDataPath, $"save{lastSave}.bin");
			if (DateTime.TryParseExact(lastSave, "yyyy-MM-dd:HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out var result))
			{
				LastSave = await LoadAsync(result);
			}
		}

		public Task SaveAsync(SaveData saveData)
		{
			var saveDateTime = saveData.SaveDateTime;
			var filePath = Path.Combine(_persistentDataPath, $"save{saveDateTime:yyyy-MM-dd:HH:mm:ss}.bin");
			IsSaveInProcess = true;
			
			return Task.Run(async () =>
			{
				if (_lastSavePath != null)
				{
					if (File.Exists(_lastSavePath))
					{
						File.Delete(_lastSavePath);
						Debug.Log($"Delete save from disk : {_lastSavePath}");
					}
				}
				
				_lastSavePath = filePath;
				var stream = new FileStream(_lastSavePath, FileMode.Create, FileAccess.Write, FileShare.None);
				var formatter = new BinaryFormatter();
				formatter.Serialize(stream, saveData);
				await stream.DisposeAsync();
				Debug.Log($"Write save on disk : {saveDateTime:yyyy-MM-dd:HH:mm:ss} at path: {filePath}");
				IsSaveInProcess = false;
			});
		}

		public async Task<SaveData> LoadAsync(DateTime saveDate)
		{
			var filePath = Path.Combine(_persistentDataPath, $"save{saveDate:yyyy-MM-dd:HH:mm:ss}.bin");

			if (!File.Exists(filePath))
			{
				return null;
			}

			await using Stream readStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
			var formatter = new BinaryFormatter();
			return (SaveData) formatter.Deserialize(readStream);
		}

		public async Task<List<SaveData>> LoadAll()
		{
			var saves = new List<SaveData>();

			if (!Directory.Exists(_persistentDataPath)) 
				return saves;
			
			var fileNames = Directory.GetFiles(_persistentDataPath);
			for (var index = fileNames.Length - 1; index >= 0; index--)
			{
				var fileName = fileNames[index];
				var saveTime = fileName
					.Replace("save", string.Empty)
					.Replace(".bin", string.Empty)
					.Replace(Application.persistentDataPath, string.Empty)
					.Replace("/", string.Empty);

				if (DateTime.TryParseExact(saveTime, "yyyy-MM-dd:HH:mm:ss", null,
						System.Globalization.DateTimeStyles.None, out var result))
				{
					var save = await LoadAsync(result);
					saves.Add(save);
				}
			}

			return saves;
		}
	}
}
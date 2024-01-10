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

		public async void Initialize()
		{
			var lastSave = PlayerPrefs.GetString(LAST_SAVE_PREFS_KEY, string.Empty);
			if (lastSave.IsEmpty())
				return;

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
			
			return Task.Run(() =>
			{
				var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
				var formatter = new BinaryFormatter();
				formatter.Serialize(stream, saveData);
				stream.DisposeAsync();
				Debug.LogError($"Write save on disk : {saveDateTime:yyyy-MM-dd:HH:mm:ss}");
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
			
			if (Directory.Exists(_persistentDataPath))
			{
				var fileNames = Directory.GetFiles(_persistentDataPath);
				var max = 0;
				for (var index = fileNames.Length - 1; index >= 0; index--)
				{
					var fileName = fileNames[index];
					if (max > 3)
					{
						return saves;
					}

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
						max++;
					}
				}
			}

			return saves;
		}
	}
}
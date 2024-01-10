using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DefaultNamespace.Systems.Save
{
	public interface ISaveModel
	{
		Task SaveAsync(SaveData saveData);
		Task<SaveData> LoadAsync(DateTime saveDate);
		Task<List<SaveData>> LoadAll();
		SaveData LastSave { get; }
		bool IsSaveInProcess { get; }
	}
}
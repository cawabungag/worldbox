using UnityEngine;

namespace DefaultNamespace.Db
{
	[CreateAssetMenu(fileName = "GameSetting", menuName = "Data/GameSetting")]
	public class GameSetting : ScriptableObject
	{
		[SerializeField]
		private bool _isSaveEnable;
		public bool IsSaveEnable => _isSaveEnable;
	}
}
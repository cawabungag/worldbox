using Core.WindowService;
using UnityEngine;

namespace DefaultNamespace.UI.Views.Save
{
	public class UiSaveHudView : BaseView
	{
		[SerializeField]
		private SaveSlotView _saveSlotOriginal;
		
		[SerializeField]
		private Transform _saveSlotTarget;
		
		public SaveSlotView SaveSlotOriginal => _saveSlotOriginal;
		public Transform SaveSlotTarget => _saveSlotTarget;
	}
}
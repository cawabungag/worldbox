using Core.WindowService;
using UnityEngine;

namespace DefaultNamespace.UI.Views.Save
{
	public class UiAutoSaveHudView : BaseView
	{
		[SerializeField]
		private GameObject _autosaveLabel;

		public GameObject AutoSaveLable => _autosaveLabel;
	}
}
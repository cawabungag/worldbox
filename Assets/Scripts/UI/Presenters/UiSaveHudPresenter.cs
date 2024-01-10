using Core.WindowService;
using DefaultNamespace.Systems.Save;
using DefaultNamespace.UI.Views.Save;
using UnityEngine;
using Zenject;

namespace UI.Presenters
{
	public class UiSaveHudPresenter : BasePresenter<UiSaveHudView>, IInitializable
	{
		private readonly ISaveModel _saveModel;
		public override bool IsPopUp => true;
		public UiSaveHudPresenter(UiSaveHudView view, ISaveModel saveModel) : base(view)
		{
			_saveModel = saveModel;
		}

		public async void Initialize()
		{
			var allSaves = await _saveModel.LoadAll();
			foreach (var save in allSaves)
			{
				var saveSlotView = Object.Instantiate(View.SaveSlotOriginal, View.SaveSlotTarget);
				var saveSlot = new SaveSlotData(save);
				var savePreview = saveSlot.GenerateSavePreview();
				saveSlotView.SaveImage.texture = savePreview;
				saveSlotView.SaveName.text = saveSlot.Name;
			}
		}
	}
}
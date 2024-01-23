using System.Collections.Generic;
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
		private readonly List<SaveSlotView> _saveSlotsBuffer = new();
		public override bool IsPopUp => true;
		public UiSaveHudPresenter(UiSaveHudView view, ISaveModel saveModel) : base(view)
		{
			_saveModel = saveModel;
		}

		public async void Initialize()
		{
			ClearSlots();

			var allSaves = await _saveModel.LoadAll();
			foreach (var save in allSaves)
			{
				var saveSlotView = Object.Instantiate(View.SaveSlotOriginal, View.SaveSlotTarget);
				var saveSlot = new SaveSlotData(save);
				var savePreview = saveSlot.GenerateSavePreview();
				saveSlotView.SaveImage.texture = savePreview;
				saveSlotView.SaveName.text = saveSlot.Name;
				_saveSlotsBuffer.Add(saveSlotView);
			}
		}

		private void ClearSlots()
		{
			foreach (var saveSlotView in _saveSlotsBuffer)
			{
				Object.Destroy(saveSlotView.gameObject);
			}

			_saveSlotsBuffer.Clear();
		}

		protected override void OnOpen()
		{
			base.OnOpen();
			Initialize();
		}
	}
}
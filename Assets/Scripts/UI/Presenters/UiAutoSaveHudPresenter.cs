using Core.WindowService;
using DefaultNamespace.Systems.Save;
using DefaultNamespace.UI.Views.Save;
using Zenject;

namespace UI.Presenters
{
	public class UiAutoSaveHudPresenter : BasePresenter<UiAutoSaveHudView>, ITickable
	{
		private readonly ISaveModel _saveModel;
		public override bool IsPopUp => true;

		public UiAutoSaveHudPresenter(UiAutoSaveHudView view, ISaveModel saveModel) : base(view)
		{
			_saveModel = saveModel;
		}

		public void Tick()
		{
			SetLable(_saveModel.IsSaveInProcess);
		}

		private void SetLable(bool isOn)
		{
			View.AutoSaveLable.SetActive(isOn);
		}
	}
}
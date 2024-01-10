using DefaultNamespace.UI.Views.Save;
using DefaultNamespace.Utils;
using UI.Presenters;
using UI.Views;
using UnityEngine;
using Zenject;

namespace Installers
{
	public class GameSceneUiInstaller : MonoInstaller
	{
		[SerializeField]
		private UiToolHudView _uiToolHudView;
		
		[SerializeField]
		private UiBrushHudView _uiBrushHudView;
		
		[SerializeField]
		private UiSaveHudView _uiSaveHudView;
		
		[SerializeField]
		private UiAutoSaveHudView _uiAutoSaveHudView;
		
		public override void InstallBindings()
		{
			Container.BindUi<UiToolHudPresenter, UiToolHudView>(_uiToolHudView);
			Container.BindUi<UiBrushHudPresenter, UiBrushHudView>(_uiBrushHudView);
			Container.BindUi<UiSaveHudPresenter, UiSaveHudView>(_uiSaveHudView);
			Container.BindUi<UiAutoSaveHudPresenter, UiAutoSaveHudView>(_uiAutoSaveHudView);
		}
	}
}
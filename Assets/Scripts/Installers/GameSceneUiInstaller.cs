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
		private GameHudView _gameHudView;
		
		public override void InstallBindings()
		{
			Container.BindUi<GameHudPresenter, GameHudView>(_gameHudView);
		}
	}
}
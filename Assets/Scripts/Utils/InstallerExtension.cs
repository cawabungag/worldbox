using Core.WindowService;
using Zenject;

namespace DefaultNamespace.Utils
{
	public static class InstallerExtension
	{
		public static void BindUi<TPresenter, TView>(this DiContainer container, TView view) where TPresenter : IPresenter
		{
			container.BindInterfacesAndSelfTo<TPresenter>().AsSingle().NonLazy();
			container.Bind<TView>().FromInstance(view);
		}
	}
}
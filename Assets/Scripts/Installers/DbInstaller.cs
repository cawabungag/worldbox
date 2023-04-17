using Db.Brush;
using UnityEngine;
using Zenject;

namespace Installers
{
	public class DbInstaller : MonoInstaller
	{
		[SerializeField]
		private BrushesData _brushes;

		public override void InstallBindings()
		{
			Container.BindInstance(_brushes);
		}
	}
}
using System;
using System.Collections.Generic;
using Zenject;

namespace Core.WindowService
{
	public class WindowService : IWindowService
	{
		private readonly DiContainer _container;
		private readonly Stack<IPresenter> _presentersStack = new();

		public WindowService(DiContainer container)
		{
			_container = container;
		}

		public void DisposePresenters()
		{
			var registeredPresenters = _container.Resolve<IPresenter[]>();
			foreach (var presenter in registeredPresenters)
			{
				presenter.Close();
				presenter.Dispose();
			}

			_presentersStack.Clear();
		}

		public void Open<T>()
		{
			if (_presentersStack.TryPop(out var openedPresenter))
				if (!openedPresenter.IsPopUp)
					openedPresenter.Close();

			var presenter = GetPresenter<T>();
			_presentersStack.Push(presenter);
			presenter.Open();
		}

		public void Close<T>()
		{
			var openedPresenter = _presentersStack.Pop();
			openedPresenter.Close();
		}

		private IPresenter GetPresenter<T>()
		{
			var registeredPresenters = _container.Resolve<IPresenter[]>();
			foreach (var presenter in registeredPresenters)
				if (presenter.GetType() == typeof(T))
					return presenter;

			throw new InvalidOperationException($"There is no presenter with the type of {typeof(T)}");
		}
	}

	public interface IPopUp
	{
	}
}
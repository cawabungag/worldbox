using System;
using System.Collections.Generic;

namespace Core.WindowService
{
	public class WindowService : IWindowService
	{
		private readonly List<IPresenter> _registeredPresenters;
		private readonly Stack<IPresenter> _presentersStack = new();

		public WindowService(IPresenter[] presenters)
		{
			_registeredPresenters = new List<IPresenter>(presenters);
		}

		public void DisposePresenters()
		{
			foreach (var presenter in _registeredPresenters)
			{
				presenter.Close();
				presenter.Dispose();
			}

			_registeredPresenters.Clear();
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
			foreach (var presenter in _registeredPresenters)
				if (presenter.GetType() == typeof(T))
					return presenter;

			throw new InvalidOperationException($"There is no presenter with the type of {typeof(T)}");
		}
	}

	public interface IPopUp
	{
	}
}
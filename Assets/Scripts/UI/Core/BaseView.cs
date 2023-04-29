using UnityEngine;

namespace Core.WindowService
{
	public abstract class BaseView : MonoBehaviour, IView
	{
		public void Open()
		{
			gameObject.SetActive(true);
			OnOpen();
		}

		public void Close()
		{
			gameObject.SetActive(false);
			OnClose();
		}
		
		protected virtual void OnOpen(){}
		protected virtual void OnClose(){}
	}
}
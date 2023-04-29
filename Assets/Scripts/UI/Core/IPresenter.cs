namespace Core.WindowService
{
	public interface IPresenter
	{
		void Open();
		void Close();
		void Dispose();
		bool IsPopUp { get; }
	}
}
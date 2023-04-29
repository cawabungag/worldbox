
namespace Core.WindowService
{
	public interface IWindowService
	{
		void DisposePresenters();
		
		void Open<T>();
		void Close<T>();
	}
}
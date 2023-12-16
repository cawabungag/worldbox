namespace DefaultNamespace.Saver
{
	public interface ISerializableComponent<TValue>
	{
		void Write(TValue value);
		TValue Read();
	}
}
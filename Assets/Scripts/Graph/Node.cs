using System.Collections.Generic;

public class Node<T>
{
	public T Value { get; set; }
	public List<Node<T>> Neighbors { get; set; }

	public Node(T value)
	{
		Value = value;
		Neighbors = new List<Node<T>>();
	}
}
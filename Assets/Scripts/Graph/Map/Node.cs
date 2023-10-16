using UnityEngine;

public class Node<T>
{
	public T Value;
	public Node(T value) => Value = value;
}

public class MapNode
{
	public int Entity;
	public Vector2Int Position;

	public MapNode(Vector2Int pos, int entity)
	{
		Position = pos;
		Entity = entity;
	}
}


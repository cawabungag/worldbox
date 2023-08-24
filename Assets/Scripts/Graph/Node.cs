using UnityEngine;

public class Node
{
	public int Entity;
	public Vector2Int Position;

	public Node(Vector2Int pos, int entity)
	{
		Position = pos;
		Entity = entity;
	}
}
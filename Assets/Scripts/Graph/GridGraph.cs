using System;
using UnityEngine;
using System.Collections.Generic;

public class GridGraph
{
	private List<Node> _neighboursBuffer = new();
	private readonly Node[,] _grid;

	public GridGraph(int gridSizeX, int gridSizeY)
	{
		_grid = new Node[gridSizeX, gridSizeY];
	}

	public void SetEntity(int x, int y, Vector2Int position, int entity)
	{
		try
		{
			_grid[x, y] = new Node(position, entity);
		}
		catch (Exception e)
		{
			Debug.LogError($"{e} x: {x} y: {y}");
			throw;
		}
	}

	public int GetEntity(int x, int y)
	{
		return _grid[x, y].Entity;
	}

	public List<Node> GetNeighbours(Node node)
	{
		_neighboursBuffer.Clear();

		for (int x = -1; x <= 1; x++)
		{
			for (int y = -1; y <= 1; y++)
			{
				if (x == 0 && y == 0)
					continue;

				var checkX = node.Position.x + x;
				var checkY = node.Position.y + y;

				if (checkX >= 0 && checkX < _grid.GetLength(0) && checkY >= 0
					&& checkY < _grid.GetLength(1))
				{
					_neighboursBuffer.Add(_grid[checkX, checkY]);
				}
			}
		}

		return _neighboursBuffer;
	}
}
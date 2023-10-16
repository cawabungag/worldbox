using System;
using UnityEngine;
using System.Collections.Generic;

public class GridGraph<T>
{
	private readonly Node<T>[,] _grid;

	public GridGraph(int gridSizeX, int gridSizeY)
	{
		_grid = new Node<T>[gridSizeX, gridSizeY];
	}

	public void SetEntity(int x, int y, T value)
	{
		try
		{
			_grid[x, y] = new Node<T>(value);
		}
		catch (Exception e)
		{
			Debug.LogError($"{e} x: {x} y: {y}");
			throw;
		}
	}

	public T GetValue(int x, int y)
	{
		return _grid[x, y].Value;
	}
}
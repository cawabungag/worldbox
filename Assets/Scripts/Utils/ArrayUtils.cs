using System;
using System.Collections.Generic;
using DefaultNamespace.Utils;
using UnityEngine;

public static class ArrayUtils
{
	public static void AddOrCreateValue(this Dictionary<int, List<Vector3>> dictionary,
		int newKey,
		Vector3 newValue,
		List<Vector3> buffer = null)
	{
		if (dictionary.TryGetValue(newKey, out var value))
		{
			value.Add(newValue);
			return;
		}

		if (buffer == null)
		{
			dictionary.Add(newKey, new List<Vector3>(WorldUtils.WORLD_SIZE * WorldUtils.WORLD_SIZE) {newValue});
		}
		else
		{
			buffer.Add(newValue);
			dictionary.Add(newKey, buffer);
		}
	}

	public static void AddOrCreateValue(this Dictionary<int, List<Vector2>> dictionary,
		int newKey,
		Vector2 newValue,
		List<Vector2> buffer = null)
	{
		if (dictionary.TryGetValue(newKey, out var value))
		{
			value.Add(newValue);
			return;
		}

		if (buffer == null)
		{
			dictionary.Add(newKey, new List<Vector2>(WorldUtils.WORLD_SIZE * WorldUtils.WORLD_SIZE) {newValue});
		}
		else
		{
			buffer.Add(newValue);
			dictionary.Add(newKey, buffer);
		}
	}

	public static void AddOrCreateValue(this Dictionary<int, List<int>> dictionary,
		int newKey,
		int newValue,
		List<int> buffer = null)
	{
		if (dictionary.TryGetValue(newKey, out var value))
		{
			value.Add(newValue);
			return;
		}

		if (buffer == null)
		{
			dictionary.Add(newKey, new List<int>(WorldUtils.WORLD_SIZE * WorldUtils.WORLD_SIZE) {newValue});
		}
		else
		{
			buffer.Add(newValue);
			dictionary.Add(newKey, buffer);
		}
	}

	public static T[,] ToRectangular<T>(this T[] original)
	{
		var size = (int) Math.Sqrt(original.Length);
		var arr = new T[size, size];
		var x = 0;
		var y = 0;

		for (int i = 0; i < original.Length; i++)
		{
			arr[y, x] = original[i];
			x++;

			if (x != size)
				continue;

			x = 0;
			y++;
		}

		return arr;
	}

	public static int[,] Slice(this int[,] board, int xStart, int yStart, int xFinish, int yFinish)
	{
		var result = new int[xFinish - xStart, yFinish - yStart];

		for (var x = xStart; x < xFinish; x++)
		for (var y = yStart; y < yFinish; y++)
			result[x - xStart, y - yStart] = board[x, y];

		return result;
	}
}
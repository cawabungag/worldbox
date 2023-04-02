using System;
using System.Collections.Generic;

public static class ArrayUtils
{
	public static void AddOrCreateValue<TK, TV>(this Dictionary<TK, List<TV>> dictionary,
		TK newKey,
		TV newValue)
	{
		if (dictionary.TryGetValue(newKey, out var value))
		{
			value.Add(newValue);
			return;
		}

		dictionary.Add(newKey, new List<TV> {newValue});
	}
	
	public static void AddOrCreateValue<TK>(this Dictionary<TK, int> dictionary,
		TK newKey,
		int newValue)
	{
		if (dictionary.TryGetValue(newKey, out var value))
		{
			value += newValue;
			dictionary[newKey] = value;
			return;
		}

		dictionary.Add(newKey, newValue);
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

		// for (var i = 0; i < original.Length; ++i)
		// {
		// 	arr[x, y] = original[i];
		// 	x++;
		//
		// 	if (x != size)
		// 		continue;
		//
		// 	x = 0;
		// 	y++;
		// }

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
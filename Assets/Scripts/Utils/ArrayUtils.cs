using System;

namespace DefaultNamespace.Utils
{
	public class ArrayUtils
	{
		public static T[,] SplitArray<T>(T[] arr, int blockSize)
		{
			var numBlocks = (int)Math.Ceiling((double)arr.Length / blockSize);
			var numRows = numBlocks;
			var numCols = blockSize;
			var blocks = new T[numRows, numCols];
			var index = 0;
			for (var i = 0; i < numRows; i++)
			{
				for (var j = 0; j < numCols; j++)
				{
					if (index < arr.Length)
					{
						blocks[i, j] = arr[index];
						index++;
					}
					else
					{
						break;
					}
				}
			}
			return blocks;
		}

		public static int[,] Slice(int[][] board, int xStart, int yStart, int xFinish, int yFinish)
		{
			var result = new int[xFinish - xStart, yFinish - yStart];
			for (var i = xStart; i < xFinish; i++)
			for (var j = yStart; j < yFinish; j++)
				result[i - xStart, j - yStart] = board[i][j];

			return result;
		}
	}
}
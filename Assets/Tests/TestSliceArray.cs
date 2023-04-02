using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class TestSliceArray
{
	[Test]
	public void TestSliceArraySimplePasses()
	{
		var testCase = new[]
		{
			1, 2, 3, 4,
			5, 6, 7, 8,
			9, 10, 11, 12,
			13, 14, 15, 16
		};

		var rectangular = testCase.ToRectangular();
		const int  chunksPerSide = 2;

		var nextCoord = new Vector2Int(chunksPerSide, chunksPerSide);
		var startCoord = new Vector2Int();

		var result = new List<int>();
		var lengthY = rectangular.GetLength(1) / chunksPerSide;

		for (int y = 0; y < lengthY; y++)
		{
			var lengthX = rectangular.GetLength(0) / chunksPerSide;
			for (int x = 0; x < lengthX; x++)
			{
				var sliceBoard = rectangular.Slice(startCoord.x, startCoord.y, nextCoord.x, nextCoord.y);
				// result.Add(item);

				startCoord.x += chunksPerSide;
				nextCoord.x += chunksPerSide;
			}

			startCoord.x = 0;
			nextCoord.x = chunksPerSide;

			startCoord.y += chunksPerSide;
			nextCoord.y += chunksPerSide;
		}


		Assert.AreEqual(result.ToArray(), testCase);
	}
}
using System;

namespace Game.Services.MapGenerator.Impls
{
	public class MapGeneratorProvider : IMapGeneratorProvider
	{
		public ETileType[] GenerateGround(int width, int height)
		{
			var random = new Random();

			var mnoise1 = new SampleGenerator(width, height, random, 16);
			var mnoise2 = new SampleGenerator(width, height, random, 16);
			var mnoise3 = new SampleGenerator(width, height, random, 16);

			var noise1 = new SampleGenerator(width, height, random, 32);
			var noise2 = new SampleGenerator(width, height, random, 32);

			var map = new ETileType[width * height];
			for (var y = 0; y < height; y++)
			{
				for (var x = 0; x < width; x++)
				{
					var i = x + y * width;

					var val = Math.Abs(noise1.values[i] - noise2.values[i]) * 3 - 2;
					var mval = Math.Abs(mnoise1.values[i] - mnoise2.values[i]);
					mval = Math.Abs(mval - mnoise3.values[i]) * 3 - 2;

					var xd = x / (width - 1.0) * 2 - 1;
					var yd = y / (height - 1.0) * 2 - 1;
					if (xd < 0) xd = -xd;
					if (yd < 0) yd = -yd;
					var dist = xd >= yd ? xd : yd;
					dist = dist * dist * dist * dist;
					dist = dist * dist * dist * dist;
					val = val + 1 - dist * 20;

					if (val < LayerRatio.Ground.NOISE_WATER_LOWER)
						map[i] = ETileType.Water;
					else if (val > LayerRatio.Ground.NOISE_ROCK_HIGHER && mval < LayerRatio.Ground.NOISE_ROCK_LOWER)
						map[i] = ETileType.Rock;
					else if (val < LayerRatio.Ground.NOISE_HILL_HIGHER)
						map[i] = ETileType.Plain;
					else
						map[i] = ETileType.Forest;
				}
			}

			for (var i = 0; i < width * height / LayerRatio.Ground.RATIO_SAND; i++)
			{
				var xs = random.Next(width);
				var ys = random.Next(height);
				for (var k = 0; k < 10; k++)
				{
					var x = xs + random.Next(21) - 10;
					var y = ys + random.Next(21) - 10;
					for (var j = 0; j < 100; j++)
					{
						var xo = x + random.Next(5) - random.Next(5);
						var yo = y + random.Next(5) - random.Next(5);
						for (var yy = yo - 1; yy <= yo + 1; yy++)
						for (var xx = xo - 1; xx <= xo + 1; xx++)
							if (xx >= 0 && yy >= 0 && xx < width && yy < height)
								if (map[xx + yy * width] == ETileType.Plain)
									map[xx + yy * width] = ETileType.Sand;
					}
				}
			}

			return map;
		}
	}
}
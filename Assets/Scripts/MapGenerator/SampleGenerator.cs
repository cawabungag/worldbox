using System;

namespace Game.Services.MapGenerator
{
	public class SampleGenerator
	{
		private readonly int _height;
		private readonly int _width;
		public readonly double[] values;

		public SampleGenerator(int width, int height, Random random, int stepSize)
		{
			_height = height;
			_width = width;

			values = new double[_width * _height];

			for (var y = 0; y < _width; y += stepSize)
			{
				for (var x = 0; x < _width; x += stepSize)
				{
					SetSample(x, y, random.NextDouble() * 2 - 1);
				}
			}

			var scale = 1.0 / _width;
			double scaleMod = 1;
			do
			{
				var halfStep = stepSize / 2;
				for (var y = 0; y < _width; y += stepSize)
				{
					for (var x = 0; x < _width; x += stepSize)
					{
						var xStepSize = x + stepSize;
						var yStepSize = y + stepSize;

						var a = GetSample(x, y);
						var b = GetSample(xStepSize, y);
						var c = GetSample(x, yStepSize);
						var d = GetSample(xStepSize, yStepSize);

						var e = (a + b + c + d) / 4.0 + (random.NextDouble() * 2 - 1) * stepSize * scale;
						SetSample(x + halfStep, y + halfStep, e);
					}
				}

				for (var y = 0; y < _width; y += stepSize)
				{
					for (var x = 0; x < _width; x += stepSize)
					{
						var xHalfStep = x + halfStep;
						var yHalfStep = y + halfStep;

						var a = GetSample(x, y);
						var b = GetSample(x + stepSize, y);
						var c = GetSample(x, y + stepSize);
						var d = GetSample(xHalfStep, yHalfStep);
						var e = GetSample(xHalfStep, y - halfStep);
						var f = GetSample(x - halfStep, yHalfStep);

						var h = (a + b + d + e) / 4.0 + (random.NextDouble() * 2 - 1) * stepSize * scale * 0.5;
						var g = (a + c + d + f) / 4.0 + (random.NextDouble() * 2 - 1) * stepSize * scale * 0.5;
						SetSample(xHalfStep, y, h);
						SetSample(x, yHalfStep, g);
					}
				}

				stepSize /= 2;
				scale *= scaleMod + 0.8;
				scaleMod *= 0.3;
			} while (stepSize > 1);
		}

		private double GetSample(int x, int y)
			=> values[(x & (_width - 1)) + (y & (_height - 1)) * _width];

		private void SetSample(int x, int y, double value)
			=> values[(x & (_width - 1)) + (y & (_height - 1)) * _width] = value;
	}
}
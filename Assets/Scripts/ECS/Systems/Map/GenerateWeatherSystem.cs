using DefaultNamespace.Components.Weather;
using DefaultNamespace.Utils;
using Leopotam.EcsLite;
using UnityEngine;

namespace ECS.Systems
{
	public class GenerateWeatherSystem : IEcsInitSystem
	{
		public void Init(IEcsSystems systems)
		{
			var world = systems.GetWorld();
			var halfWidth = WorldUtils.WORLD_SIZE / 2;
			var halfHeight = WorldUtils.WORLD_SIZE / 2;
			var mapGraph = new GridGraph<float>(WorldUtils.WORLD_SIZE, WorldUtils.WORLD_SIZE);
			var perlinNoise = new PerlinNoiseGenerator("asdasdasdfafsdfdsasdasdas");
			for (var i = 0; i < WorldUtils.WORLD_SIZE; i++)
			{
				for (var j = 0; j < WorldUtils.WORLD_SIZE; j++)
				{
					var noise = perlinNoise.GetNoise(new Vector2Int(j, i));
					var temperature = Mathf.Lerp(WorldUtils.MIN_TEMPERATURE,
						WorldUtils.MAX_TEMPERATURE, noise);
					mapGraph.SetEntity(j, i, temperature);
				}
			}

			var mapGraphEntity = world.NewEntity();
			world.GetPool<MapWeatherComponent>().Add(mapGraphEntity).Value = mapGraph;
		}
	}

	internal class PerlinNoiseGenerator
	{
		private float[,] Noise { get; set; }
		private string Seed { get; set; }

		public PerlinNoiseGenerator(string seed)
		{
			Seed = seed;
			Noise = new float[WorldUtils.WORLD_SIZE, WorldUtils.WORLD_SIZE];
		}

		public float GetXZ(Vector2Int position, int octaves, int resolution, float bias)
		{
			var fNoise = 0.0f;
			var fScale = 1.0f;
			var fScaleAcc = 0.0f;

			for (var i = 0; i < octaves; i++)
			{
				var pitch = resolution >> i;
				if (pitch <= 0) continue;

				var sample1 = new Vector2Int(
					(position.x / pitch) * pitch,
					(position.y / pitch) * pitch);

				var sample2 = new Vector2Int(
					(sample1.x + pitch),
					(sample1.y + pitch));

				var fBlendX = (position.x - sample1.x) / (float) pitch;
				var fBlendZ = (position.y - sample1.y) / (float) pitch;

				var fSampleT = (1.0f - fBlendX) * GetNoise(sample1)
								+ fBlendX * GetNoise(new Vector2Int(sample2.x, sample1.y));
				var fSampleB =
					(1.0f - fBlendX) * GetNoise(new Vector2Int(sample1.x, sample2.y))
					+ fBlendX * GetNoise(sample2);

				fScaleAcc += fScale;
				fNoise += (fBlendZ * (fSampleB - fSampleT) + fSampleT) * fScale;
				fScale = fScale / bias;
			}

			return fNoise / fScaleAcc;
		}

		public float GetNoise(Vector2Int position)
		{
			if (Noise[position.x, position.y] == 0.0)
			{
				Noise[position.x, position.y] = GetXZNoise(position);
			}

			return Noise[position.x, position.y];
		}

		private float GetXZNoise(Vector2Int position)
		{
			var key = position.x + position.y.ToString() + Seed;
			return Hash.Jenkins(key) / (float) uint.MaxValue;
		}
	}

	internal class Hash
	{
		public static uint Jenkins(string key)
		{
			var i = 0;
			uint hash = 0;
			while (i != key.Length)
			{
				hash += key[i++];
				hash += hash << 10;
				hash ^= hash >> 6;
			}

			hash += hash << 3;
			hash ^= hash >> 11;
			hash += hash << 15;
			return hash;
		}
	}
}
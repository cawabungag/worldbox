using Game.Services.MapGenerator;
using NUnit.Framework;
using Services.Map;
using UnityEngine;

namespace Tests
{
	public class TestMapService
	{
		[Test]
		public void TestMapServiceFixture()
		{
			IMapService mapService = new MapService();
			var voxel001 = new VoxelData(new Vector3Int(0, 0, 1), VoxelType.None);
			var voxel010 = new VoxelData(new Vector3Int(0, 1, 0), VoxelType.None);
			var voxel100 = new VoxelData(new Vector3Int(1, 0, 0), VoxelType.None);

			mapService.AddVoxel(voxel001);
			mapService.AddVoxel(voxel010);
			mapService.AddVoxel(voxel100);

			Assert.IsFalse(mapService.IsTransparent(0, 0, 1));
			Assert.IsFalse(mapService.IsTransparent(0, 1, 0));
			Assert.IsFalse(mapService.IsTransparent(1, 0, 0));
			Assert.IsTrue(mapService.IsTransparent(1, 1, 1));
		}
	}
}
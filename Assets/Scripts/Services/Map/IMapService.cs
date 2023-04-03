namespace Services.Map
{
	public interface IMapService
	{
		void AddVoxel(VoxelData voxelData);
		bool IsTransparent(int x, int y, int z);
	}
}
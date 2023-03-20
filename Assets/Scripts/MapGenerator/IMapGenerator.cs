namespace Game.Services.MapGenerator
{
	public interface IMapGenerator
	{
		VoxelType[] GenerateGround(int width, int height);
	}
}
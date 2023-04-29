using UnityEngine;

namespace Core.Camera
{
	public interface ISceneCamera : ICamera
	{
		float Size { get; set; }

		Vector3 Position { get; set; }

		void Translate(Vector2 translation);
	}
}
using UnityEngine;

namespace Core.Camera
{
	public interface ICamera
	{
		Vector3 ScreenToWordPoint(Vector3 screenPos);
	}
}
using System;
using DefaultNamespace.Utils;
using UnityEngine;

namespace Core.Camera.Impl
{
	public class SceneCamera : MonoBehaviour, ISceneCamera
	{
		[SerializeField]
		private UnityEngine.Camera _camera;

		[SerializeField]
		private float _cameraSpeed;

		private float _size;

		public float Size
		{
			get => _size;
			set
			{
				if (Math.Abs(_size - value) < 0.0001f)
					return;

				_size = Mathf.Max(value, 1);
				_camera.orthographicSize = _size;
			}
		}

		public Vector3 Position
		{
			get => transform.position;
			set => transform.position = value;
		}

		public Vector3 ScreenToWordPoint(Vector3 screenPos)
		{
			screenPos.z = 0.1f;
			return _camera.ScreenToWorldPoint(screenPos);
		}

		public void Translate(Vector2 translation)
		{
			var positionY = Position.y;
			var moveVector = new Vector3(translation.x, 0, translation.y);
			moveVector.Normalize();
			var newPosiiton = Position - moveVector * _cameraSpeed;
			newPosiiton.x = Mathf.Clamp(newPosiiton.x, -WorldUtils.WORLD_SIZE / 2, WorldUtils.WORLD_SIZE / 2);
			newPosiiton.z = Mathf.Clamp(newPosiiton.z, -WorldUtils.WORLD_SIZE / 2, WorldUtils.WORLD_SIZE / 2);
			Position = new Vector3(newPosiiton.x, positionY, newPosiiton.z);
		}

		private void Awake()
		{
			_size = _camera.orthographicSize;
		}
	}
}
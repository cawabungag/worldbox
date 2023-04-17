using System;
using UnityEngine;

namespace Core.Camera.Impl
{
	public class SceneCamera : MonoBehaviour, ISceneCamera
	{
		[SerializeField]
		private UnityEngine.Camera _camera;


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

		public Vector2 Position
		{
			get => transform.position;
			set
			{
				var position = transform.position;
				position.x = value.x;
				position.y = value.y;
				transform.position = position;
			}
		}

		public Vector3 ScreenToWordPoint(Vector3 screenPos) => _camera.ScreenToWorldPoint(screenPos);

		public void Translate(Vector2 translation) => transform.Translate(-translation);

		private void Awake()
		{
			_size = _camera.orthographicSize;
		}
	}
}
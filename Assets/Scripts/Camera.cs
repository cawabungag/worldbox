using UnityEngine;

namespace DefaultNamespace
{
	public class Camera : MonoBehaviour
	{
		public Transform _transform;
 
		void Update(){
			transform.LookAt(_transform);
			var horizontal = Input.GetAxis("Horizontal");
			var vertical = Input.GetAxis("Vertical");
			var directional = new Vector2(horizontal, vertical);
			transform.Translate(directional * Time.deltaTime);
		}
	}
}
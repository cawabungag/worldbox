using UnityEngine;

[RequireComponent(typeof(Camera))]
public class RTSCameraController : MonoBehaviour
{
	public float translationSpeed = 60f;

	private void Update()
	{
		var hor = Input.GetAxis("Horizontal");
		var ver = Input.GetAxis("Vertical");
		var direction = new Vector3(hor, 0, ver);
		transform.Translate(direction * Time.deltaTime * translationSpeed, Space.World);
	}
	
	private void LateUpdate()
	{
		var pos = gameObject.transform.position;
		gameObject.transform.position =
			new Vector3(Mathf.Clamp(pos.x, -128f, 128f), pos.y, Mathf.Clamp(pos.z, -128f, 128f));
	}
}
using UnityEngine;

public class Zoom : MonoBehaviour
{
	private Camera _camera;

	public float sensitivity;

	public float speedMultiplier = 5;

	private void Start()
	{
		_camera = GetComponent<Camera>();
	}

	private void Update()
	{
		_camera.orthographicSize -= Input.mouseScrollDelta.y * sensitivity * _camera.orthographicSize * (Input.GetKey(KeyCode.LeftShift) ? speedMultiplier : 1);
	}
}
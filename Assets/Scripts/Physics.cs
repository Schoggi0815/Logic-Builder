using UnityEngine;

public static class Physics
{
	public static Vector3 GetWorldPosOfMouse()
	{
		var screenToWorldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		return new Vector3(screenToWorldPoint.x, screenToWorldPoint.y, 0);
	}

	public static bool IsClose(Vector3 first, Vector3 second)
	{
		var distance = first - second;
		return distance.magnitude < 2;
	}
}
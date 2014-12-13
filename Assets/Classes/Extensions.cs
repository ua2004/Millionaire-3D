using UnityEngine;
using System.Collections;

/**
 * Class which contains extension methods for some Unity classes.
 * Must be static.
 */
public static class Extensions {
	
	/**
	 * Sets only X coordinate of object's position remaining Y and Z untouched.
	 * First parameter must have "this" in front of type.
	 */
	public static void SetPositionX(this Transform t, float newX)
	{
		t.position = new Vector3(newX, t.position.y, t.position.z);
	}

	/**
	 * Destroys all children of specified object.
	 * @return Transform
	 */
	public static Transform DestroyChildren(this Transform transform)
	{
		foreach (Transform child in transform) {
			GameObject.Destroy(child.gameObject);
		}
		return transform;
	}
}

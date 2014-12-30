using UnityEngine;
using System.Collections;

/**
 * Base class for all lifelines.
 */
public abstract class Lifeline {

	/**
	 * Must be overridden in derived classes.
	 */
	public abstract void Use();
}

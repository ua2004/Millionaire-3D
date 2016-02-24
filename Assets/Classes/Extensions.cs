using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
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

	public static void SetPositionY(this Transform t, float newY)
	{
		t.position = new Vector3(t.position.x, newY, t.position.z);
	}

	public static void SetPositionZ(this Transform t, float newZ)
	{
		t.position = new Vector3(t.position.x, t.position.y, newZ);
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

	/**
	 * Add listener (event handler) to the object which has EventTrigger component.
	 * @param UnityAction action Event handler (lambda or function name) to be called when the event is triggered
	 * @param EventTriggerType triggerType Event to be handled
	 * 
	 * @author AyARL
	 */
	public static void AddEventTrigger(this EventTrigger eventTrigger, UnityAction action, EventTriggerType triggerType)
	{
		// Create a nee TriggerEvent and add a listener
		EventTrigger.TriggerEvent trigger = new EventTrigger.TriggerEvent();
		trigger.AddListener((eventData) => action()); // ignore event data
		
		// Create and initialise EventTrigger.Entry using the created TriggerEvent
		EventTrigger.Entry entry = new EventTrigger.Entry() { callback = trigger, eventID = triggerType };
		
		// Add the EventTrigger.Entry to delegates list on the EventTrigger
		eventTrigger.triggers.Add(entry);
	}
	
	/**
	 * Add listener (event handler) to the object which has EventTrigger component.
	 * Use listener that uses the BaseEventData passed to the Trigger.
	 * @param UnityAction<BaseEventData> action Event handler (lambda or function name) to be called when the event is triggered
	 * @param EventTriggerType triggerType Event to be handled
	 * 
	 * @author AyARL
	 */
	public static void AddEventTrigger(this EventTrigger eventTrigger, UnityAction<BaseEventData> action, EventTriggerType triggerType)
	{
		// Create a nee TriggerEvent and add a listener
		EventTrigger.TriggerEvent trigger = new EventTrigger.TriggerEvent();
		trigger.AddListener((eventData) => action(eventData)); // capture and pass the event data to the listener
		
		// Create and initialise EventTrigger.Entry using the created TriggerEvent
		EventTrigger.Entry entry = new EventTrigger.Entry() { callback = trigger, eventID = triggerType };
		
		// Add the EventTrigger.Entry to delegates list on the EventTrigger
		eventTrigger.triggers.Add(entry);
	}
}

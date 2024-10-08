using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// EventListener - MonoBehaviour that listens to an Event and invokes a UnityEvent in response.
/// </summary>
public class EventListener : MonoBehaviour
{
	[SerializeField] private VoidEvent _event = default; // The Event to Subscribe to.
	public UnityEvent listener; // The UnityEvent to invoke in response to the Event.

	/// <summary>
	/// Subscribe to the Event when this MonoBehaviour is enabled.
	/// </summary>
	private void OnEnable()
	{
		_event?.Subscribe(Respond);
	}

	/// <summary>
	/// UnSubscribe from the Event when this MonoBehaviour is disabled.
	/// </summary>
	private void OnDisable()
	{
		_event?.UnSubscribe(Respond);
	}

	/// <summary>
	/// Response method invoked when the Subscribed Event is raised.
	/// Invokes the UnityEvent to trigger the associated Unity events.
	/// </summary>
	private void Respond()
	{
		listener?.Invoke();
	}
}

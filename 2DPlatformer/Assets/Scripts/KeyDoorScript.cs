using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class KeyDoorPair
{
	public bool KeyGot = false;
	public GameObject Key;
	[Tooltip("Sets keygot to true automatically but doesn't despawn the door. ")]
	public UnityEvent OnKeyTouch;
	public GameObject Door;
	[Tooltip("Only runs if keygot is true. ")]
	public UnityEvent OnDoorTouch;

	public void SetKeyDoors()
	{
		var keyScript = Key.AddComponent(typeof(KeyScript)) as KeyScript;
		var doorScript = Door.AddComponent(typeof(DoorScript)) as DoorScript;
		keyScript.thisPair = this;
		doorScript.thisPair = this;
	}

	public class KeyScript : MonoBehaviour
	{
		[HideInInspector] public KeyDoorPair thisPair;
		private void OnCollisionEnter2D(Collision2D collision)
		{
			OnTriggerEnter2D(collision.collider);
		}
		private void OnTriggerEnter2D(Collider2D collision)
		{
			if (collision.gameObject.tag == "Player")
			{
				thisPair.KeyGot = true;
				thisPair.OnKeyTouch.Invoke();
			}
		}
	}
	public class DoorScript : MonoBehaviour
	{
		[HideInInspector] public KeyDoorPair thisPair;
		private void OnCollisionEnter2D(Collision2D collision)
		{
			OnTriggerEnter2D(collision.collider);
		}
		private void OnTriggerEnter2D(Collider2D collision)
		{
			if (collision.gameObject.tag == "Player" && thisPair.KeyGot)
			{
				thisPair.OnDoorTouch.Invoke();
			}
		}
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour {

    public static GameManager instance = null;
	public KeyDoorPair[] KeyDoors;
    private int score;
    private int health;
    private bool keyCollected;
    private bool doubleJumpEnabled;
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
    private void Awake()
    {
		foreach (var keydoorpair in KeyDoors)
		{
			keydoorpair.SetKeyDoors();
		}
        if(instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        score = 0;
        health = 3;
        keyCollected = false;
        doubleJumpEnabled = false;
    }

    public void AddScore(int points)
    {
        score += points;
    }

    public int GetScore()
    {
        return score;
    }

    public void AddHealth(int hearts)
    {
        health += hearts;
    }

    public void TakeHealth(int hearts)
    {
        health -= hearts;
    }

    public int GetHealth()
    {
        return health;
    }

    public void SetKey(bool keyStatus)
    {
        keyCollected = keyStatus;
        AddScore(1000);
    }

    public void SetDoubleJumpStatus(bool status)
    {
        if(status == true)
        {
            doubleJumpEnabled = true;
            FindObjectOfType<PlayerController>().jumps = 2;
        }
        else
        {
            doubleJumpEnabled = false;
            FindObjectOfType<PlayerController>().jumps = 1;
        }
    }

    public void SetKeyStatus(bool status)
    {
            keyCollected = status;
            FindObjectOfType<PlayerController>().keyCollected = status;
    }
}

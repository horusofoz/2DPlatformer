using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour {

    public static GameManager instance = null;
    private int score;
    private int health;
    private bool keyCollected;
    private bool doubleJumpEnabled;
    public bool attackEnabled;

    private void Awake()
    {
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
        health = 3;
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

    public void SetAttackStatus(bool status)
    {
        attackEnabled = status;
    }

    public void SetKeyStatus(bool status)
    {
            keyCollected = status;
            FindObjectOfType<PlayerController>().keyCollected = status;
    }

    public void StartGame()
    {
        score = 0;
        health = 3;
        keyCollected = false;
        doubleJumpEnabled = false;
        attackEnabled = false;
        MenuController.instance.LoadScene(1);
    }
}

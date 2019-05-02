using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager instance = null;
    private int score;
    private int health;
    private bool keyCollected;

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
        score = 0;
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
}

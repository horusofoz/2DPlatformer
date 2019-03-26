using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager instance = null;
    private int score;
    private int health;

    // Euns before Start
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

    public void addScore(int points)
    {
        score += points;
    }

    public int getScore()
    {
        return score;
    }

    public void addHealth(int hearts)
    {
        health += hearts;
    }

    public void takeHealth(int hearts)
    {
        health -= hearts;
    }

    public int getHealth()
    {
        return health;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

public class EnemyMovementJeff : MonoBehaviour {

    private Rigidbody2D rb;
    public Transform leftLimit;
    public Transform rightLimit;
    public float moveSpeed;

    // Use this for initialization
    void Start ()
    {
        rb = GetComponent<Rigidbody2D>();

        
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void FixedUpdate()
    {
        MoveEnemy();
    }

    private void MoveEnemy()
    {
        // Enemies starts moving from right to left to appear moving towards player initially
        if(transform.position.x >= leftLimit.position.x)
        {
            Debug.Log("Need to walk right to left");
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(leftLimit.position.x, transform.position.y), moveSpeed);
        }
        
        // When enemy reaches leftMovementLimit, Player starts moving to rightMovementLimit
        else if (transform.position.x <= rightLimit.position.x)
        {
            Debug.Log("Need to walk left to right");
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(rightLimit.position.x, transform.position.y), moveSpeed);
        }
    }
}

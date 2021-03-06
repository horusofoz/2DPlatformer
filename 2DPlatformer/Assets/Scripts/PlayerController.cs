﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

public class PlayerController : MonoBehaviour
{
	private Rigidbody2D rb;
    private MenuController mc;

    private bool isWalking
    {
        get
        {
            return GetComponent<Animator>().GetBool("isWalking");
        }
        set
        {
            GetComponent<Animator>().SetBool("isWalking", value);
        }
    }

    private bool isGrounded
    {
        get
        {
            return GetComponent<Animator>().GetBool("isGrounded");
        }
        set
        {
            GetComponent<Animator>().SetBool("isGrounded", value);
        }
    }

    public bool isAttacking
    {
        get
        {
            return _isAttacking;
        }
        set
        {
            _isAttacking = value;
            if (_isAttacking)
            {
                GetComponent<Animator>().SetTrigger("Attack");
            }
        }
    }

    public bool isDead
    {
        get
        {
            return _isDead;
        }
        set
        {
            _isDead = value;
            if (_isDead)
            {
                GetComponent<Animator>().SetTrigger("Die");
            }
        }
    }

    public bool keyCollected = false;
    [Header("Movement")]
    public float speed;
	public bool _isAttacking;
    public bool _isDead;
    private float moveInput;
	[Header("Ground Checking")]
	public Transform feetPos;
	public float checkRadius;
	public LayerMask whatIsGround;
	[Header("Jump Variables")]
	public float jumpForce;
    public float jumpTime;
	private float jumpTimeCounter;
    public int jumps = 1;
    private int jumpCounter;
    [Header("Attack Variables")]
    public float attackInput;
	[Header("Audio")]
	public AudioSource WalkSound;
	public AudioSource JumpSound;
	// CollectionSound can be renamed and changed so that the crystal pickup and key pickup is diferent. 
	public AudioSource CollectionSound;
	public AudioSource DoorOpenSound;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        //mc = GameObject.FindObjectOfType<MenuController>();
        //gameManager = GameObject.FindObjectOfType<GameManager>();

    }

    void FixedUpdate()
    {
        SetPlayerDirection();
        MovePlayerHorizontal();
    }

    void Update()
    {
        ProcessInput();
        CheckIsPlayerGrounded();
    }

    private void MovePlayerHorizontal()
    {
        rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);
        if(rb.velocity.x != 0 && isGrounded == true)
        {
            isWalking = true;
			if (WalkSound.isPlaying == false)
			{
				WalkSound.Play();
			}
        }
        else if (isGrounded == true)
        {
            isWalking = false;
			WalkSound.Stop();
        }
    }

    private void SetPlayerDirection()
    {
        if (moveInput != 0)
        {
            if (moveInput > 0)
            {
                transform.eulerAngles = new Vector3(0, 0, 0);
            }
            else
            {
                transform.eulerAngles = new Vector3(0, 180, 0);
            }
        }


    }
	
	private void ProcessInput()
    {
        moveInput = Input.GetAxisRaw("Horizontal");
        attackInput = Input.GetAxisRaw("Fire1");
        Jump();
        Attack();
    }
    
    private void Jump()
    {
        // If the jump button isn't down this function does nothing, return
        if (!Input.GetButton("Jump"))
        {
            jumpTimeCounter = 0f;
            return;
        }

        // On jump, take one jumpCounter and refil jump time
        // If on ground, refill jumps first
        if (Input.GetButtonDown("Jump"))
        {
            Debug.Log("pressed jump");
            if (isGrounded)
            {
                jumpCounter = jumps;
            }
			if (JumpSound && jumpCounter > 0)
			{
				JumpSound.Play();
			}
            jumpCounter--;
            jumpTimeCounter = jumpTime;
        }
        // If there's jump time left and there are spare jumps, continue adding force
        if (jumpTimeCounter > 0 && jumpCounter >= 0)
        {
            jumpTimeCounter -= Time.deltaTime;
            rb.velocity = Vector2.up * jumpForce;
        }      
        
    }

    private void Attack()
    {
        if(GameManager.instance.attackEnabled == true)
        {
            // If the attack button isn't down or the user is already attacking, this function does nothing, return
            if (Input.GetAxisRaw("Fire1") != 1 || isAttacking == true)
            {
                return;
            }
            isAttacking = true;
        }
    }

    private void CheckIsPlayerGrounded()
    {
        isGrounded = Physics2D.OverlapCircle(feetPos.position, checkRadius, whatIsGround);
    }

    private void OnCollisionEnter2D(Collision2D collider)
    {
        if (collider.gameObject.tag == "enemy" && isAttacking == false)
        {

            GameManager.instance.TakeHealth(1);
            int hearts = GameManager.instance.GetHealth();
            if (hearts <= 0)
            {
                isDead = true;
                MenuController.instance.LoadScene(5);
            }

        }

        if (collider.gameObject.name == "Door Key Parent" && keyCollected == true)
        {
            Debug.LogError("Destroy Door");
            collider.gameObject.SetActive(false);
            GameManager.instance.AddScore(2000);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Finish")
        {
            MenuController.instance.LoadNextScene();
        }

        if (collision.gameObject.name == "Attack Item")
        {
            Debug.Log("Unlocked Attack!");
            GameManager.instance.SetAttackStatus(true);
            collision.gameObject.SetActive(false);
            GameManager.instance.AddScore(1000);
        }

        if (collision.gameObject.name == "Double Jump Item")
        {
            Debug.Log("Unlocked Double Jump!");
            GameManager.instance.SetDoubleJumpStatus(true);
            collision.gameObject.SetActive(false);
            GameManager.instance.AddScore(1000);
        }
        
        if (collision.gameObject.tag == "keyGold")
        {
            collision.gameObject.SetActive(false);
            GameManager.instance.SetKeyStatus(true);
            GameManager.instance.AddScore(2000);
        }

        if (collision.gameObject.tag == "crystal")
        {
			CollectionSound.Play();
            collision.gameObject.SetActive(false);
            GameManager.instance.AddScore(250);
        }

        if (collision.gameObject.tag == "killarea")
        {
            isDead = true;
            MenuController.instance.LoadScene(5);
        }
    }
}
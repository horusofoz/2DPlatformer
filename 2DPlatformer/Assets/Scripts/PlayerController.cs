using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

public class PlayerController : MonoBehaviour
{
	private Rigidbody2D rb;
	[Header("Movement")]
    public float speed;
    private float moveInput;
	[Header("Ground Checking")]
	public Transform feetPos;
	public float checkRadius;
	public LayerMask whatIsGround;
	private bool isGrounded;
	[Header("Jump Variables")]
	public float jumpForce;
    public float jumpTime;
	private float jumpTimeCounter;
    public int jumps = 2;
    private int jumpCounter;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        SetPlayerDirection();
        MovePlayerHorizontal();
    }

    void Update()
    {
        ProcessInput();
    }

    private void MovePlayerHorizontal()
    {
        rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);
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
        Jump();
    }
    
    private void Jump()
    {
        // If the jump button isn't down this function does nothing, return
        if (!Input.GetButton("Jump"))
        {
            jumpTimeCounter = 0f;
            return;
        }

        isGrounded = Physics2D.OverlapCircle(feetPos.position, checkRadius, whatIsGround);
        // On jump, take one jumpCounter and refil jump time
        // If on ground, refill jumps first
        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded)
            {
                jumpCounter = jumps;
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
}
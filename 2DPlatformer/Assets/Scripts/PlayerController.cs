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
	[Obsolete] private bool isJumping;

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
	#region JumpPlayer
	[Obsolete]
	private void JumpPlayer()
	{
		isGrounded = Physics2D.OverlapCircle(feetPos.position, checkRadius, whatIsGround);

		if (isGrounded == true && Input.GetKeyDown(KeyCode.Space))
		{
			isJumping = true;
			rb.velocity = Vector2.up * jumpForce;
			jumpTimeCounter = jumpTime;
		}

		if (Input.GetKey(KeyCode.Space) && isJumping == true)
		{

			if (jumpTimeCounter > 0)
			{
				rb.velocity = Vector2.up * jumpForce;
				jumpTimeCounter -= Time.deltaTime;
			}
			else
			{
				isJumping = false;
			}
		}

		if (Input.GetKeyUp(KeyCode.Space))
		{
			isJumping = false;
		}
	} 
	#endregion

	private void ProcessInput()
    {
        moveInput = Input.GetAxisRaw("Horizontal");
        Jump();
    }
	
    private void Jump()
    {
        isGrounded = Physics2D.OverlapCircle(feetPos.position, checkRadius, whatIsGround);

        // If is ground is true and jump button is pressed, initiate jump
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            rb.velocity = Vector2.up * jumpForce;
            jumpTimeCounter = jumpTime;
			return;
        }
		// If is on the ground, has jump time and is pressing space
		if (!isGrounded && jumpTimeCounter > 0 && Input.GetButton("Jump"))
		{
			rb.velocity = Vector2.up * jumpForce;
			jumpTimeCounter -= Time.deltaTime;
			return;
		}
		// Not pressing jump anymore
		if (Input.GetButtonUp("Jump"))
		{
			jumpTimeCounter = 0;
		}
        //Debug.Log("Jump Axis = " + Input.GetAxisRaw("Jump"));
    }
}
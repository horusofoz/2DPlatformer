using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

public class PlayerController : MonoBehaviour
{

    private Rigidbody2D rb;
    public float speed;
    public float jumpForce;
    private float moveInput;

    private bool isGrounded;
    public Transform feetPos;
    public float checkRadius;
    public LayerMask whatIsGround;

    private float jumpTimeCounter;
    public float jumpTime;
    private bool isJumping;

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

    private void ProcessInput()
    {
        moveInput = Input.GetAxisRaw("Horizontal");
        JumpPlayer();
    }

    private void Jump()
    {
        isGrounded = Physics2D.OverlapCircle(feetPos.position, checkRadius, whatIsGround);

        // If is ground is true and jump button is pressed, initiate jump
        if (isGrounded == true && Input.GetAxis("Jump") > 0)
        {
            isJumping = true;
            rb.velocity = Vector2.up * jumpForce;
            jumpTimeCounter = jumpTime;
        }

        // If player is pressing and jump while is already jumping, initiate Bigger jump
        if (Input.GetAxisRaw("Jump") > 0 && isJumping == true)
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

        // If player is not pressing jump, set isJump to false
        if (Input.GetAxisRaw("Jump") < 1)
        {
            isJumping = false;
        }

        Debug.Log("Jump Axis = " + Input.GetAxisRaw("Jump"));
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

public class PlayerController : MonoBehaviour
{
	private Rigidbody2D rb;

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

    public void SetAttackToFalse()
    {

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
    public int jumps = 2;
    private int jumpCounter;
    [Header("Attack Variables")]
    public float attackInput;


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
        CheckIsPlayerGrounded();
    }

    private void MovePlayerHorizontal()
    {
        rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);
        if(rb.velocity.x != 0 && isGrounded == true)
        {
            isWalking = true;
        }
        else if (isGrounded == true)
        {
            isWalking = false;
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

    private void Attack()
    {
        // If the attack button isn't down or the user is already attacking, this function does nothing, return
        if (Input.GetAxisRaw("Fire1") != 1 || isAttacking == true)
        {
            return;
        }
        isAttacking = true;
    }

    private void CheckIsPlayerGrounded()
    {
        isGrounded = Physics2D.OverlapCircle(feetPos.position, checkRadius, whatIsGround);
    }

    private void OnCollisionEnter2D(Collision2D collider)
    {
        if (collider.gameObject.tag == "enemy" && isAttacking == false)
        {
            isDead = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Finish")
        {
            Debug.Log("Victory!!");
            MenuController mc = new MenuController();
            mc.LoadScene(2);
        }
    }
}
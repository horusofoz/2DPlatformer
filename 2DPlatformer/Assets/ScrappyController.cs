using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrappyController : MonoBehaviour
{
    private Rigidbody2D rb;
    private MenuController mc;
    private Animator anim;
    private BoxCollider2D boxCol;
    private GameManager gm;
    private GameObject swordArm;

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
                GetComponent<Animator>().SetTrigger("attack");
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
                GetComponent<Animator>().SetTrigger("die");
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
    public bool isJumping;
    public float jumpForce;
    public float jumpTime;
    private float jumpTimeCounter;


    [Header("Attack Variables")]
    public float attackInput;
    public GameObject weaponArm;
    public Sprite armSword;
    public Sprite armUnarmed;
    private const int WEAPONUNARMED = 0;
    private const int WEAPONSWORD = 1;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        mc = GameObject.FindObjectOfType<MenuController>();
        anim = GetComponent<Animator>();
        boxCol = GetComponent<BoxCollider2D>();
        gm = GameObject.FindObjectOfType<GameManager>();
        swordArm =  GameObject.Find("scrappy arm left");
    }

    void FixedUpdate()
    {
        SetPlayerDirection();
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
        if (rb.velocity.x != 0 && isGrounded == true)
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
        if (isDead == false)
        {
            moveInput = Input.GetAxisRaw("Horizontal");
            attackInput = Input.GetAxisRaw("Fire1");
            Jump();
            Attack();
        }
        
    }

    private void Jump()
    {
        // If on ground and jump pressed
        if (isGrounded == true && Input.GetButtonDown("Jump"))
        {
            anim.SetTrigger("takeOf");
            isJumping = true;
            jumpTimeCounter = jumpTime;
            rb.velocity = Vector2.up * jumpForce;
        }

        if (isGrounded == true)
        {
            anim.SetBool("isJumping", false);
        }
        else
        {
            anim.SetBool("isJumping", true);
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
        StartCoroutine(EnableWeaponCollider());
        isAttacking = false;
    }

    private IEnumerator Die()
    {
        moveInput = 0;
        attackInput = 0;
        isDead = true;
        yield return new WaitForSeconds(1);
        mc.LoadScene(3);
    }

    private void CheckIsPlayerGrounded()
    {
        isGrounded = Physics2D.OverlapCircle(feetPos.position, checkRadius, whatIsGround);
    }

    private void OnCollisionEnter2D(Collision2D collider)
    {
        if (collider.gameObject.tag == "enemy" && isAttacking == false)
        {
            StartCoroutine(Die());
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Finish")
        {
            Debug.Log("Victory!!");
            mc.LoadScene(2);
            
        }
    }



    private IEnumerator EnableWeaponCollider()
    {
        swordArm.GetComponent<BoxCollider2D>().enabled = true;
        yield return new WaitForSeconds(.5f);
        swordArm.GetComponent<BoxCollider2D>().enabled = false;
    }

}

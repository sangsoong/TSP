using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class PlayerScript : MonoBehaviour
{
    // === Classes ===

    // === Variables ===
    // Components
    private Rigidbody2D rigid2D;
    private BoxCollider2D playerCollider2D;
    private SpriteRenderer spriteRD;
    private Animator animator;

    // Ground Check
    [Header("Ground Check")]
    [SerializeField] private Transform GroundCheckPoint;
    [SerializeField] private LayerMask GroundLayer;
    private float checkHeight = 0.1f;
    private bool isGrounded;
    private bool jumpQueued;

    // Movement
    [Header("Movement")]
    [SerializeField] private float movePower = 1f;
    [SerializeField] private float jumpPower = 5f;
    [SerializeField] private float runCoeff = 2f;
    [SerializeField] private float crouchCoeff = 0.5f;

    // States
    private bool isRunning;
    private bool isCrouching;
    private bool isAttacking;

    // Double-click run
    private KeyCode lastRunKey;
    private float lastRunKeyTime;
    private float runDoubleClickLimit = 0.15f;

    // === Lifecycle ===
    void Start()
    {
        rigid2D = GetComponent<Rigidbody2D>();
        playerCollider2D = GetComponent<BoxCollider2D>();
        spriteRD = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        
    }

    void Update()
    {
        // -- Input: Jump --
        if (isGrounded && Input.GetKeyDown(KeyCode.W) && !isCrouching)
        {
            jumpQueued = true;
        }

        // -- Input: Crouch --
        isCrouching = !isAttacking && isGrounded && Input.GetKey(KeyCode.S);

        // -- Input: Run (double click A/D) --
        if (!isAttacking && !isCrouching && isGrounded
            &&(Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D)))
        {
            KeyCode key = Input.GetKeyDown(KeyCode.A) ? KeyCode.A : KeyCode.D;
            float now = Time.time;
            if (lastRunKey == key && (now - lastRunKeyTime) <= runDoubleClickLimit)
            {
                isRunning = true;
            }
            lastRunKey = key;
            lastRunKeyTime = now;
        }
        // Release run on opposite key or key up
        if (isRunning && (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D)
            || Input.GetKeyDown(KeyCode.A) && lastRunKey == KeyCode.D
            || Input.GetKeyDown(KeyCode.D) && lastRunKey == KeyCode.A))
        {
            isRunning = false;
        }

    }

    void FixedUpdate()
    {
        // -- Ground Check --
        float checkWidth = playerCollider2D.size.x * transform.localScale.x * 5/9f;
        Vector2 boxSize = new Vector2(checkWidth, checkHeight);
        isGrounded = Physics2D.OverlapBox(GroundCheckPoint.position, boxSize, 0f, GroundLayer);
        animator.SetBool("isFlying", !isGrounded);

        // -- Movement: Walk/Run/Crouch --
        Vector2 moveDir = Vector2.zero;
        if (Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D)) moveDir = Vector2.left;
        else if (Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A)) moveDir = Vector2.right;

        // Apply movement state
        if (!isAttacking)
        {
            if (isCrouching)
            {
                animator.SetBool("isRunning", false);
                animator.SetBool("isMoving", false);
                
                animator.SetBool("isCrouching", true);
                animator.SetBool("isCrawling", moveDir != Vector2.zero);
                Vector2 velocity = rigid2D.linearVelocity;
                velocity.x = moveDir.x * movePower * crouchCoeff;
                rigid2D.linearVelocity = velocity;
            }
            else if (isRunning)
            {   
                animator.SetBool("isMoving", false);
                animator.SetBool("isCrouching", false);
                animator.SetBool("isCrawling", false);

                animator.SetBool("isRunning", true);
                Vector2 velocity = rigid2D.linearVelocity;
                velocity.x = moveDir.x * movePower * runCoeff;
                rigid2D.linearVelocity = velocity;
            }
            else
            {
                animator.SetBool("isRunning", false);
                animator.SetBool("isCrouching", false);
                animator.SetBool("isCrawling", false);

                animator.SetBool("isMoving", moveDir != Vector2.zero);
                Vector2 velocity = rigid2D.linearVelocity;
                velocity.x = moveDir.x * movePower;
                rigid2D.linearVelocity = velocity;
            }

            // Flip only when there is input
            if (moveDir.x != 0f)
                spriteRD.flipX = moveDir.x < 0f;
        }

        // -- Jump --
        if (jumpQueued)
        {
            rigid2D.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            jumpQueued = false;
        }
    }
    
    // === Functions ===

}
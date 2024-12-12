using UnityEngine;
using System.Collections;
using UnityEditor.PackageManager;
using System;

public class Player : MonoBehaviour
{
	Rigidbody2D rigid2D;
    BoxCollider2D box2D;
    SpriteRenderer spriteRD;
    Animator animator;

    // Movement
	public float movePower = 1f;
	public float jumpPower = 1f;
    public float runCoeff = 2f;
    public float crouchCoeff = 0.5f;
    Vector3 movement;

    // Fly
    private float airLayExtraHeight = 0.1f;
    bool isFlying = false;

    // Jump
	bool canJump = true;
    bool isJumped = false;

    // Walk
    bool isWalking = true;

    // Run
    bool canRun = true;
    bool isRunning = false;
    KeyCode runDoubleClickKey;
    private bool runDoubleClickChecking = false;
    private float runDoubleClickTime = 0f;
    private float runDoubleClickTimeLimit = 0.2f;

    // Crouch
    bool canCrouch = true;
    bool isCrouching = false;

    // Attack
    private float currentAttackStiff;
    public float attackStiffTime = 1f;
    bool isAttacked = false;
    bool isAttacking = false;
    String currCombo = "";
    private bool comboChecking = false;
    private float comboCheckTime = 0f;
    private float comboCheckTimeLimit = 0.3f;


    void Start()
    {

        rigid2D = gameObject.GetComponent<Rigidbody2D>();
        box2D = gameObject.GetComponent<BoxCollider2D>();
        spriteRD = gameObject.GetComponent<SpriteRenderer>();
        animator = gameObject.GetComponent<Animator>();

    }

    void Update()
    {
        // is in air?
        // ******* 바닥 중심에서 확인해서 끄트머리에 걸치면 공중에뜸 판정 ********* 고쳐야함
        RaycastHit2D rayHitBottom = Physics2D.Raycast(rigid2D.position, Vector3.down, box2D.bounds.extents.y + airLayExtraHeight, LayerMask.GetMask("Standable"));
        if (rayHitBottom.collider != null)
            isFlying = false;
        else
            isFlying = true;
        
        // fly
        if (isFlying)
            animator.SetBool("isFlying", true);
        else
            animator.SetBool("isFlying", false);

        // walk
        if (isAttacking || isRunning || isCrouching)
            isWalking = false;
        else {
            isWalking = true;
        }
        
        // crouch
        if (isAttacking || isFlying)
            canCrouch = false;
        else
            canCrouch = true;
        
        if (canCrouch && Input.GetKey(KeyCode.S))
            isCrouching = true;
        else
            isCrouching = false;

        // run
        Debug.Log(isRunning);
        if (isFlying || isAttacking || isRunning || isCrouching)
            canRun = false;
        else
            canRun = true;
        if (!canRun) {
            runDoubleClickChecking = false;
            runDoubleClickTime = 0;
        }
        if (isAttacking || isCrouching)
            isRunning = false;
        
        if (canRun && runDoubleClickChecking && runDoubleClickKey == KeyCode.A && Input.GetKeyDown(KeyCode.A)
            || canRun && runDoubleClickChecking && runDoubleClickKey == KeyCode.D && Input.GetKeyDown(KeyCode.D)) {   // 더블클릭 체크
            isRunning = true;
            runDoubleClickChecking = false;
            runDoubleClickTime = 0;
        }
        if ((isRunning && runDoubleClickKey == KeyCode.A && Input.GetKeyUp(KeyCode.A))
            || (isRunning && runDoubleClickKey == KeyCode.D && Input.GetKeyUp(KeyCode.D))
            || (isRunning && runDoubleClickKey == KeyCode.A && Input.GetKeyDown(KeyCode.D))
            || (isRunning && runDoubleClickKey == KeyCode.D && Input.GetKeyDown(KeyCode.A)))  // 달리기 해제
            isRunning = false;
        if (canRun && !runDoubleClickChecking) { // 달릴수 있을때 더블클릭 체크 시작
            if (Input.GetKeyDown(KeyCode.A)){
                runDoubleClickChecking = true;
                runDoubleClickKey = KeyCode.A;
            }
            else if (Input.GetKeyDown(KeyCode.D)) {
                runDoubleClickChecking = true;
                runDoubleClickKey = KeyCode.D;
            }
        }
        if (runDoubleClickChecking && (runDoubleClickTime <= runDoubleClickTimeLimit)){   // 더블클릭 시간 증가
            runDoubleClickTime += Time.deltaTime;
        }
        if (runDoubleClickTime > runDoubleClickTimeLimit) { // 더블클릭 제한시간 체크, 초기화
            runDoubleClickChecking = false;
            runDoubleClickTime = 0;
        }

        // jump
        if (!isFlying && !isAttacking)
            canJump = true;
        else
            canJump = false;

        if (canJump && Input.GetKeyDown(KeyCode.W))
            isJumped = true;
        
        // Attack
        if (currentAttackStiff <= 0) {
            currentAttackStiff = 0;
            isAttacking = false;
            if (Input.GetKeyDown(KeyCode.U)) {
                isAttacked = true;
                animator.SetBool("isAttacking", true);
                currentAttackStiff += attackStiffTime;
            }
        }
        else if (currentAttackStiff > 0) {
            currentAttackStiff -= Time.deltaTime;
            isAttacking = true;
        }

    }

    void FixedUpdate()
    {

        Walk();
        Crouch();
        Run();
        Jump();
        Attack();

    }

    void Walk()
    {

        if (!isWalking)
            return;
                
        Vector3 moveVelocity = Vector3.zero;

        if (Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D)) {
            moveVelocity = Vector3.left;
            spriteRD.flipX = true;
            animator.SetBool("isMoving", true);
        }
        else if (!Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D)) {
            moveVelocity = Vector3.right;
            spriteRD.flipX = false;
            animator.SetBool("isMoving", true);
        }
        else
            animator.SetBool("isMoving", false);

        transform.position += moveVelocity * movePower * Time.deltaTime;

    }

    void Crouch()
    {

        if (!isCrouching) {
            animator.SetBool("isCrouching", false);
            animator.SetBool("isCrawling", false);
            return;
        }
        
        Vector3 moveVelocity = Vector3.zero;

        if (Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D)) {
            moveVelocity = Vector3.left;
            spriteRD.flipX = true;
            animator.SetBool("isCrawling", true);
            animator.SetBool("isCrouching", false);
        }
        else if (!Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D)) {
            moveVelocity = Vector3.right;
            spriteRD.flipX = false;
            animator.SetBool("isCrawling", true);
            animator.SetBool("isCrouching", false);
        }
        else {
            animator.SetBool("isCrouching", true);
            animator.SetBool("isCrawling", false);
        }

        transform.position += moveVelocity * movePower * crouchCoeff * Time.deltaTime;

    }

    void Run()
    {

        if (!isRunning){
            animator.SetBool("isRunning", false);
            return;
        }
        
        Vector3 moveVelocity = Vector3.zero;

        if (Input.GetKey(KeyCode.A)) {
            moveVelocity = Vector3.left;
            spriteRD.flipX = true;
            animator.SetBool("isRunning", true);
        }
        else if (Input.GetKey(KeyCode.D)) {
            moveVelocity = Vector3.right;
            spriteRD.flipX = false;
            animator.SetBool("isRunning", true);
        }
        
        transform.position += moveVelocity * movePower * runCoeff * Time.deltaTime;

    }

    void Jump()
    {

        if (!isJumped)
            return;

        rigid2D.linearVelocity = Vector2.zero;

        Vector2 jumpVelocity = new Vector2(0, jumpPower);
        rigid2D.AddForce(jumpVelocity, ForceMode2D.Impulse);

        isJumped = false;

    }

    void Attack()
    {

        if (!isAttacked)
            return;
        
        isAttacked = false;

    }
}

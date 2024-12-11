using UnityEngine;
using System.Collections;
using UnityEditor.PackageManager;

public class Player : MonoBehaviour
{
	Rigidbody2D rigid2D;
    BoxCollider2D box2D;
    SpriteRenderer spriteRD;
    Animator animator;

	public float movePower = 1f;
	public float jumpPower = 1f;

    private float extraHeight = 0.1f;
    Vector3 movement;
    private float currentAttackStiff;
    public float attackStiffTime = 1f;

    bool canMove = true;
	bool canJump = true;
    bool isJumped = false;
    bool isAttacked = false;
    bool isAttacking = false;
    
    void Start()
    {

        rigid2D = gameObject.GetComponent<Rigidbody2D>();
        box2D = gameObject.GetComponent<BoxCollider2D>();
        spriteRD = gameObject.GetComponent<SpriteRenderer>();
        animator = gameObject.GetComponent<Animator>();

    }

    void Update()
    {

        // move
        if (isAttacking)
            canMove = false;
        else
            canMove = true;

        // jump
        RaycastHit2D rayHit = Physics2D.Raycast(rigid2D.position, Vector3.down, box2D.bounds.extents.y + extraHeight, LayerMask.GetMask("Standable"));

        if (rayHit.collider != null && isAttacking == false) {
            canJump = true;
            animator.SetBool("isFlying", false);
        }
        else {
            canJump = false;
            animator.SetBool("isFlying", true);
        }

        if (canJump && Input.GetKeyDown(KeyCode.W)) {
            isJumped = true;
        }
        
        // Attack
        if (currentAttackStiff <= 0) {
            isAttacking = false;
            if (Input.GetKeyDown(KeyCode.U)) {
                isAttacked = true;
                animator.SetBool("isAttacking", true);
                currentAttackStiff = attackStiffTime;
            }
        }
        else if (currentAttackStiff > 0) {
            currentAttackStiff -= Time.deltaTime;
            isAttacking = true;
        }

    }

    void FixedUpdate()
    {

        Move();
        Jump();
        Attack();

    }

    void Move()
    {

        if (!canMove)
            return;
                
        Vector3 moveVelocity = Vector3.zero;

        if (Input.GetKey(KeyCode.A)) {
            moveVelocity = Vector3.left;
            spriteRD.flipX = true;
            animator.SetBool("isMoving", true);
        }
        else if (Input.GetKey(KeyCode.D)) {
            moveVelocity = Vector3.right;
            spriteRD.flipX = false;
            animator.SetBool("isMoving", true);
        }
        else
            animator.SetBool("isMoving", false);

        transform.position += moveVelocity * movePower * Time.deltaTime;

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

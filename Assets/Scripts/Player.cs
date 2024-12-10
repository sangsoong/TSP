using UnityEngine;
using System.Collections;
using UnityEditor.PackageManager;

public class Player : MonoBehaviour
{
	public float movePower = 1f;
	public float jumpPower = 1f;

    private float extraHeight = 0.1f;

	Rigidbody2D rigid2D;
    BoxCollider2D box2D;
    SpriteRenderer spriteRD;
    Animator animator;

    Vector3 movement;
	bool canJump = true;
    bool isJumped = false;
    
    void Start()
    {

        rigid2D = gameObject.GetComponent<Rigidbody2D>();
        box2D = gameObject.GetComponent<BoxCollider2D>();
        spriteRD = gameObject.GetComponent<SpriteRenderer>();
        animator = gameObject.GetComponent<Animator>();

    }

    void Update()
    {

        RaycastHit2D rayHit = Physics2D.Raycast(rigid2D.position, Vector3.down, box2D.bounds.extents.y + extraHeight, LayerMask.GetMask("Standable"));

        if (rayHit.collider != null) {
            canJump = true;
        }
        else
            canJump = false;

        if (canJump && Input.GetButtonDown("Jump"))
            isJumped = true;

    }

    void FixedUpdate()
    {

        Move();
        Jump();

    }

    void Move()
    {

        Vector3 moveVelocity = Vector3.zero;

        if (Input.GetAxisRaw("Horizontal") < 0) {
            moveVelocity = Vector3.left;
            spriteRD.flipX = true;
            animator.SetBool("isMoving", true);
        }
        else if (Input.GetAxisRaw("Horizontal") > 0) {
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
}

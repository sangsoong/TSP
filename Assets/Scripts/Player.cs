using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
	public float movePower = 1f;
	public float jumpPower = 1f;

	Rigidbody2D rigid;

    Vector3 movement;
	bool canJump = true;
    bool isJumped = false;
    
    void Start()
    {

        rigid = gameObject.GetComponent<Rigidbody2D>();

    }

    void Update()
    {

        RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 0.01f, LayerMask.GetMask("Platform"));
        
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
        }
        else if (Input.GetAxisRaw("Horizontal") > 0) {
            moveVelocity = Vector3.right;
        }

        transform.position += moveVelocity * movePower * Time.deltaTime;

    }

    void Jump()
    {

        if (!isJumped)
            return;

        rigid.linearVelocity = Vector2.zero;

        Vector2 jumpVelocity = new Vector2(0, jumpPower);
        rigid.AddForce(jumpVelocity, ForceMode2D.Impulse);

        isJumped = false;

    }
}

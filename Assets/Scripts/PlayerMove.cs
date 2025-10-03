using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    public float maxSpeed;
    public float moveSpeed;
    public float jumpPower;
    public float deceleration = 0.95f;
    Vector2 inputVec;
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator anim;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    void OnMove(InputValue value)
    {
        inputVec = value.Get<Vector2>();
    }
    void OnJump(InputValue value)
    {
        if (value.isPressed && !anim.GetBool("isJumping")) //기본적으로 OnJump는 눌렀을 때 true, 뗐을 때 false 그래서 눌렀을때 점프는 한번만 동작하는 듯? 근데 이 value.isPressed랑 지금 중복 기능인거 같은데 얘로 응용하는게 있는듯
        {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            anim.SetBool("isJumping", true);
        }
    }
    void Update() // 단발적인 키 입력은 그냥 업데이트가 나음 물리라고 해도 왜냐하면 fixed는 1초에 50번 돌고 업데이트는 1초에 컴퓨터 성능에 따라 다르지만 1초에 100번 이상 돌기 때문에
    {
        if (inputVec.x < 0)
            spriteRenderer.flipX = true;
        else if (inputVec.x > 0)
            spriteRenderer.flipX = false;
        // normalized 방향만 남기고 속도 1로 만들기

        //Animation
        if (Mathf.Abs(rigid.linearVelocity.x) < 0.3f)
        {
            anim.SetBool("isWalking", false);
        }
        else
        {
            anim.SetBool("isWalking", true);
        }
    }

    void FixedUpdate()
    {
        //Move by control
        if (inputVec.x != 0)
        {
            // 현재 속도가 최고 속도(maxSpeed)보다 낮을 때만 힘을 가함
            if (Mathf.Abs(rigid.linearVelocity.x) < maxSpeed)
            {
                // Force 모드를 사용해 부드럽게 힘을 줌. moveSpeed는 힘의 크기가 됨.
                // impulse는 순간적으로 힘을 줌
                rigid.AddForce(new Vector2(inputVec.x * moveSpeed, 0), ForceMode2D.Impulse);
            }
            //rigid.linearVelocity = new Vector2(inputVec.x * moveSpeed, rigid.linearVelocity.y);
        }
        // 2. 입력이 없을 때 (키를 뗐을 때) Stop Speed
        else
        {
            // 현재 속도를 점차 줄여나감 (예: 매 프레임 10%씩 감속)
            rigid.linearVelocity = new Vector2(rigid.linearVelocity.x * deceleration, rigid.linearVelocity.y);
        }

        //Landing Platform
        if (rigid.linearVelocity.y < 0) //낙하중일 때만 레이캐스트
        {
            Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0));
            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Platform"));
            if (rayHit.collider != null)
            {
                if (rayHit.distance < 0.5f)
                    anim.SetBool("isJumping", false);
            }
        }
        /*Vector2 nextvec = new Vector2(inputVec.x, 0);
        rigid.AddForce(nextvec, ForceMode2D.Impulse);
        

        if (rigid.linearVelocity.x > maxSpeed) //Right max speed
            rigid.linearVelocity = new Vector2(maxSpeed, rigid.linearVelocity.y);
        else if (rigid.linearVelocity.x < maxSpeed*(-1)) //Left max speed
            rigid.linearVelocity = new Vector2(maxSpeed*(-1), rigid.linearVelocity.y);
        */
        //Velocity 리지드바디의 현재 속도
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            //rigid.linearVelocity.y < 0 && transform.position.y > collision.transform.position.y
            //Damaged
            
            //변경 전
            //OnDamaged(collision.transform.position);

             //변경 후
            Vector2 contactPoint = collision.contacts[0].point;
            OnDamaged(contactPoint);
            

        }
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Item")
        {
            //point

            // Deactive Item
            collision.gameObject.SetActive(false);
        }
    }


    void OnDamaged(Vector2 targetPos)
    {
        //Change Layer (Immortal)
        gameObject.layer = 11;
        //View Alpha
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);

        //Reaction Force
        int dirc = transform.position.x - targetPos.x > 0 ? 1 : -1;
        rigid.AddForce(new Vector2(dirc, 1) * 3, ForceMode2D.Impulse);

        // Animation
        anim.SetTrigger("doDamaged");
        Invoke("offDamaged", 2);
    }

    void offDamaged()
    {
        //Change layer (Normal)
        gameObject.layer = 10;
        spriteRenderer.color = new Color(1, 1, 1, 1);
    }
    
    // Update is called once per frame
}

using UnityEngine;
using System.Collections;

/// <summary>
/// Player의 실제 이동, 점프, 대시 로직을 처리하는 클래스
/// PlayerInput에서 전달받은 입력값을 통해 물리 기반 움직임 제어
/// </summary>
public class PlayerLocomotion : MonoBehaviour
{
    private PlayerInput playerInput;    // PlayerInput 스크립트 참조
    private Rigidbody2D rb;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 4f;

    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckDistance = 0.05f;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.8f, 0.1f);
    [SerializeField] private Vector2 groundCheckOffset = new Vector2(0f, -0.6f);

    [Header("Dash Settings")]
    [SerializeField] private float dashSpeed = 10f;
    [SerializeField] private float dashDuration = 0.2f;

    private bool isDashing = false; // 대시 중 상태 관리

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody2D>();

        playerInput.OnJumpEvent += HandleJump;
        playerInput.OnDashEvent += HandleDash;
    }

    private void OnDestroy()
    {
        // 💡 스크립트 파괴 시 이벤트 구독 해지 (메모리 누수 방지)
        if (playerInput != null)
        {
            playerInput.OnJumpEvent -= HandleJump;
            playerInput.OnDashEvent -= HandleDash;
        }
    }

    private void FixedUpdate()
    {
        // 대시 중이 아닐 때만 일반 이동 처리
        if (!isDashing)
        {
            HandleMovement();
        }
    }

    // 좌우 이동 물리 제어
    private void HandleMovement()
    {
        Vector2 velocity = playerInput.MoveVector * moveSpeed;
        rb.linearVelocity = new Vector2(velocity.x, rb.linearVelocity.y);
    }

    // 점프 물리 제어
    private void HandleJump()
    {
        if (IsGrounded())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    // 지면 접촉 여부 확인
    private bool IsGrounded()
    {
        // BoxCast 위치 계산
        Vector2 position = (Vector2)transform.position + groundCheckOffset;

        // BoxCast를 사용하여 지면 확인
        RaycastHit2D hit = Physics2D.BoxCast(
            position,
            groundCheckSize,
            0f,
            Vector2.down,
            groundCheckDistance,
            groundLayer
        );

        // 지면(충돌체) 감지 시 지면 접촉으로 판단
        return hit.collider != null;
    }
    #region Ground Check Gizmo
    // 지면 감지 BoxCollider 시각화 (디버깅용)
    private void OnDrawGizmos()
    {
        Gizmos.color = IsGrounded() ? Color.green : Color.red;
        Vector2 position = (Vector2)transform.position + groundCheckOffset;
        Gizmos.DrawWireCube(position, groundCheckSize);
    }
    #endregion

    // 대시 물리 제어
    private void HandleDash()
    {
        // 이미 대시 중인 경우 무시
        if (isDashing) return;

        StartCoroutine(DashCoroutine());
    }

    private IEnumerator DashCoroutine()
    {
        isDashing = true;

        // 이동 방향 계산
        float direction;
        if (playerInput.MoveVector.x != 0)
        {
            // 입력이 있을 경우, 입력 방향의 부호(-1 또는 1) 사용
            direction = Mathf.Sign(playerInput.MoveVector.x);
        }
        else
        {
            // 입력이 없을 경우, 플레이어가 현재 바라보는 방향의 부호(-1 또는 1) 사용
            direction = Mathf.Sign(transform.localScale.x); 
        }

        // 기존 속도 저장
        float originalVelocity = rb.linearVelocity.x;

        // 대시 속도 적용
        rb.linearVelocity = new Vector2(direction * dashSpeed, 0f);

        // 대시 지속 시간만큼 기다림
        yield return new WaitForSeconds(dashDuration);
        
        // 대시 종료 후 기존 속도 복원
        rb.linearVelocity = new Vector2(originalVelocity, rb.linearVelocity.y); 

        isDashing = false;
    }
}

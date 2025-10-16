using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Player의 실제 이동, 점프, 대시 로직을 처리하는 클래스
/// PlayerInput에서 전달받은 입력값을 통해 물리 기반 움직임 제어
/// </summary>
public class PlayerLocomotion : MonoBehaviour
{
    private PlayerInput playerInput;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private PlayerDamageStateManager damageStateManager;
    private StatusEffectManager statusManager;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 4f;
    public float MoveSpeedMultiplier { get; set; } = 1f;    // 이동 속도 배율
    private bool isMoving = false;

    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckDistance = 0.05f;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.8f, 0.1f);
    [SerializeField] private Vector2 groundCheckOffset = new Vector2(0f, -0.6f);
    [SerializeField] private int maxJumpCount = 2;
    private int currentJumpCount = 0;
    private bool isJumping = false;

    [Header("Dash Settings")]
    [SerializeField] private float dashSpeed = 10f;
    [SerializeField] private float dashDuration = 0.2f;
    private bool isDashing = false; // 대시 중 상태 관리

    public bool IsMoving => isMoving;
    public bool IsJumping => isJumping;
    public bool IsDashing => isDashing;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>(); 
        damageStateManager = GetComponent<PlayerDamageStateManager>();
        statusManager = GetComponent<StatusEffectManager>();

        // 이벤트 구독
        playerInput.OnJumpEvent += HandleJump;
        playerInput.OnDashEvent += HandleDash;
    }

    private void OnDestroy()
    {
        // 이벤트 구독 해지
        if (playerInput != null)
        {
            playerInput.OnJumpEvent -= HandleJump;
            playerInput.OnDashEvent -= HandleDash;
        }
    }

    private void FixedUpdate()
    {
        // 지면 착지 시 점프 카운트 초기화
        if (IsGrounded())
        {
            // 점프 직후에는 점프 카운트 초기화 x (점프 오작동 방지)
            if (rb.linearVelocity.y <= 0)
            {
                currentJumpCount = 0;
                isJumping = false;
            }
        }
        // 이동 제한 상태인 경우 (일반/피격 상태 제외) 이동 처리 무시
        if (damageStateManager.CurrentDamageState != DamageState.Normal &&
        damageStateManager.CurrentDamageState != DamageState.Hit)
        {
            return;
        }

        // 대시 중이 아닐 때만 일반 이동 처리
        if (!isDashing)
        {
            HandleMovement();
        }

        CheckMovement();
    }
    
    // 현재 이동 상태 확인
    private void CheckMovement()
    {
        // 물리적인 움직임이 있는지 판단
        bool isMovingPhysically = Mathf.Abs(rb.linearVelocity.x) > 0.01f;

        // 대시 중이거나 이동 중인 경우 newMoveState에 true가 할당
        bool newMovementState = isDashing || isMovingPhysically;

        // 현재 이동 상태가 변경된 경우에만 이벤트 발행
        if (newMovementState != isMoving)
        {
            isMoving = newMovementState;
        }
    }

    // 좌우 이동 물리 제어
    private void HandleMovement()
    {
        Vector2 velocity = playerInput.MoveVector * moveSpeed * MoveSpeedMultiplier;
        rb.linearVelocity = new Vector2(velocity.x, rb.linearVelocity.y);

        // 왼쪽 이동 시 flipX를 통해 좌우 반전
        if (playerInput.MoveVector.x < 0)
        {
            spriteRenderer.flipX = true;
        }
        // 오른쪽 이동 시 원본 방향 유지
        else if (playerInput.MoveVector.x > 0)
        {
            spriteRenderer.flipX = false;
        }
    }

    // 점프/2단점프 물리 제어
    private void HandleJump()
    {
        // 골절 시 점프 불가
        if (statusManager.IsFractured) return;

        // 점프 최대 2회 허용
        if (currentJumpCount < maxJumpCount)
        {
            isJumping = true;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            currentJumpCount++;
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
            // 입력이 있을 경우, 입력 방향으로 설정
            direction = Mathf.Sign(playerInput.MoveVector.x);
        }
        else
        {
            // 입력이 없을 경우, 플레이어가 현재 바라보는 방향으로 설정
            // flipX가 true면 왼쪽, false면 오른쪽
            direction = spriteRenderer.flipX ? -1f : 1f;
        }

        // 방향에 따라 좌우 방향 설정
        spriteRenderer.flipX = (direction < 0);

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

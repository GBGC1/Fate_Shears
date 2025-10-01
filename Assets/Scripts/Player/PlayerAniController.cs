using UnityEngine;

/// <summary>
/// 플레이어 애니메이션 상태 전환을 담당하는 클래스
/// PlayerInput 이벤트를 구독하여 애니메이션 설정
/// </summary>
public class PlayerAniController : MonoBehaviour
{
    private Animator animator;
    private PlayerInput playerInput;

    #region Ani Parameter Names
    // const string으로 애니 파라미터 정의
    private const string ANIM_TRIGGER_JUMP = "Jump";
    private const string ANIM_TRIGGER_DASH = "Dash";
    private const string ANIM_BOOL_IS_MOVING = "IsMoving";
    #endregion

    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>();

        if (animator == null || playerInput == null)
        {
            Debug.LogError("필수 컴포넌트를 찾지 못했습니다.");
            enabled = false;
            return;
        }

        // 이벤트 구독
        playerInput.OnJumpEvent += SetJumpAni;
        playerInput.OnDashEvent += SetDashAni;
    }

    private void OnDestroy()
    {
        // 이벤트 구독 해지
        if (playerInput != null)
        {
            playerInput.OnJumpEvent -= SetJumpAni;
            playerInput.OnDashEvent -= SetDashAni;
        }
    }

    private void Update()
    {
        SetMoveAni();
    }

    // 좌우 이동 상태 애니메이션 처리 (SetBool)
    private void SetMoveAni()
    {
        float moveX = playerInput.MoveVector.x;
        bool isMoving = Mathf.Abs(moveX) > 0.01f;

        animator.SetBool(ANIM_BOOL_IS_MOVING, isMoving);
    }

    // 점프 상태 애니메이션 처리 (SetTrigger)
    private void SetJumpAni()
    {
        animator.SetTrigger(ANIM_TRIGGER_JUMP);
    }

    // 대시 상태 애니메이션 처리 (SetTrigger)
    private void SetDashAni()
    {
        animator.SetTrigger(ANIM_TRIGGER_DASH);
    }
}

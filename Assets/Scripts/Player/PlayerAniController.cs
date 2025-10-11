using UnityEngine;

/// <summary>
/// 플레이어 애니메이션 상태 전환을 담당하는 클래스
/// PlayerInput 이벤트를 구독하여 애니메이션 설정
/// </summary>
public class PlayerAniController : MonoBehaviour
{
    private Animator animator;
    private PlayerInput playerInput;
    private StatManager statManager;
    private PlayerDamageStateManager damageStateManager;

    #region Ani Parameter Names
    // const string으로 애니 파라미터 정의
    private const string ANIM_TRIGGER_JUMP = "Jump";
    private const string ANIM_TRIGGER_DASH = "Dash";
    private const string ANIM_TRIGGER_HURT = "Hurt"; 
    private const string ANIM_TRIGGER_DEATH = "Death";
    private const string ANIM_BOOL_IS_MOVING = "IsMoving";
    #endregion

    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>();
        statManager = GetComponent<StatManager>();
        damageStateManager = GetComponent<PlayerDamageStateManager>();

        if (animator == null || playerInput == null || statManager == null || damageStateManager == null)
        {
            Debug.LogError("필수 컴포넌트를 찾지 못했습니다.");
            enabled = false;
            return;
        }

        // 이벤트 구독
        playerInput.OnJumpEvent += SetJumpAni;
        playerInput.OnDashEvent += SetDashAni;
        damageStateManager.OnPlayerHurt += SetHurtAni; 
        statManager.OnDeath += SetDeathAni;
    }

    private void OnDestroy()
    {
        // 이벤트 구독 해지
        if (playerInput != null)
        {
            playerInput.OnJumpEvent -= SetJumpAni;
            playerInput.OnDashEvent -= SetDashAni;
            damageStateManager.OnPlayerHurt -= SetHurtAni; 
            statManager.OnDeath -= SetDeathAni;
        }
    }

    private void Update()
    {
        DamageState currentState = damageStateManager.CurrentDamageState;

        // 움직임이 제한되는 상태에서는 이동 애니메이션 중단
        if (currentState == DamageState.Stunned || currentState == DamageState.Knockback || currentState == DamageState.Dead)
        {
            animator.SetBool(ANIM_BOOL_IS_MOVING, false);
            return;
        }
        
        // 일반/피격 상태일 때만 이동 애니메이션 처리
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

    // 피격 상태 애니메이션 처리 (SetTrigger)
    private void SetHurtAni()
    {
        animator.SetTrigger(ANIM_TRIGGER_HURT);
    }
    
    // 사망 상태 애니메이션 처리 (SetTrigger)
    private void SetDeathAni()
    {
        animator.SetTrigger(ANIM_TRIGGER_DEATH);
        
        // 사망 후 다른 애니메이션이 실행되지 않도록 스크립트 비활성화
        enabled = false; 
    }
}

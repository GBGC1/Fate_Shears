using UnityEngine;
using System;
using System.Collections;

// 플레이어의 피해 관련 상태
public enum DamageState
{
    Normal,      // 일반 상태
    Hit,         // 피격 상태
    Stunned,     // 스턴 상태
    Knockback,   // 넉백 상태
    Dead         // 사망 상태
}

/// <summary>
/// 플레이어의 피격, 스턴, 넉백, 사망 등 피해로 인한 상태 변화 로직 관리
/// StatManager와 이벤트 통신
/// </summary>
public class PlayerDamageStateManager : MonoBehaviour
{
    private StatManager statManager;
    private Rigidbody2D rb;
    private PlayerLocomotion locomotion;
    private SpriteRenderer spriteRenderer;

    [Header("Hit State Settings")]    // 피격
    [SerializeField] private float hitIgnoreDuration = 0.5f;
    [SerializeField] private float hitSlowdownMultiplier = 0.5f;    // 피격 시 적용할 속도 배율 (50%)

    [Header("Stun State Settings")]   // 스턴(기절)
    [SerializeField] private float defaultStunDuration = 1.0f;      // 기본 스턴 지속시간

    [Header("Knockback Settings")]    // 넉백(넘어짐)
    [SerializeField] private float defaultKnockbackForce = 2.0f;    // 기본 넉백 적용 힘
    [SerializeField] private float knockbackDuration = 0.15f;       // 넉백 지속시간
    
    private DamageState currentDamageState = DamageState.Normal;

    // 피격 애니메이션 요청 이벤트
    public event Action OnPlayerHurt;

    #region Properties
    public bool IsInHitState { get; private set; } = false;
    public DamageState CurrentDamageState => currentDamageState;
    #endregion

    private void Awake()
    {
        statManager = GetComponent<StatManager>();
        rb = GetComponent<Rigidbody2D>();
        locomotion = GetComponent<PlayerLocomotion>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (statManager == null || rb == null || locomotion == null || spriteRenderer == null)
        {
            Debug.LogError("필수 컴포넌트를 찾지 못했습니다.");
            enabled = false;
            return;
        }

        // StatManager 이벤트 구독
        statManager.OnHurt += HandleHit; // 피격 발생 시 호출 (HP 감소 후)
        statManager.OnDeath += HandleDeath; 
    }

    private void OnDestroy()
    {
        if (statManager != null)
        {
            statManager.OnHurt -= HandleHit;
            statManager.OnDeath -= HandleDeath;
        }
    }

    // StatManager로부터 HP 감소가 발생했음을 통보받아 피격 상태 시작
    private void HandleHit(StatManager sender)
    {
        // 이미 피격 or 사망 상태인 경우 피해 무시
        if (currentDamageState == DamageState.Dead || IsInHitState)
        {
            return;
        }

        // 피격 상태 처리 시작
        StartCoroutine(ExecuteHitState());
    }

    // 피격 상태를 처리하는 코루틴
    private IEnumerator ExecuteHitState()
    {
        // 피격 상태로 전환
        currentDamageState = DamageState.Hit;
        IsInHitState = true;

        // 피격 시 이동 속도 절반 감소
        locomotion.MoveSpeedMultiplier = hitSlowdownMultiplier;

        // 피격 애니메이션 실행
        OnPlayerHurt.Invoke();

        yield return new WaitForSeconds(hitIgnoreDuration);

        // 피격 상태 지속시간 종료 후 일반 상태로 전환
        currentDamageState = DamageState.Normal;
        IsInHitState = false;

        // 이동 속도 원상 복구
        locomotion.MoveSpeedMultiplier = 1f; 
    }

    // StatManager로부터 사망 이벤트 발생을 통보받아 사망 상태 처리
    private void HandleDeath()
    {
        // 이미 사망 상태인 경우 무시
        if (currentDamageState == DamageState.Dead) return;

        // 모든 코루틴 중지
        StopAllCoroutines(); 

        currentDamageState = DamageState.Dead;

        // Rigidbody2D 물리 정지
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        locomotion.MoveSpeedMultiplier = 1f;    // 이동 속도 복구

        Debug.Log($"{gameObject.name} has died.");
    }
    #region Stun Method
    // 스턴 상태를 시작하는 함수 (적 공격 로직에서 사용)
    public void StartStun(float duration)
    {
        if (currentDamageState == DamageState.Dead || IsInHitState) return;

        StartCoroutine(ExecuteStunState(defaultStunDuration + duration));
    }

    // 스턴 상태를 처리하는 처리하는 코루틴
    private IEnumerator ExecuteStunState(float duration)
    {
        currentDamageState = DamageState.Stunned;

        // 스턴 시 이동 비활성화 (물리 정지)
        rb.linearVelocity = Vector2.zero;

        OnPlayerHurt?.Invoke();     // 피격 애니메이션 실행

        Debug.Log("스턴 상태!");

        yield return new WaitForSeconds(duration);

        // 스턴 상태 해제 후 일반 상태로 전환
        if (currentDamageState != DamageState.Dead)
        {
            currentDamageState = DamageState.Normal;
        }
    }
    #endregion

    #region Knockback Method
    // 넉백 상태를 시작하는 함수 (적 공격 로직에서 사용)
    public void StartKnockback(float forceX)
    {
        if (currentDamageState == DamageState.Dead || IsInHitState) return;

        StartCoroutine(ExecuteKnockbackState(defaultKnockbackForce + forceX));
    }
    
    private IEnumerator ExecuteKnockbackState(float forceX)
    {
        currentDamageState = DamageState.Knockback;
        IsInHitState = true; // 넉백 시 Hit 중복 실행 방지를 위해 true로 설정

        // 플레이어 이동 제어 차단
        locomotion.MoveSpeedMultiplier = 0f;

        // 플레이어가 바라보는 방향의 반대로 넉백 방향 설정
        // (flipX == true : 왼쪽을 보고 있음 (-1) , false : 오른쪽을 보고 있음 (1) )
        float facingDirection = spriteRenderer.flipX ? -1f : 1f;
        float knockbackDirection = -facingDirection;

        // 피격 애니메이션 실행
        OnPlayerHurt?.Invoke();

        // 넉백 힘 적용
        Vector2 knockbackForce = new Vector2(knockbackDirection * forceX, 0f);
        rb.linearVelocity = knockbackForce;

        Debug.Log("넉백 상태!");

        yield return new WaitForSeconds(knockbackDuration);

        // 넉백 상태 종료 및 일반 상태로 복구
        if (currentDamageState != DamageState.Dead)
        {
            // 넉백 힘 제거 및 물리 정지
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

            currentDamageState = DamageState.Normal;

            // 이동 속도 원상 복구
            locomotion.MoveSpeedMultiplier = 1f;
        }
        IsInHitState = false; 
    }
    #endregion
}

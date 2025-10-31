using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private StatManager playerStat;
    [SerializeField] private StatManager enemyStat;
    [SerializeField] private LootDropper lootDropper;

    [Header("Move Settings")]
    [SerializeField] private float moveSpeed = 2f;
    public Transform[] points;

    [Header("Attack Settings")]
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private float attackCooldown = 1f;
    private float lastAttackTime = 0f;
    
    [Header("AI Settings")]
    [SerializeField] private float chaseRange = 10f;    // Move 상태 시작 기준 범위
    [SerializeField] private float attackRange = 5f;    // Attack 상태 시작 기준 범위
    [SerializeField] private Transform playerTransform;

    // 현재 상태 변수
    private EnemyState currentState = EnemyState.Idle;
    private bool isDead = false;

    private int i;
    private SpriteRenderer spriteRenderer;
    private Animator ani;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        ani = GetComponent<Animator>();

        if (playerTransform == null)
        {
            Debug.LogError("Enemy : 플레이어 트랜스폼이 할당되지 않았습니다.");
        }

        if (playerStat == null || enemyStat == null || lootDropper == null)
        {
            Debug.LogError("Enemy : 필수 컴포넌트가 할당되지 않았습니다.");
        }

        ChangeState(EnemyState.Idle);
    }

    void Update()
    {
        if (isDead) return;

        // 상태 전환을 위해 플레이어와의 거리 체크
        float distance = GetDistanceToPlayer();

        EnemyState newState = EnemyStateHandler.CheckAndTransition(
            currentState, distance,
            chaseRange, attackRange);

        // 새로운 상태로의 전환이 필요한 경우 ChangeState 호출
        if (newState != currentState)
        {
            ChangeState(newState);
        }

        // 체력이 0 이하가 되면 Dead 상태로 전환
        if (enemyStat != null && enemyStat.CurrentHP <= 0 && !isDead)
        {
            ChangeState(EnemyState.Dead);
        }

        switch (currentState)
        {
            case EnemyState.Idle:
                // Idle 상태에서는 플랫폼 순찰 이동 중지
                break;
            case EnemyState.Move:
                // MOVE 상태에서는 플랫폼 순찰 이동
                HandlePatrolMovement();
                break;
            case EnemyState.Attack:
                // Attack 상태에서는 플레이어 공격
                HandleAttack();
                break;
            case EnemyState.Hit:
                break;
            case EnemyState.Dead:
                HandleDead();
                break;
        }
    }
    
    private float GetDistanceToPlayer()
    {
        if (playerTransform != null)
        {
            return Vector2.Distance(transform.position, playerTransform.position);
        }
        return Mathf.Infinity; // 플레이어가 없으면 거리를 무한대로 설정
    }

    // 상태 변경 및 애니메이터 트리거 설정
    private void ChangeState(EnemyState newState)
    {
        if (currentState == newState) return;

        if (isDead && newState != EnemyState.Dead) return;

        currentState = newState;

        // 모든 bool 애니메이션 초기화
        ani.SetBool("IsMoving", false);
        ani.SetBool("IsAttacking", false);
        ani.SetBool("IsIdle", false);

        switch (currentState)
        {
            case EnemyState.Idle:
                ani.SetBool("IsIdle", true);
                break;
            case EnemyState.Move:
                ani.SetBool("IsMoving", true);
                break;
            case EnemyState.Attack:
                ani.SetBool("IsAttacking", true);
                break;
            case EnemyState.Hit:
                ani.SetTrigger("Hit"); 
                break;
            case EnemyState.Dead:
                ani.SetTrigger("Dead"); 
                break;
        }
    }

    // Move 상태에서 실행되는 플랫폼 순찰 이동
    private void HandlePatrolMovement()
    {
        if (Vector2.Distance(transform.position, points[i].position) < 0.25f)
        {
            i++;
            if (i == points.Length)
            {
                i = 0;
            }
        }
        transform.position = Vector2.MoveTowards(transform.position, points[i].position, moveSpeed * Time.deltaTime);
        
        // 스프라이트 방향 전환
        spriteRenderer.flipX = (points[i].position.x - transform.position.x) < 0f;
    }

    // Attack 상태 처리
    private void HandleAttack()
    {
        if (playerTransform != null)
        {
            // 플레이어 방향으로 스프라이트 방향 전환하여 플레이어 응시
            spriteRenderer.flipX = (playerTransform.position.x - transform.position.x) < 0f;

            // 공격 쿨다운이 지난 경우 공격 실행 및 플레이어에게 데미지 적용
            if (Time.time - lastAttackTime >= attackCooldown)
            {
                if (playerStat != null)
                {
                    if (playerStat.IsDead)
                    {
                        ChangeState(EnemyState.Idle);
                        return;   
                    }
                    playerStat.TakeDamage(attackDamage);
                    lastAttackTime = Time.time;
                }
            }
        }
    }

    private void HandleDead()
    {
        // 사망 시 이동 중지
        GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;

        // 아이템 드롭 처리
        if (lootDropper != null)
        {
            lootDropper.DropLoot();
        }

        // 플레이어에게 그림자 조각 지급
        PlayerAbility ability = PlayerManager.Instance?.PlayerStat?.GetComponent<PlayerAbility>();
        if (ability != null)
        {
            ability.AddShadowFragments(10);
        }

        isDead = true;
        StartCoroutine(DeactivateEnemy());
    }

    private IEnumerator DeactivateEnemy()
    {
        yield return new WaitForSeconds(2f);

        Destroy(gameObject);
    }

    // Hit 애니메이션 종료 후 거리 기반으로 다음 상태 재확인
    public void EndHitAnimation()
    {
        ChangeState(EnemyStateHandler.CheckAndTransition(
            currentState,
            GetDistanceToPlayer(),
            chaseRange,
            attackRange));
    }
    
    // 외부에서 호출되는 함수 (예시)
    public void ReceiveHit()
    {
        ChangeState(EnemyState.Hit);
    }
}

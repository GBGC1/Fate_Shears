using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private PlayerAniController ani;

    [Header("Attack Settings")]
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private int baseDamage = 10;
    [SerializeField] private float attackCooldown = 0.5f;

    private float lastAttackTime = 0f;
    private bool isAttacking = false;

    private void Awake()
    {
        if (playerInput == null || ani == null)
        {
            Debug.LogError("PlayerAttack : 필수 컴포넌트가 할당되지 않았습니다.");
            enabled = false;
            return;
        }

        playerInput.OnAttackEvent += HandleAttack;
    }

    private void OnDestroy()
    {
        // 이벤트 구독 해제
        if (playerInput != null)
        {
            playerInput.OnAttackEvent -= HandleAttack;
        }
    }

    private void HandleAttack()
    {
        // 쿨다운 중이거나 공격 중일 경우 무시
        if (Time.time - lastAttackTime < attackCooldown || isAttacking)
        {
            return;
        }

        // 공격 실행
        ExecuteAttack();
    }

    // 공격 실행 로직
    private void ExecuteAttack()
    {
        Transform enemy = FindEnemyInAttackRange();

        if (enemy != null)
        {
            StatManager enemyStat = enemy.GetComponent<StatManager>();

            if (enemyStat != null)
            {
                enemyStat.TakeDamage(baseDamage);
            }
        }

        // 쿨다운 시작
        lastAttackTime = Time.time;
    }

    private Transform FindEnemyInAttackRange()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        Transform closestEnemy = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector2.Distance(transform.position, enemy.transform.position);

            if (distance <= attackRange)
            {
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = enemy.transform;
                }
            }
        }
        return closestEnemy;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}

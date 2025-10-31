using UnityEngine;

namespace Script.Enemy // 1. 팀장님의 "팀 구역" 네임스페이스 안에 만듭니다.
{
    /// <summary>
    /// 보스의 '추격' 상태입니다.
    /// 플레이어를 향해 이동하며, 공격 범위 또는 대기 범위에 들어왔는지 확인합니다.
    /// </summary>
    public class BossChaseState : EnemyStateBase
    {
        private BossFunction bossFunction;
        private Transform playerTransform;
        private Rigidbody2D rb;

        private float moveSpeed;
        private float chaseRange;
        private float attackRange;

        public BossChaseState(EnemyStateController controller) : base(controller)
        {

            bossFunction = controller.Function as BossFunction;
            rb = controller.Rigidbody;
            
            
            if (bossFunction != null)
            {
                playerTransform = bossFunction.PlayerTransform;
                moveSpeed = bossFunction.MoveSpeed;
                chaseRange = bossFunction.ChaseRange;
                attackRange = bossFunction.AttackRange;
            }
        }

        public override void Start()
        {
            // 추격 상태에 진입하면, 이동 애니메이션을 켭니다.
            bossFunction?.StartMove();
        }

        public override void Update()
        {
            if (playerTransform == null || bossFunction == null)
            {
                // 플레이어를 잃어버렸거나 툴박스가 없으면 '대기' 상태로 돌아갑니다.
                controller.ChangeState(new BossIdleState(controller));
                return;
            }

            // 플레이어와의 거리를 잽니다.
            float distanceToPlayer = Vector2.Distance(controller.transform.position, playerTransform.position);

            // 1. 플레이어가 '추격 범위' 밖으로 벗어났다면,
            if (distanceToPlayer > chaseRange)
            {
                // "대기 상태"(BossIdleState)로 변경합니다.
                controller.ChangeState(new BossIdleState(controller));
            }
            // 2. 플레이어가 '공격 범위' 안으로 들어왔다면,
            else if (distanceToPlayer < attackRange)
            {
                // "공격 상태"(BossAttackState)로 변경합니다!
                controller.ChangeState(new BossAttackState(controller));
            }
            // 3. 그 외 (추격 범위 안, 공격 범위 밖)
            else
            {
                // 플레이어를 향해 실제로 이동합니다.
                MoveTowardsPlayer();
            }
        }

        public override void End()
        {
            // 추격 상태를 떠날 때, 이동 애니메이션을 끕니다.
            bossFunction?.EndMove();
            
            // 이동을 멈춥니다. (Y축 속도는 유지하여 중력 등은 적용되게 함)
            if (rb != null)
            {
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            }
        }

        /// <summary>
        /// 플레이어 방향으로 보스를 물리적으로 이동시킵니다.
        /// </summary>
        private void MoveTowardsPlayer()
        {
            // 플레이어 방향으로 가는 벡터를 계산합니다 (방향만 필요하므로 normalize)
            Vector2 direction = (playerTransform.position - controller.transform.position).normalized;
            
            // Rigidbody의 속도를 설정하여 이동시킵니다.
            rb.linearVelocity = new Vector2(direction.x * moveSpeed, rb.linearVelocity.y);

            // (선택 사항: 이동 방향에 따라 보스 스프라이트 좌우 반전)
            // if (direction.x < 0)
            //     bossFunction.SpriteRenderer.flipX = true;
            // else if (direction.x > 0)
            //     bossFunction.SpriteRenderer.flipX = false;
        }
    }
}


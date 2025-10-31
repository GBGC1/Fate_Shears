using UnityEngine;

namespace Script.Enemy
{
    /// <summary>
    /// 보스의 '대기' 상태입니다.
    /// 플레이어가 추격 범위(chaseRange) 안에 들어오는지 확인합니다.
    /// </summary>
    public class BossIdleState : EnemyStateBase
    {
        private BossFunction bossFunction;
        private Transform playerTransform;
        private float chaseRange;

        public BossIdleState(EnemyStateController controller) : base(controller)
        {
            // controller.Function은 BaseEnemyFunction 타입이므로 BossFunction으로 캐스팅(형변환)합니다.
            bossFunction = controller.Function as BossFunction;
            
            // 툴박스에서 필요한 데이터(플레이어 위치, 추격 범위)를 미리 꺼내놓습니다.
            if (bossFunction != null)
            {
                playerTransform = bossFunction.PlayerTransform;
                chaseRange = bossFunction.ChaseRange;
            }
        }

        public override void Start()
        {
            // 대기 상태에 진입하면, 혹시 모르니 이동 애니메이션을 끕니다.
            bossFunction?.EndMove();
        }

        public override void Update()
        {
            // 플레이어가 없거나 BossFunction 연결이 잘못되면 아무것도 안 함
            if (playerTransform == null || bossFunction == null) return;

            // 플레이어와의 거리를 잽니다.
            float distanceToPlayer = Vector2.Distance(controller.transform.position, playerTransform.position);

            // 만약 플레이어가 추격 범위(chaseRange) 안에 들어왔다면,
            if (distanceToPlayer < chaseRange)
            {
                // "추격 상태"(BossChaseState)로 변경합니다!
                controller.ChangeState(new BossChaseState(controller));
            }
        }

        public override void End()
        {
            
        }
    }
}


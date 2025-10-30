using UnityEngine;

namespace Script.Enemy // 1. 팀장님의 "팀 구역" 네임스페이스 안에 만듭니다.
{
    /// <summary>
    /// 보스의 '공격' 상태입니다.
    /// 공격 애니메이션을 1회 실행하고, 쿨타임이 지나면 '대기' 상태로 돌아갑니다.
    /// </summary>
    public class BossAttackState : EnemyStateBase
    {
        private BossFunction bossFunction;
        private float attackCooldown;
        private float lastAttackTime; // 마지막 공격 시간을 기록

        public BossAttackState(EnemyStateController controller) : base(controller)
        {
            // 1단계에서 만든 BossFunction(툴박스)을 가져옵니다.
            bossFunction = controller.Function as BossFunction;
            
            // 툴박스에서 공격 쿨타임 데이터를 가져옵니다.
            if (bossFunction != null)
            {
                attackCooldown = bossFunction.AttackCooldown;
            }
        }

        public override void Start()
        {
            // 공격 상태에 진입하면, 즉시 공격 애니메이션을 실행합니다.
            bossFunction?.Attack();
            
            // 현재 시간을 마지막 공격 시간으로 기록합니다.
            lastAttackTime = Time.time;
        }

        public override void Update()
        {
            // (공격 애니메이션이 끝날 때까지 기다리는 로직을 추가할 수도 있습니다)
            
            // 공격 애니메이션(또는 쿨타임)이 끝났는지 확인합니다.
            // 현재 시간이 (마지막 공격 시간 + 쿨타임)보다 커졌다면,
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                // "대기 상태"(BossIdleState)로 변경합니다.
                // (대기 상태는 다시 플레이어의 거리를 확인하여 추격/공격 여부를 결정할 것입니다)
                controller.ChangeState(new BossIdleState(controller));
            }
        }

        public override void End()
        {
            // 이 상태를 떠날 때 특별히 할 일은 없습니다.
        }
    }
} // 2. 네임스페이스 괄호를 닫아줍니다.


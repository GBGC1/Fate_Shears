using UnityEngine;

namespace Script.Enemy
{
    public class EnemyStateAttack : EnemyStateBase
    {
        public EnemyStateAttack(EnemyStateController controller, Transform transform) : base(controller)
        {
        }

        public override void Start()
        {
            CanChangeState = false;
            controller.Function.Attack();

            // 플레이어 싱글톤 클래스를 통해 StatManager에 접근
            if (PlayerManager.Instance != null && PlayerManager.Instance.PlayerStat != null)
            {
                // 적 자신의 공격력만큼 플레이어에게 데미지 적용
                var enemyStat = controller.GetComponent<StatManager>();
                float damageAmount = enemyStat.AttackPower;
                PlayerManager.Instance.PlayerStat.TakeDamage(damageAmount);
            }
        }

        public override void Update()
        {
            controller.ChangeState(new EnemyStateIdle(controller));
        }

        public override void End()
        {
        }
    }
}
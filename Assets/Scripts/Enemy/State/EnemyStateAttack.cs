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
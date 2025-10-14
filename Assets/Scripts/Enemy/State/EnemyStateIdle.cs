using UnityEngine;

namespace Script.Enemy
{
    public class EnemyStateIdle : EnemyStateBase
    {
        private float idleTime = 2f;
        private float timer;

        public EnemyStateIdle(EnemyStateController controller) : base(controller)
        {
        }

        public override void Start()
        {
            timer = 0f;
            controller.Rigidbody.linearVelocity = Vector2.zero;
        }

        public override void Update()
        {
            timer += Time.deltaTime;
            if (timer >= idleTime)
            {
                controller.ChangeState(new EnemyStateMove(controller));
            }
        }
        public override void End()
        {
        }
    }
}
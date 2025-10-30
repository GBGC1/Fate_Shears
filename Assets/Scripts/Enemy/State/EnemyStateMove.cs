using Map;
using UnityEngine;

namespace Script.Enemy
{
    public class EnemyStateMove : EnemyStateBase
    {
        private Vector3 target;


        private float speed = 2f;
        /*
         * 적의 stat 공격력, 이동 속도 등은 StatManager를 사용하면 되는지?
         * StatManager에서 해당 수치들을 어떻게 받아와야 하는지?
         */

        public EnemyStateMove(EnemyStateController controller) : base(controller)
        {
        }

        public override void Start()
        {
            target = controller.GetComponentInParent<SpawnEnemy>().RandomPos();
            controller.Function.StartMove();
        }

        public override void Update()
        {
            MoveRandomPos();
        }

        public override void End()
        {
            controller.Rigidbody.linearVelocity = Vector2.zero;
            controller.Function.EndMove();
        }

        private void MoveRandomPos()
        {
            if (Vector2.Distance(controller.transform.position, target) < 1)
            {
                controller.CanChangeState();
                controller.ChangeState(new EnemyStateIdle(controller));
                return;
            }

            controller.Rigidbody.linearVelocity =
                (target - controller.transform.position).normalized * speed;
        }
    }
}
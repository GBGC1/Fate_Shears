using UnityEngine;

namespace Script.Enemy
{
    public class EnemyStateDeath : EnemyStateBase
    {
        public EnemyStateDeath(EnemyStateController controller) : base(controller)
        {
        }

        public override void Start()
        {
            CanChangeState = false;
            controller.Function.Death();

            // 아이템 드롭 처리
            LootDropper lootDropper = controller.GetComponent<LootDropper>();
            if (lootDropper != null) lootDropper.DropLoot();
            else Debug.LogError("EnemyStateDeath : LootDropper 컴포넌트가 할당되지 않았습니다.");

            // Rigidbody 비활성화 (물리적 동작 중지)
            controller.Rigidbody.linearVelocity = Vector2.zero;
            controller.Rigidbody.simulated = false;
        }

        public override void Update()
        {
        }

        public override void End()
        {
            // 사망 상태 종료 시 몬스터 오브젝트 파괴
            Object.Destroy(controller.gameObject);
        }
    }
}


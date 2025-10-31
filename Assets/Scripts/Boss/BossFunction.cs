using Script.Enemy.Function;
using UnityEngine;

namespace Script.Enemy
{
    /// <summary>
    /// BaseEnemyFunction을 상속받아, 보스 AI에 필요한
    /// 고유 데이터(속도, 범위)와 플레이어 참조를 관리합니다.
    /// 앞으로 만들 모든 보스 상태(State)가 이 스크립트의 데이터를 참조합니다.
    /// </summary>
    public class BossFunction : BaseEnemyFunction
    {
        [Header("보스 AI 설정")]
        [Tooltip("보스가 추격할 대상입니다. Player 오브젝트를 연결해주세요.")]
        [SerializeField] private Transform playerTransform;

        [Header("보스 스탯")]
        [SerializeField] private float moveSpeed = 3f;

        [Header("보스 AI 범위")]
        [Tooltip("이 거리 안으로 플레이어가 들어오면 추격을 시작합니다.")]
        [SerializeField] private float chaseRange = 10f;
        [Tooltip("이 거리 안으로 플레이어가 들어오면 공격을 시작합니다.")]
        [SerializeField] private float attackRange = 2f;

        [Tooltip("공격 후 다음 공격까지의 대기 시간(초)입니다.")]
        [SerializeField] private float attackCooldown = 2f;
        // ------------------------------------

        // --- 다른 스크립트(State)들이 이 값들을 읽을 수 있도록 프로퍼티(get)로 공개합니다 ---

        /// <summary>
        /// 플레이어의 Transform (읽기 전용)
        /// </summary>
        public Transform PlayerTransform => playerTransform;

        /// <summary>
        /// 보스의 이동 속도 (읽기 전용)
        /// </summary>
        public float MoveSpeed => moveSpeed;

        /// <summary>
        /// 보스의 추격 시작 범위 (읽기 전용)
        /// </summary>
        public float ChaseRange => chaseRange;

        /// <summary>
        /// 보스의 공격 시작 범위 (읽기 전용)
        /// </summary>
        public float AttackRange => attackRange;

        /// <summary>
        /// 보스의 공격 쿨타임 (읽기 전용)
        /// </summary>
        public float AttackCooldown => attackCooldown;
        // --------------------------------------
    }
}


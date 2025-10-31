using System.Collections;
using UnityEngine;


namespace Script.Enemy
{
    /// <summary>
    /// EnemyStateController를 상속받아, 보스의 상태(State)를 제어하는 '두뇌' 역할을 합니다.
    /// 보스 전용 상태(BossIdleState)에서 AI를 시작하도록 초기화합니다.
    /// </summary>
    public class BossStateController : EnemyStateController
    {
        private IEnumerator Start()
        {
            // 부모(EnemyStateController)의 EnemyAppearance 코루틴을 실행합니다.
            // (중력으로 떨어지고, 땅에 닿으면 AI 시작)
            yield return StartCoroutine(EnemyAppearance());

            // 보스 전용 상태인 'BossIdleState'로 첫 상태를 지정합니다.
            // (EnemyAppearance에서 설정한 EnemyStateIdle을 덮어씁니다)
            currentState = new BossIdleState(this);
            currentState.Start();
        }
    }
}


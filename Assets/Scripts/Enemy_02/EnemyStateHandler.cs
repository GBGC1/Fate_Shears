using UnityEngine;

// 적 상태 정의
public enum EnemyState
{
    Idle,
    Move,
    Hit,
    Attack,
    Dead
}

public class EnemyStateHandler : MonoBehaviour
{
    public static EnemyState CheckAndTransition(
        EnemyState currentState, float distanceToPlayer, 
        float chaseRange, float attackRange)
    {
        // Dead 상태는 전환 불가
        if (currentState == EnemyState.Dead)
        {
            return EnemyState.Dead;
        }

        // Hit 상태는 외부 요인(피격)에 의해서만 전환되어야 하므로, 
        // 거리 기반 체크에서는 현재 상태를 유지합니다.
        if (currentState == EnemyState.Hit)
        {
            return EnemyState.Hit;
        }
        
        if (distanceToPlayer <= attackRange)
        {
            return EnemyState.Attack;
        }
        else if (distanceToPlayer <= chaseRange)
        {
            return EnemyState.Move;
        }
        else
        {
            return EnemyState.Idle;
        }
    }
}

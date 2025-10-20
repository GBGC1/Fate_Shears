using Map;
using Script.Enemy;
using Script.Enemy.Function;
using UnityEngine;

public class EnemyStateController : MonoBehaviour
{
    public BaseEnemyFunction Function { get; private set; }
    private EnemyStateBase currentState;
    public EnemyStateBase NextState;

    public Rigidbody2D Rigidbody { get; private set; }

    void Awake()
    {
        Function = GetComponent<BaseEnemyFunction>();
        Rigidbody = GetComponent<Rigidbody2D>();

        currentState = new EnemyStateIdle(this);
    }

    void Update()
    {
        Debug.Log(currentState.GetType().Name);
        currentState.Update();
    }

    public void ChangeState(EnemyStateBase stateBase)
    {
        if (currentState.CanChangeState)
        {
            currentState.End();
            currentState = stateBase;
            currentState.Start();
        }
    }

    public void CanChangeState()
    {
        currentState.CanChangeState = true;
    }
}
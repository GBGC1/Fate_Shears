using Script.Enemy;
using Script.Enemy.Function;
using UnityEngine;

public class EnemyStateController : MonoBehaviour
{
    public BaseEnemyFunction Function { get; private set; }
    private EnemyStateBase currentStateBase;

    public Rigidbody2D Rigidbody { get; private set; }

    void Awake()
    {
        Function = GetComponent<BaseEnemyFunction>();
        Rigidbody = GetComponent<Rigidbody2D>();

        currentStateBase = new EnemyStateIdle(this);
    }

    void Update()
    {
        Debug.Log(currentStateBase.GetType().Name);
        currentStateBase.Update();
    }

    public void ChangeState(EnemyStateBase stateBase)
    {
        if (currentStateBase.CanChangeState)
        {
            currentStateBase.End();
            currentStateBase = stateBase;
            currentStateBase.Start();
        }
    }

    public void CanChangeState()
    {
        currentStateBase.CanChangeState = true;
    }
}
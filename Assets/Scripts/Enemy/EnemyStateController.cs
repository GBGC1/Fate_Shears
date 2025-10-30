using System.Collections;
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
    }

    void FixedUpdate()
    {
        currentState?.Update();
    }

    public void ChangeState(EnemyStateBase stateBase)
    {
        if (currentState.CanChangeState)
        {
            currentState.CanChangeState = false;
            currentState.End();
            currentState = stateBase;
            currentState.Start();
        }
    }

    public void CanChangeState()
    {
        currentState.CanChangeState = true;
    }

    public IEnumerator EnemyAppearance()
    {
        Rigidbody.gravityScale = 10;
        while (!IsGrounded())
        {
            yield return null;
        }

        Rigidbody.linearVelocity = Vector2.zero;
        Rigidbody.gravityScale = 0;
        Debug.Log("중력 제거, State 시작");
        currentState = new EnemyStateIdle(this);
    }

    private bool IsGrounded()
    {
        Bounds bounds = GetComponent<Collider2D>().bounds;

        Vector2 origin = new Vector2(bounds.center.x, bounds.min.y);
        Vector2 size = new Vector2(bounds.size.x * 0.9f, 0.05f);
        float distance = 0.05f;

        RaycastHit2D hit = Physics2D.BoxCast(
            origin,
            size,
            0f,
            Vector2.down,
            distance,
            LayerMask.GetMask("Ground")
        );

        return hit.collider != null;
    }
}
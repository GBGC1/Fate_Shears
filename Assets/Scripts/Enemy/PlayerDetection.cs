using Script.Enemy;
using UnityEngine;

public class PlayerDetection : MonoBehaviour
{
    private EnemyStateController controller;

    void Start()
    {
        controller = GetComponentInParent<EnemyStateController>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag.Equals("Player"))
        {
            controller.ChangeState(new EnemyStateAttack(controller, other.transform));
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag.Equals("Player"))
        {
            controller.NextState = new EnemyStateAttack(controller, other.transform);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag.Equals("Player"))
        {
            controller.NextState = null;
        }
    }
}
using UnityEngine;

namespace Script.Enemy.Function
{
    public abstract class BaseEnemyFunction : MonoBehaviour
    {
        protected Animator animator;

        void Awake()
        {
            animator = GetComponent<Animator>();
        }
        
        public virtual void StartMove()
        {
            Debug.Log("StartMove");
            //animator.SetBool("Move",true);
        }

        public virtual void EndMove()
        {
            Debug.Log("EndMove");
            //animator.SetBool("Move",false);
        }

        public abstract void Attack();
        public abstract void Death();

    }
}
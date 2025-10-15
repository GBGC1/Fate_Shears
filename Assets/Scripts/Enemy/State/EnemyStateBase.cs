namespace Script.Enemy
{
    public abstract class EnemyStateBase
    {
        protected EnemyStateController controller;
        public bool CanChangeState { get; set; } = true;
        
        protected EnemyStateBase(EnemyStateController controller)
        {
            this.controller = controller;
        }
        
        public abstract void Start();
        public abstract void Update();
        public abstract void End();
    }
}
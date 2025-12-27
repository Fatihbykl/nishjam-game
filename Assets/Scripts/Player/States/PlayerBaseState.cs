using UnityEngine;

namespace Player.States
{
    public abstract class PlayerBaseState
    {
        protected PlayerStateManager Ctx;

        public PlayerBaseState(PlayerStateManager context)
        {
            this.Ctx = context;
        }

        public abstract void EnterState();
        public abstract void UpdateState();
        public abstract void ExitState();
    }
}

using Helper;
using UnityEngine;

namespace Player.States
{
    public class HeadState : PlayerBaseState
    {
        private PlayerStateManager _ctx;
        public HeadState(PlayerStateManager context) : base(context)
        {
            _ctx = context;
        }

        public override void EnterState()
        {
            Debug.Log("HEAD MODE");
            CameraFadeManager.Instance.TransitionToCamera(Ctx.headCam);
            Ctx.headMesh.GetComponent<SkinnedMeshRenderer>().material = Ctx.bodyDefaultMaterial;
            // reveal enemy here
        }

        public override void UpdateState()
        {
        }

        public override void ExitState()
        {
            // hide enemy here
        }
    }
}

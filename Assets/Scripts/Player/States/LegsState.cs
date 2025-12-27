using Helper;
using UnityEngine;

namespace Player.States
{
    public class LegsState : PlayerBaseState
    {
        private CharacterController _characterController;
        
        public LegsState(PlayerStateManager context) : base(context)
        {
            _characterController = context.GetComponent<CharacterController>();
        }

        public override void EnterState()
        {
            CameraFadeManager.Instance.TransitionToCamera(Ctx.legsCam);
            Ctx._animator.SetBool("WalkingState", true);
            Ctx.legsMesh.GetComponent<SkinnedMeshRenderer>().material = Ctx.bodyDefaultMaterial;
        }

        public override void UpdateState()
        {
            Move();
        }

        public override void ExitState()
        {
            Ctx._animator.SetBool("WalkingState", false);
        }
        
        private void Move()
        {
            Vector2 input = InputHandler.Actions.Player.Move.ReadValue<Vector2>();
            Vector3 forward = Camera.main.transform.forward;
            Vector3 right = Camera.main.transform.right;
            
            forward.y = 0;
            right.y = 0;
            forward.Normalize();
            right.Normalize();
            
            Vector3 move = input.x * right + input.y * forward;

            _characterController.Move(move * Time.deltaTime * Ctx.runSpeed);
            _characterController.Move(Vector3.down * 9.81f * Time.deltaTime);
            RotatePlayer(move);
        }

        private void RotatePlayer(Vector3 target)
        {
            if (target == Vector3.zero)
            {
                Ctx._animator.SetBool("Walking", false);
                return;
            }
            Ctx._animator.SetBool("Walking", true);
            
            Quaternion targetRotation = Quaternion.LookRotation(target);
            Ctx.gameObject.transform.rotation = Quaternion.Slerp(Ctx.gameObject.transform.rotation, targetRotation, Ctx.rotateSpeed * Time.deltaTime);
        }
    }
}

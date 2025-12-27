using Helper;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player.States
{
    public class ArmsState : PlayerBaseState
    {
        public ArmsState(PlayerStateManager context) : base(context) { }

        public override void EnterState()
        {
            Debug.Log("ARMS MODE: Silah aktif. Yavaş hareket.");
            CameraFadeManager.Instance.TransitionToCamera(Ctx.armsCam);
            InputHandler.Actions.Player.Interact.started += Shoot;
            Ctx._animator.SetBool("Shooting", true);
            Ctx.weaponModel.SetActive(true);
            Ctx.armsMesh.GetComponent<SkinnedMeshRenderer>().material = Ctx.bodyDefaultMaterial;
        }

        public override void UpdateState()
        {
            HandleCameraRotation();
        }

        public override void ExitState()
        {
            InputHandler.Actions.Player.Interact.started -= Shoot;
            Ctx._animator.SetBool("Shooting", false);
            Ctx.weaponModel.SetActive(false);
        }
        
        private void HandleCameraRotation()
        {
            Vector2 input = InputHandler.Actions.Player.Look.ReadValue<Vector2>();

            float mouseX = input.x * Ctx.mouseSensitivity * Time.deltaTime; 
            float mouseY = input.y * Ctx.mouseSensitivity * Time.deltaTime;

            Ctx.transform.Rotate(Vector3.up * mouseX);

            Ctx.xRotation -= mouseY;
            Ctx.xRotation = Mathf.Clamp(Ctx.xRotation, -15f, 15f);

            Ctx.cameraFollowRoot.localRotation = Quaternion.Euler(Ctx.xRotation, 0f, 0f);
        }

        void Shoot(InputAction.CallbackContext obj)
        {
            Ctx._animator.SetTrigger("Shoot");
        }

        public void Fire()
        {
            Debug.Log("Shoot");
            RaycastHit hit;
            if (Physics.Raycast(Ctx.armsCam.transform.position, Ctx.armsCam.transform.forward, out hit, 100f, Ctx.enemyLayer))
            {
                if (hit.collider.CompareTag("Enemy"))
                {
                    Debug.Log("Düşman Vuruldu (Stun)!");
                    // Düşman scriptine stun sinyali gönder: hit.collider.GetComponent<EnemyAI>().Stun();
                }
            }
        }
    }
}

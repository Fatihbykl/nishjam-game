using Enemy;
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
            Ctx.crosshairCanvas.SetActive(true);
            ActivateParticles();
        }
        
        private void ActivateParticles()
        {
            Ctx.headBloodParticle.SetActive(true);
            foreach (var particle in Ctx.armsBloodParticles)
            {
                particle.gameObject.SetActive(false);
            }
            Ctx.legsBloodParticle.SetActive(true);
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
            Ctx.crosshairCanvas.SetActive(false);
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
            Ctx.revolverParticle.SetActive(true);
            
            // Mermiyi namludan oluştur
            GameObject bullet = GameObject.Instantiate(Ctx.bulletPrefab, Ctx.firePoint.position, Ctx.firePoint.rotation);
    
            // Mermiye hız ver (Rigidbody üzerinden)
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // Namlunun baktığı yöne (forward) fırlat
                rb.linearVelocity = Ctx.armsCam.transform.forward * Ctx.bulletSpeed;
            }
        }
    }
}

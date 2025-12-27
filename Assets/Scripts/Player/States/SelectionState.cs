using Helper;
using UnityEngine;

namespace Player.States
{
    public class SelectionState : PlayerBaseState
    {
        public SelectionState(PlayerStateManager context) : base(context)
        {
        }

        public override void EnterState()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            CameraFadeManager.Instance.TransitionToCamera(Ctx.selectionCam, OnCameraTransitionFinished);
            Ctx._animator.SetBool("Levitating", true);
        }

        public override void UpdateState()
        {
        }

        private void OnCameraTransitionFinished()
        {
            Ctx.selectionCanvas.SetActive(true);
            SetDissolveMaterials();
            SetVisuals(true, true, true);
        }
        
        public void HighlightPart(BodyPartState part, bool isHovering)
        {
            if (!isHovering)
            {
                SetVisuals(true, true, true);
                return;
            }

            switch (part)
            {
                case BodyPartState.Head:
                    SetVisuals(true, false, false);
                    break;
                case BodyPartState.Arms:
                    SetVisuals(false, true, false);
                    break;
                case BodyPartState.Legs:
                    SetVisuals(false, false, true);
                    break;
            }
        }

        private void SetVisuals(bool head, bool arms, bool legs)
        {
            AnimatePart(Ctx.headMesh, head, Ctx.dissolveDuration);
            AnimatePart(Ctx.armsMesh, arms, Ctx.dissolveDuration);
            AnimatePart(Ctx.legsMesh, legs, Ctx.dissolveDuration);
        }
        
        private void AnimatePart(GameObject meshObj, bool isSelected, float duration)
        {
            Renderer rend = meshObj.GetComponent<Renderer>();

            if (isSelected)
            {
                rend.DODissolveIn(duration);
            }
            else
            {
                rend.DODissolveOut(duration); 
        
                // Seçenek B: Hafif silik/hayalet kalsın istiyorsan (Ghost Mode)
                // rend.DODissolve(0.85f, duration); 
            }
        }

        private void SetDissolveMaterials()
        {
            Ctx.headMesh.GetComponent<SkinnedMeshRenderer>().material = Ctx.bodyDissolveMaterial;
            Ctx.legsMesh.GetComponent<SkinnedMeshRenderer>().material = Ctx.bodyDissolveMaterial;
            Ctx.armsMesh.GetComponent<SkinnedMeshRenderer>().material = Ctx.bodyDissolveMaterial;
        }

        public void SelectPartByIndex(int index)
        {
            switch (index)
            {
                case 1: Ctx.SwitchState(Ctx.headState); break;
                case 2: Ctx.SwitchState(Ctx.armsState); break;
                case 3: Ctx.SwitchState(Ctx.legsState); break;
            }
        }

        public override void ExitState()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Ctx.selectionCanvas.SetActive(false);
            Ctx._animator.SetBool("Levitating", false);
        }
    }
}

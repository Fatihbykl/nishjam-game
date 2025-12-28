using System;
using Helper;
using Unity.Cinemachine;
using UnityEngine;

namespace Player.States
{
    public enum BodyPartState
    {
        Head,
        Arms,
        Legs
    }
    public class PlayerStateManager : MonoBehaviour
    {
        [HideInInspector] public PlayerBaseState _currentState;
        [HideInInspector] public CharacterController _controller;
        [HideInInspector] public Animator _animator;
        public LegsState legsState;
        public HeadState headState;
        public ArmsState armsState;
        public SelectionState selectionState;
        
        [Header("Aiming Settings")]
        public float mouseSensitivity = 100f;
        public Transform cameraFollowRoot;
        public LayerMask enemyLayer;
        [HideInInspector]
        public float xRotation = 0f;
        
        [Header("Weapon Settings")]
        public GameObject bulletPrefab;
        public Transform firePoint;
        public float bulletSpeed = 20f;

        [Header("Components")]
        
        public GameObject weaponModel;
        public GameObject enemyRevealEffect;
        public GameObject selectionCanvas;
        public GameObject crosshairCanvas;

        [Header("Settings")]
        public float runSpeed = 5f;
        public float rotateSpeed = 10f;

        [Header("Cinemachine Cameras")]
        public CinemachineCamera legsCam;
        public CinemachineCamera headCam;
        public CinemachineCamera armsCam;
        public CinemachineCamera selectionCam;
        
        [Header("Body Part Visuals")]
        public float dissolveDuration = 0.4f;
        public GameObject headMesh;
        public GameObject armsMesh;
        public GameObject legsMesh;
        public Material bodyDefaultMaterial;
        public Material bodyFresnelMaterial;
        public Material bodyDissolveMaterial;
        
        [Header("Particles")]
        public GameObject[] armsBloodParticles;
        public GameObject legsBloodParticle;
        public GameObject headBloodParticle;
        public GameObject revolverParticle;

        public void OnHoverHead(bool isHovering) 
        {
            if(_currentState is SelectionState sel) sel.HighlightPart(BodyPartState.Head, isHovering);
        }

        public void OnHoverArms(bool isHovering) 
        {
            if(_currentState is SelectionState sel) sel.HighlightPart(BodyPartState.Arms, isHovering);
        }

        public void OnHoverLegs(bool isHovering) 
        {
            if(_currentState is SelectionState sel) sel.HighlightPart(BodyPartState.Legs, isHovering);
        }
    
        public void OnClickSelectPart(int partIndex)
        {
            if(_currentState is SelectionState sel) sel.SelectPartByIndex(partIndex);
        }

        public void OnFire()
        {
            if (_currentState is ArmsState arms) arms.Fire();
        }

        private void OnEnable()
        {
            InputHandler.Actions.Player.SwitchBody.performed += OnSwitchBodyPerformed;
        }

        private void OnDisable()
        {
            InputHandler.Actions.Player.SwitchBody.performed -= OnSwitchBodyPerformed;
        }

        void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            _controller = GetComponent<CharacterController>();
            _animator = GetComponent<Animator>();
            
            legsState = new LegsState(this);
            headState = new HeadState(this);
            armsState = new ArmsState(this);
            selectionState = new SelectionState(this);

            SwitchState(headState);
        }

        void Update()
        {
            _currentState.UpdateState();
        }
        
        private void OnSwitchBodyPerformed(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            SwitchState(selectionState);
        }

        public void SwitchState(PlayerBaseState newState)
        {
            if (_currentState == newState) return;
            if (_currentState != null) _currentState.ExitState();

            _currentState = newState;
            _currentState.EnterState();
        }
    }
}

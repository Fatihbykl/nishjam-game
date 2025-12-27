using System;
using DG.Tweening;
using Helper;
using UnityEngine;

namespace Player
{
    public class PlayerMovement : MonoBehaviour
    {
        public float torsoSpeed = 5f;
        public float legSpeed = 5f;
        public float rotationSpeed = 5f;
        
        private CharacterController _characterController;
        private float _currentSpeed;

        private void Start()
        {
            _characterController = GetComponent<CharacterController>();
            _currentSpeed = torsoSpeed;
        }

        private void Update()
        {
            Move();
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

            _characterController.Move(move * Time.deltaTime * _currentSpeed);
            _characterController.Move(Vector3.down * 9.81f * Time.deltaTime);
            RotatePlayer(move);
        }

        private void RotatePlayer(Vector3 target)
        {
            if (target == Vector3.zero) return;
            
            Quaternion targetRotation = Quaternion.LookRotation(target);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}

using Assets.CodeBase.Multiplayer;
using Colyseus.Schema;
using System;
using System.Collections.Generic;
using UnityEngine;
using static StringConstants;

namespace Assets.CodeBase.Player
{
    public class PlayerCharacter : Character
    {
        [SerializeField] private Health _health;
        [SerializeField] private Transform _head;
        [SerializeField] private Transform _cameraParent;
        [SerializeField] private Rigidbody _rb;
        [SerializeField] private GroundCheckerBase _groundChecker;
        [SerializeField] private CharacterAnimator _characterAnimator;
        [SerializeField] private Vector2 _xRotationBorder = new Vector2(-60, 89);
        [SerializeField] private float _jumpForce = 10f;
        [SerializeField] private float _jumpDelay = 0.2f;

        private float _inputH;
        private float _inputV;
        private float _rotateY;
        private float _currentRotateX;
        private float _jumpTime;

        private void Start()
        {
            SetCamera();

            _health.SetMax(MaxHealth);
            _health.SetCurrent(MaxHealth);
        }

        private void FixedUpdate()
        {
            Move();
            RotateY();
        }

        public void RotateX(float value)
        {
            _currentRotateX = Mathf.Clamp(_currentRotateX + value, _xRotationBorder.x, _xRotationBorder.y);
            _head.localEulerAngles = new Vector3(_currentRotateX, 0, 0);
        }

        public void Jump()
        {
            if (_groundChecker.IsGrounded == false || TooEarlyToJump())
                return;

            _jumpTime = Time.time;
            _rb.AddForce(0, _jumpForce, 0, ForceMode.VelocityChange);
        }

        private bool TooEarlyToJump()
        {
            return Time.time - _jumpTime < _jumpDelay;
        }

        private void RotateY()
        {
            _rb.angularVelocity = new Vector3(0, _rotateY, 0);

            _rotateY = 0;
        }

        public void SetInput(float h, float v, float rotateY)
        {
            _inputH = h;
            _inputV = v;
            _rotateY += rotateY;
        }

        public void GetMoveInfo(out Vector3 position, out Vector3 velocity, out float rotateX, out float rotateY)
        {
            position = transform.position;
            velocity = _rb.velocity;

            rotateX = _head.localEulerAngles.x;
            rotateY = transform.localEulerAngles.y;
        }

        private void Move()
        {
            //Vector3 direction = new Vector3(_inputH, 0, _inputV).normalized;
            //transform.position += Time.deltaTime * _speed * direction;

            Vector3 velocity = (transform.forward * _inputV + transform.right * _inputH).normalized * Speed;
            velocity.y = _rb.velocity.y;
            Velocity = velocity;
            _rb.velocity = velocity;
        }

        private void SetCamera()
        {
            Camera.main.transform.parent = _cameraParent;
            Camera.main.transform.localPosition = Vector3.zero;
            Camera.main.transform.localEulerAngles = Vector3.zero;
        }

        public void OnPlayerChange(List<DataChange> changes)
        {
            foreach (var change in changes)
            {
                switch (change.Field)
                {
                    case LOSS:
                        MultiplayerManager.Instance.LossCounter.SetPlayerLoss((byte)change.Value);
                        break;
                    case CURRENT_HP:
                        _health.SetCurrent((sbyte)change.Value);
                        break;
                }
            }
        }
    }
}
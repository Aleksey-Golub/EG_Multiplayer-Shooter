using System;
using UnityEngine;

namespace Assets.CodeBase.Player
{
    public class EnemyCharacter : Character
    {
        [SerializeField] private Transform _head;

        private float _velocityMagnitude;
        private Vector3 _headRotation;
        private Vector3 _bodyRotation;

        public Vector3 TargetPosition { get; private set; }

        private void Start()
        {
            TargetPosition = transform.position;
        }

        private void Update()
        {
            Move();
        }

        public void SetMovement(in Vector3 position, in Vector3 velocity, in float averageInterval)
        {
            TargetPosition = position + (velocity * averageInterval);
            _velocityMagnitude = velocity.magnitude;
            Velocity = velocity;
        }

        public void SetSpeed(float speed)
        {
            Speed = speed;
        }

        public void SetRotateX(float value)
        {
            _head.localEulerAngles = new Vector3(value, 0, 0);
        }

        public void SetRotateY(float value)
        {
            transform.localEulerAngles = new Vector3(0, value, 0);
        }

        private void Move()
        {
            if (_velocityMagnitude > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, TargetPosition, _velocityMagnitude * Time.deltaTime);
            }
            else
            {
                transform.position = TargetPosition;
            }
        }
    }
}

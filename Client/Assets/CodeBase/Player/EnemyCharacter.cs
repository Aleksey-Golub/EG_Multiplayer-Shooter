using System;
using UnityEngine;

namespace Assets.CodeBase.Player
{
    public class EnemyCharacter : Character
    {
        [SerializeField] private Health _health;
        [SerializeField] private Transform _head;

        private float _velocityMagnitude;

        public Vector3 TargetPosition { get; private set; }

        public event Action<int> Damaged;

        private void Start()
        {
            TargetPosition = transform.position;
        }

        private void Update()
        {
            Move();
        }

        public void ApplyDamage(int damage)
        {
            _health.ApplyDamage(damage);

            Damaged?.Invoke(damage);
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

        public void SetMaxHP(int value)
        {
            MaxHealth = value;
            _health.SetMax(value);
            _health.SetCurrent(value);
        }

        public void RestoreHp(int newValue)
        {
            _health.SetCurrent(newValue);
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

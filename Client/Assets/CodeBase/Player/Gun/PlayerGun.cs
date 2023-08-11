using System;
using UnityEngine;

namespace Assets.CodeBase.Player
{
    public class PlayerGun : CharacterGunBase
    {
        [SerializeField] private Transform _shootPoint;
        [SerializeField] private float _bulletSpeed = 1f;
        [SerializeField] private float _shootDelay = 0.5f;
        [SerializeField] private int _damage = 1;

        private float _lastShootTime;

        public bool TryShoot(out ShootInfo info)
        {
            info = new ShootInfo();

            if (Time.time - _lastShootTime < _shootDelay)
                return false;

            Vector3 position = _shootPoint.position;
            Vector3 velocity = _shootPoint.forward * _bulletSpeed;

            _lastShootTime = Time.time;
            Bullet bullet = Instantiate(_bulletPrefab, position, _shootPoint.rotation);
            bullet.Init(velocity, _damage);

            InvokeShootHappened();

            info.pX = position.x;
            info.pY = position.y;
            info.pZ = position.z;
            info.dX = velocity.x;
            info.dY = velocity.y;
            info.dZ = velocity.z;

            return true;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.CodeBase.Player
{
    public class EnemyGun : CharacterGunBase
    {
        public void Shoot(Vector3 position, Vector3 velocity)
        {
            Bullet bullet = Instantiate(_bulletPrefab, position, Quaternion.identity);
            bullet.Init(velocity);

            InvokeShootHappened();
        }
    }
}

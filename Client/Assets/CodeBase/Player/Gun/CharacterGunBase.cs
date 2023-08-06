using System;
using UnityEngine;

namespace Assets.CodeBase.Player
{
    public abstract class CharacterGunBase : MonoBehaviour
    {
        [SerializeField] protected Bullet _bulletPrefab;

        public event Action ShootHappened;

        protected void InvokeShootHappened()
        {
            ShootHappened?.Invoke();
        }
    }
}

using System;
using System.Collections;
using UnityEngine;

namespace Assets.CodeBase.Player
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private float _lifeTime = 20f;
        [SerializeField] private Rigidbody _rb;

        private int _damage;

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.TryGetComponent(out EnemyCharacter enemy))
                enemy.ApplyDamage(_damage);

            DeInit();
        }

        public void Init(Vector3 velocity, int damage = 0)
        {
            _rb.velocity = velocity;
            _damage = damage;

            StartCoroutine(DeInitAfterLifeTime());
        }

        private IEnumerator DeInitAfterLifeTime()
        {
            yield return new WaitForSeconds(_lifeTime);

            DeInit();
        }

        private void DeInit()
        {
            Destroy(gameObject);
        }
    }
}

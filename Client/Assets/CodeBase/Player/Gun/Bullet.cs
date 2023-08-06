using System;
using System.Collections;
using UnityEngine;

namespace Assets.CodeBase.Player
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private float _lifeTime = 20f;
        [SerializeField] private Rigidbody _rb;

        private void OnCollisionEnter(Collision collision)
        {
            DeInit();
        }

        public void Init(Vector3 velocity)
        {
            _rb.velocity = velocity;

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

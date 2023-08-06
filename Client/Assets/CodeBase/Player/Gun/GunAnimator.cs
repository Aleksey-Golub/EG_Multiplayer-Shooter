using UnityEngine;

namespace Assets.CodeBase.Player
{
    public class GunAnimator : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private CharacterGunBase _gun;
        
        private const string SHOOT = "Shoot";

        private void OnEnable()
        {
            _gun.ShootHappened += OnShootHappened;
        }
        private void OnDisable()
        {
            _gun.ShootHappened -= OnShootHappened;
        }

        private void OnShootHappened()
        {
            _animator.SetTrigger(SHOOT);
        }
    }
}

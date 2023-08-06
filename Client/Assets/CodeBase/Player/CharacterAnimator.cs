using UnityEngine;

namespace Assets.CodeBase.Player
{
    public class CharacterAnimator : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private GroundCheckerBase _groundChecker;
        [SerializeField] private Character _character;

        private const string GROUNDED = "Grounded";
        public const string SPEED = "Speed";

        private void Update()
        {
            Vector3 localVelocity = _character.transform.InverseTransformVector(_character.Velocity);
            float speed = localVelocity.magnitude / _character.Speed;
            float sign = Mathf.Sign(localVelocity.z);

            _animator.SetFloat(SPEED, speed * sign);
            _animator.SetBool(GROUNDED, _groundChecker.IsGrounded);
        }
    }
}

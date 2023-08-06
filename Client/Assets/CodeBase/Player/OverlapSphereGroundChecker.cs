using System;
using UnityEngine;

namespace Assets.CodeBase.Player
{
    public class OverlapSphereGroundChecker : GroundCheckerBase
    {
        [SerializeField] private Transform _center;
        [SerializeField] private LayerMask _layerMask;
        [SerializeField] private float _radius;
        [SerializeField] private float _coyoteTime = 0.15f;

        private float _flyTimer;

        public override bool IsGrounded { get; protected set; }

        private void Update()
        {
            CalculateIsGrounded();
        }

        private void CalculateIsGrounded()
        {
            if (Physics.CheckSphere(_center.position, _radius, _layerMask))
            {
                IsGrounded = true;
                _flyTimer = 0;
            }
            else
            {
                _flyTimer += Time.deltaTime;
                if (_flyTimer >= _coyoteTime)
                    IsGrounded = false;
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(_center.position, _radius);
        }
#endif
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.CodeBase.Player
{
    public class GunRay : MonoBehaviour
    {
        [SerializeField] private Transform _rayOrigin;
        [SerializeField] private Transform _rayPoint;
        [SerializeField] private LayerMask _layerMask;
        [SerializeField] private float _rayPointSize = 1f;
        [SerializeField] private float _rayMaxDistance = 50f;

        private Transform _camera;

        private void Awake()
        {
            _camera = Camera.main.transform;
        }

        private void Update()
        {
            Ray ray = new Ray(_rayOrigin.position, _rayOrigin.forward);

            if(Physics.Raycast(ray, out RaycastHit hit, _rayMaxDistance, _layerMask, QueryTriggerInteraction.Ignore))
            {
                _rayOrigin.localScale = new Vector3(1,1, hit.distance * 0.5f);
                _rayPoint.position = hit.point;
                float distanceToCamera = Vector3.Distance(_camera.position, hit.point);
                _rayPoint.localScale = distanceToCamera * _rayPointSize * Vector3.one;
            }
        }
    }
}

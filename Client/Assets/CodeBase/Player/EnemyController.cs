using Colyseus.Schema;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.CodeBase.Player
{
    public class EnemyController : MonoBehaviour
    {
        [SerializeField] private EnemyCharacter _character;
        
        private bool _hasUpdate;
        private Vector3 _oldPosition;
        private Vector3 _newPosition;
        private Vector3 _predictedPosition;

        private void Update()
        {
            Vector3 toPos = _hasUpdate ? _predictedPosition : _newPosition;
            Vector3 newPosition = Vector3.Lerp(_character.Position, toPos, 0.5f);
            _character.SetPosition(newPosition);
            _hasUpdate = false;
        }

        public void Initialize()
        {
            _newPosition = _character.Position;
            _oldPosition = _character.Position;
            _predictedPosition = _character.Position;
        }

        public void OnChange(List<DataChange> changes)
        {
            _hasUpdate = true;

            _oldPosition = _newPosition;
            _newPosition = _character.Position;

            foreach (var change in changes)
            {
                switch (change.Field)
                {
                    case "x":
                        _newPosition.x = (float)change.Value;
                        break;
                    case "y":
                        _newPosition.z = (float)change.Value;
                        break;
                    default:
                        Debug.LogWarning($"Data is not handled: {change.Field}");
                        break;
                }
            }
            _predictedPosition = _newPosition + ((_newPosition - _oldPosition) * 0.5f);
        }
    }
}

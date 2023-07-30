using Colyseus.Schema;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.CodeBase.Player
{
    public class EnemyController : MonoBehaviour
    {
        [SerializeField] private EnemyCharacter _character;

        public void OnChange(List<DataChange> changes)
        {
            Vector3 position = _character.transform.position;

            foreach (var change in changes)
            {
                switch (change.Field)
                {
                    case "x":
                        position.x = (float)change.Value;
                        break;
                    case "y":
                        position.z = (float)change.Value;
                        break;
                    default:
                        Debug.LogWarning($"Data is not handled: {change.Field}");
                        break;
                }
            }
            Debug.Log(position);
            _character.SetPosition(position);
        }
    }
}

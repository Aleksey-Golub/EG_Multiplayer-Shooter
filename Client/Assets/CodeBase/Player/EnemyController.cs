using Colyseus.Schema;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using static StringConstants;
using System;
using Assets.CodeBase.Multiplayer;

namespace Assets.CodeBase.Player
{
    public class EnemyController : MonoBehaviour
    {
        [SerializeField] private EnemyCharacter _character;
        [SerializeField] private EnemyGun _gun;
        private MultiplayerManager _multiplayerManager;
        private Multiplayer.Player _player;
        private string _sessionId;
        private float _lastOnChageUpdateTime;
        private readonly Queue<float> _onChangeUpdateIntervals = new Queue<float>(new float[] { 0f, 0f, 0f, 0f, 0f });

        public void Init(Multiplayer.Player player, string currentEnemySessionId)
        {
            _multiplayerManager = MultiplayerManager.Instance;
            _sessionId = currentEnemySessionId;

            _player = player;
            _player.OnChange += OnPlayerChange;
            _character.Damaged += OnCharacterDamaged;
            _character.SetSpeed(_player.speed);
            _character.SetMaxHP(_player.maxHp);
        }

        public void DeInit()
        {
            _player.OnChange -= OnPlayerChange;
            _character.Damaged -= OnCharacterDamaged;

            _multiplayerManager = null;
            Destroy(gameObject);
        }

        public void Shoot(ShootInfo shootInfo)
        {
            Vector3 position = new Vector3(shootInfo.pX, shootInfo.pY, shootInfo.pZ);
            Vector3 velocity = new Vector3(shootInfo.dX, shootInfo.dY, shootInfo.dZ);

            _gun.Shoot(position, velocity);
        }

        private void OnCharacterDamaged(int appliedDamage)
        {
            Dictionary<string, object> data = new Dictionary<string, object>()
            {
                { ID,               _sessionId},
                { APPLIED_DAMAGE,   appliedDamage},
            };

            _multiplayerManager.SendMessage(DAMAGE, data);
        }

        private void OnPlayerChange(List<DataChange> changes)
        {
            UpdateTimeIntervals();

            Vector3 position = _character.TargetPosition;
            Vector3 velocity = _character.Velocity;

            foreach (var change in changes)
            {
                switch (change.Field)
                {
                    case LOSS:
                        MultiplayerManager.Instance.LossCounter.SetEnemyLoss((byte)change.Value);
                        break;
                    case CURRENT_HP:
                        if ((sbyte)change.Value > (sbyte)change.PreviousValue)
                            _character.RestoreHp((sbyte)change.Value);
                        break;
                    case POSITION_X:
                        position.x = (float)change.Value;
                        break;
                    case POSITION_Y:
                        position.y = (float)change.Value;
                        break;
                    case POSITION_Z:
                        position.z = (float)change.Value;
                        break;
                    case VELOCITY_X:
                        velocity.x = (float)change.Value;
                        break;
                    case VELOCITY_Y:
                        velocity.y = (float)change.Value;
                        break;
                    case VELOCITY_Z:
                        velocity.z = (float)change.Value;
                        break;
                    case ROTATE_X:
                        _character.SetRotateX((float)change.Value);
                        break;
                    case ROTATE_Y:
                        _character.SetRotateY((float)change.Value);
                        break;
                    default:
                        Debug.LogWarning($"Data is not handled: {change.Field}");
                        break;
                }
            }

            _character.SetMovement(position, velocity, _onChangeUpdateIntervals.Average());
        }

        private void UpdateTimeIntervals()
        {
            float currentInterval = Time.time - _lastOnChageUpdateTime;
            _lastOnChageUpdateTime = Time.time;
            _onChangeUpdateIntervals.Dequeue();
            _onChangeUpdateIntervals.Enqueue(currentInterval);
        }
    }
}

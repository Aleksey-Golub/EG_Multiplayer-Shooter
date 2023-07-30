using Assets.CodeBase.Player;
using Colyseus;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.CodeBase.Multiplayer
{
    public class MultiplayerManager : ColyseusManager<MultiplayerManager>
    {
        [SerializeField] private GameObject _playerPrefab;
        [SerializeField] private EnemyController _enemyPrefab;

        private ColyseusRoom<State> _room;

        protected override void Awake()
        {
            base.Awake();

            Instance.InitializeClient();
            Connect();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            _room.Leave();
        }

        public void SendMessage(string key, Dictionary<string, object> data)
        {
            _room.Send(key, data);
        }

        private async void Connect()
        {
            _room = await Instance.client.JoinOrCreate<State>("state_handler");

            _room.OnStateChange += OnStateChange;
        }

        private void OnStateChange(State state, bool isFirstState)
        {
            if (isFirstState == false)
                return;

            state.players.ForEach((key, player) => {

                if (key == _room.SessionId)
                    CreatePlayer(player);
                else
                    CreateEnemy(key, player);
            });

            _room.State.players.OnAdd += CreateEnemy;
            _room.State.players.OnRemove += RemoveEnemy;
        }

        private void CreatePlayer(Player player)
        {
            var position = new Vector3(player.x, 0, player.y);

            Instantiate(_playerPrefab, position, Quaternion.identity);
        }

        private void CreateEnemy(string key, Player player)
        {
            var position = new Vector3(player.x, 0, player.y);

            EnemyController enemy = Instantiate(_enemyPrefab, position, Quaternion.identity);
            enemy.Initialize();
            player.OnChange += enemy.OnChange;
        }

        private void RemoveEnemy(string key, Player player)
        {

        }
    }
}
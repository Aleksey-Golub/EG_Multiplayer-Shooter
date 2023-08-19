using Assets.CodeBase.Player;
using Colyseus;
using System.Collections.Generic;
using UnityEngine;
using static StringConstants;

namespace Assets.CodeBase.Multiplayer
{
    public class MultiplayerManager : ColyseusManager<MultiplayerManager>
    {
        [field: SerializeField] public LossCounter LossCounter { get; private set; }
        [field: SerializeField] public SpawnPoints SpawnPoints { get; private set; }
        [SerializeField] public Skins _skins;
        [SerializeField] private PlayerCharacter _playerPrefab;
        [SerializeField] private EnemyController _enemyPrefab;

        private ColyseusRoom<State> _room;
        private readonly Dictionary<string, EnemyController> _enemies = new();

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

        public void SendMessage(string key, string data)
        {
            _room.Send(key, data);
        }

        public string GetClientSessionId()
        {
            return _room.SessionId;
        }

        private async void Connect()
        {
            SpawnPoints.GetPoint(Random.Range(0, SpawnPoints.Count), out Vector3 spawnPosition, out Vector3 spawnRotation);

            Dictionary<string, object> options = new Dictionary<string, object>()
            {
                { SKINS_COUNT, _skins.Count},
                { SPAWN_POINTS_COUNT, SpawnPoints.Count},
                { SPEED, _playerPrefab.Speed},
                { MAX_HP, _playerPrefab.MaxHealth},
                { SPAWN_POSITION_X, spawnPosition.x},
                { SPAWN_POSITION_Y, spawnPosition.y},
                { SPAWN_POSITION_Z, spawnPosition.z},
                { SPAWN_ROTATION_Y, spawnRotation.y},
            };

            _room = await Instance.client.JoinOrCreate<State>("state_handler", options);

            _room.OnStateChange += OnStateChange;
            _room.OnMessage<string>(SHOOT_FORM_SERVER, ReceiveShootInfo);
        }

        private void ReceiveShootInfo(string json)
        {
            ShootInfo shootInfo = JsonUtility.FromJson<ShootInfo>(json);

            string key = shootInfo.key;
            if (_enemies.ContainsKey(key))
            {
                _enemies[key].Shoot(shootInfo);
            }
            else
            {
                Debug.LogWarning($"Enemy with session id: {key} shoot but is missing on the client");
            }
        }

        private void OnStateChange(State state, bool isFirstState)
        {
            if (isFirstState)
            {
                state.players.ForEach((key, player) => {

                    if (key == _room.SessionId)
                        CreatePlayer(player);
                    else
                        CreateEnemy(key, player);
                });

                _room.State.players.OnAdd += CreateEnemy;
                _room.State.players.OnRemove += RemoveEnemy;
            }
        }

        private void CreatePlayer(Player player)
        {
            var position = new Vector3(player.pX, player.pY, player.pZ);
            var rotation = Quaternion.Euler(0, player.rY, 0);

            var playerCharacter = Instantiate(_playerPrefab, position, rotation);
            player.OnChange += playerCharacter.OnPlayerChange;

            _room.OnMessage<int>(RESTART, playerCharacter.GetComponent<Controller>().Restart);

            playerCharacter.GetComponent<SetSkin>().Set(_skins.GetMaterial(player.skin));
        }

        private void CreateEnemy(string key, Player player)
        {
            var position = new Vector3(player.pX, player.pY, player.pZ);

            EnemyController enemy = Instantiate(_enemyPrefab, position, Quaternion.identity);
            enemy.Init(player, key);
            enemy.GetComponent<SetSkin>().Set(_skins.GetMaterial(player.skin));

            _enemies.Add(key, enemy);
        }

        private void RemoveEnemy(string key, Player player)
        {
            if (_enemies.ContainsKey(key))
            {
                var enemy = _enemies[key];
                enemy.DeInit();

                _enemies.Remove(key);
            }
        }
    }
}
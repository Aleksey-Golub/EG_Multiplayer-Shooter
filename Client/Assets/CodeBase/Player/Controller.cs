using Assets.CodeBase.Multiplayer;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static StringConstants;

namespace Assets.CodeBase.Player
{
    public class Controller : MonoBehaviour
    {
        [SerializeField] private float _restartDelay = 3f;
        [SerializeField] private PlayerCharacter _player;
        [SerializeField] private PlayerGun _gun;
        [SerializeField] private float _mouseSensitivity = 10f;
        
        private MultiplayerManager _multiplayerManager;
        private bool _hold = false;

        private void Start()
        {
            _multiplayerManager = MultiplayerManager.Instance;
        }

        private void Update()
        {
            if (_hold)
                return;

            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");

            float mouseX = Input.GetAxisRaw("Mouse X");
            float mouseY = Input.GetAxisRaw("Mouse Y");
            bool isJump = Input.GetKeyDown(KeyCode.Space);
            bool isShoot = Input.GetKey(KeyCode.Mouse0);
            
            _player.SetInput(h, v, mouseX * _mouseSensitivity);
            _player.RotateX(-mouseY * _mouseSensitivity);
            if (isJump)
                _player.Jump();

            if (isShoot && _gun.TryShoot(out ShootInfo shootInfo))
                SendShoot(ref shootInfo);

            SendMove();
        }

        public void Restart(string jsonRestartInfo)
        {
            var restartInfo = JsonUtility.FromJson<RestartInfo>(jsonRestartInfo);
            StartCoroutine(Hold());

            _player.transform.position = new Vector3(restartInfo.x, 0f, restartInfo.z);
            _player.SetInput(0, 0, 0);

            Dictionary<string, object> data = new Dictionary<string, object>()
            {
                { POSITION_X, restartInfo.x},
                { POSITION_Y, 0},
                { POSITION_Z, restartInfo.z},
                { VELOCITY_X, 0},
                { VELOCITY_Y, 0},
                { VELOCITY_Z, 0},
                { ROTATE_X, 0},
                { ROTATE_Y, 0},
            };

            _multiplayerManager.SendMessage(MOVE, data);
        }

        private IEnumerator Hold()
        {
            _hold = true;
            yield return new WaitForSecondsRealtime(_restartDelay);
            _hold = false;
        }

        private void SendShoot(ref ShootInfo shootInfo)
        {
            shootInfo.key = _multiplayerManager.GetClientSessionId();
            string data = JsonUtility.ToJson(shootInfo);
            _multiplayerManager.SendMessage(SHOOT, data);
        }

        private void SendMove()
        {
            _player.GetMoveInfo(out Vector3 position, out Vector3 velocity, out float rotateX, out float rotateY);
            Dictionary<string, object> data = new Dictionary<string, object>() 
            {
                { POSITION_X, position.x},    
                { POSITION_Y, position.y},
                { POSITION_Z, position.z},
                { VELOCITY_X, velocity.x},    
                { VELOCITY_Y, velocity.y},
                { VELOCITY_Z, velocity.z},
                { ROTATE_X, rotateX},
                { ROTATE_Y, rotateY},
            };

            _multiplayerManager.SendMessage(MOVE, data);
        }
    }

    [Serializable]
    public struct ShootInfo
    {
        public string key;

        public float pX;
        public float pY;
        public float pZ;

        public float dX;
        public float dY;
        public float dZ;
    }

    [Serializable]
    public struct RestartInfo
    {
        public float x;
        public float z;
    }
}

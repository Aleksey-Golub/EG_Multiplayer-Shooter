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
        private bool _hideCursor;

        private void Start()
        {
            _multiplayerManager = MultiplayerManager.Instance;

            _hideCursor = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                _hideCursor = !_hideCursor;
                Cursor.lockState = _hideCursor ? CursorLockMode.Locked : CursorLockMode.None;
                Cursor.visible = !_hideCursor;
            }

            if (_hold)
                return;

            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            float mouseX = 0;
            float mouseY = 0;
            bool isShoot = false;

            if (_hideCursor)
            {
                mouseX = Input.GetAxisRaw("Mouse X");
                mouseY = Input.GetAxisRaw("Mouse Y");
                isShoot = Input.GetKey(KeyCode.Mouse0);
            }

            bool isJump = Input.GetKeyDown(KeyCode.Space);
            
            _player.SetInput(h, v, mouseX * _mouseSensitivity);
            _player.RotateX(-mouseY * _mouseSensitivity);
            if (isJump)
                _player.Jump();

            if (isShoot && _gun.TryShoot(out ShootInfo shootInfo))
                SendShoot(ref shootInfo);

            SendMove();
        }

        public void Restart(int spawnIndex)
        {
            _multiplayerManager.SpawnPoints.GetPoint(spawnIndex, out Vector3 respawnPosition, out Vector3 respawnRotation);
            StartCoroutine(Hold());

            _player.transform.position = respawnPosition;
            respawnRotation.x = 0;
            respawnRotation.z = 0;
            _player.transform.eulerAngles = respawnRotation;
            _player.SetInput(0, 0, 0);

            Dictionary<string, object> data = new Dictionary<string, object>()
            {
                { POSITION_X, respawnPosition.x},
                { POSITION_Y, respawnPosition.y},
                { POSITION_Z, respawnPosition.z},
                { VELOCITY_X, 0},
                { VELOCITY_Y, 0},
                { VELOCITY_Z, 0},
                { ROTATION_X, 0},
                { ROTATION_Y, respawnRotation.y},
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
                { ROTATION_X, rotateX},
                { ROTATION_Y, rotateY},
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
}

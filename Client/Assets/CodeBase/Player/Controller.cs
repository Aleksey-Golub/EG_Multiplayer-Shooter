using Assets.CodeBase.Multiplayer;
using System;
using System.Collections.Generic;
using UnityEngine;
using static StringConstants;

namespace Assets.CodeBase.Player
{
    public class Controller : MonoBehaviour
    {
        [SerializeField] private PlayerCharacter _player;
        [SerializeField] private PlayerGun _gun;
        [SerializeField] private float _mouseSensitivity = 10f;
        
        private MultiplayerManager _multiplayerManager;

        private void Start()
        {
            _multiplayerManager = MultiplayerManager.Instance;
        }

        private void Update()
        {
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

        private void SendShoot(ref ShootInfo shootInfo)
        {
            shootInfo.key = _multiplayerManager.GetClientSessionId();
            string data = JsonUtility.ToJson(shootInfo);
            _multiplayerManager.SendMessage("shoot", data);
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

            _multiplayerManager.SendMessage("move", data);
        }
    }

    [System.Serializable]
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

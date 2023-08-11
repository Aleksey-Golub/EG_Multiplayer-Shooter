using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.CodeBase.Player
{
    public class LossCounter : MonoBehaviour
    {
        [SerializeField] private Text _text;

        private int _enemyLoss;
        private int _playerLoss;

        public void SetEnemyLoss(int value)
        {
            _enemyLoss = value;
            UpdateText();
        }

        public void SetPlayerLoss(int value)
        {
            _playerLoss = value;
            UpdateText();
        }

        private void UpdateText()
        {
            _text.text = $"{_playerLoss} : {_enemyLoss}";
        }
    }
}

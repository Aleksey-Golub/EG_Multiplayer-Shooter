using System;
using UnityEngine;

namespace Assets.CodeBase.Player
{
    public class Health : MonoBehaviour
    {
        [SerializeField] private HealthView _view;

        private int _max;
        private int _current;

        public void SetMax(int max)
        {
            _max = max;

            UpdateView();
        }

        public void SetCurrent(int current)
        {
            _current = current;
            UpdateView();
        }

        public void ApplyDamage(int damage)
        {
            _current -= damage;
            UpdateView();
        }

        private void UpdateView()
        {
            _view.UpdateHealth(_max, _current);
        }
    }
}

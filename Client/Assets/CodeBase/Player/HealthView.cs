using UnityEngine;

namespace Assets.CodeBase.Player
{
    public class HealthView : MonoBehaviour
    {
        [SerializeField] private RectTransform _fg;
        
        public void UpdateHealth(int max, int current)
        {
            Vector3 scale = _fg.localScale;
            scale.x = Mathf.Clamp01((float)current / max);
            _fg.localScale = scale;
        }
    }
}

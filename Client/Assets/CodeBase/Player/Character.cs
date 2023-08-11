using UnityEngine;

namespace Assets.CodeBase.Player
{
    public abstract class Character : MonoBehaviour
    {
        [field: SerializeField] public int MaxHealth{ get; protected set; } = 10;
        [field: SerializeField] public float Speed { get; protected set; } = 5f;
        public Vector3 Velocity { get; protected set; }
    }
}

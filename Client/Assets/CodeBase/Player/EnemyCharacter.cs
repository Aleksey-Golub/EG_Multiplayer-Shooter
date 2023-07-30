using UnityEngine;

namespace Assets.CodeBase.Player
{
    public class EnemyCharacter : MonoBehaviour
    {
        public Vector3 Position => transform.position;

        public void SetPosition(Vector3 position)
        {
            transform.position = position;
        }
    }
}

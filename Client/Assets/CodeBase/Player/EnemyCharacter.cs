using UnityEngine;

namespace Assets.CodeBase.Player
{
    public class EnemyCharacter : MonoBehaviour
    {
        public void SetPosition(Vector3 position)
        {
            transform.position = position;
        }
    }
}

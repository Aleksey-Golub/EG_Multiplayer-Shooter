using UnityEngine;

namespace Assets.CodeBase.Player
{
    public class Skins : MonoBehaviour
    {
        [SerializeField] private Material[] _materials;

        public int Count => _materials.Length;

        public Material GetMaterial(int index)
        {
            if (index < 0 || index >= _materials.Length)
            {
                Debug.LogError($"Index is invalid, return first");
                return _materials[0];
            }

            return _materials[index];
        }
    }
}

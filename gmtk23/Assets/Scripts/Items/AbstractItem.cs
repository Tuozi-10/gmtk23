using UnityEngine;

namespace Items
{
    public abstract class AbstractItem : MonoBehaviour
    {
        public Vector2Int m_size = Vector2Int.one;
        public int price = 1;
    }
}
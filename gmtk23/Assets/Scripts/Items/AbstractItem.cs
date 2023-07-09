using UnityEngine;

namespace Items
{
    public abstract class AbstractItem : ScriptableObject, ICatchable
    {
        public Vector2Int m_size = Vector2Int.one;
        public int price = 1;

        public Sprite sprite;
    }
}
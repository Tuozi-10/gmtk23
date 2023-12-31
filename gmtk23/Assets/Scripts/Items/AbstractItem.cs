﻿using System.Collections.Generic;
using UnityEngine;

namespace Items
{
    public abstract class AbstractItem : ScriptableObject, ICatchable
    {
        public Vector2Int m_size = Vector2Int.one;
        public int level = 0;

        public List<Sprite> sprite;
    }
}
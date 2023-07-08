﻿using IAs;
using UnityEngine;

namespace Items
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "ScriptableObjects/Weapon", order = 1)]
    public class Weapon : AbstractItem
    {
        public AI.Jobs AssociatedJob = AI.Jobs.Cac;
        public int damages;

    }
}
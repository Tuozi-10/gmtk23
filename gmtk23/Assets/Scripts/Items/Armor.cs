using IAs;
using UnityEngine;

namespace Items
{
    [CreateAssetMenu(fileName = "Armor", menuName = "ScriptableObjects/Armor", order = 2)]
    public class Armor : AbstractItem
    {
        public AI.Skills m_skill = AI.Skills.Barbarian;
    }
}
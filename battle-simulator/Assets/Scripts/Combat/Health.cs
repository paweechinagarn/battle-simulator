using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace BattleSimulator
{
    public struct Health : IComponentData
    {
        public int Value;

        // Prevent max health to go below 1
        [Min(1)]
        [SerializeField] private int maxValue;
        public int MaxValue
        {
            get { return maxValue; }
            set { maxValue = math.max(1, value); }
        }
    }
}

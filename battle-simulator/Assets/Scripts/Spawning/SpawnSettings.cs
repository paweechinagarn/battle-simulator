using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace BattleSimulator
{
    public struct SpawnSettings : IComponentData
    {
        // Prevent speed to go below 0.1
        [Min(0.1f)]
        [SerializeField] private float gridSize;
        public float GridSize
        {
            get { return gridSize; }
            set { gridSize = math.max(0.1f, value); }
        }
    }
}

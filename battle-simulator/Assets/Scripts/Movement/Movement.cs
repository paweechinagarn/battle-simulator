using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace BattleSimulator
{
    public struct Movement : IComponentData
    {
        // Prevent speed to go below 0.1
        [Min(0.1f)]
        [SerializeField] private float speed;
        public float Speed
        {
            get { return speed; }
            set { speed = math.max(0.1f, value); }
        }
    }
}

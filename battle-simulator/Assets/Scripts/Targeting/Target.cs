using Unity.Entities;
using Unity.Mathematics;

namespace BattleSimulator
{
    public struct Target : IComponentData
    {
        public Entity Entity;
        public float3 Position;
    }
}

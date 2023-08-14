using Unity.Entities;
using Unity.Mathematics;

namespace BattleSimulator
{
    public struct SpawnGrid : IComponentData
    {
        public int Width;
        public int Height;
        public float3 OriginPosition;
    }
}

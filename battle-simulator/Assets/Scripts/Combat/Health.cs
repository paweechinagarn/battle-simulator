using Unity.Entities;

namespace BattleSimulator
{
    public struct Health : IComponentData
    {
        public int Value;
        public int MaxValue;
    }
}

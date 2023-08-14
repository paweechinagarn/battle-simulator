using Unity.Entities;

namespace BattleSimulator
{
    public struct Owner : IComponentData
    {
        public Entity Value;
    }
}

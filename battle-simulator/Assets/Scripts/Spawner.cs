using Unity.Entities;

namespace BattleSimulator
{
    public struct Spawner : IComponentData
    {
        public Entity Prefab;
        public int Amount;
    }
}

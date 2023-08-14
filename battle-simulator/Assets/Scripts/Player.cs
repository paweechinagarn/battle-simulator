using Unity.Entities;

namespace BattleSimulator
{
    public struct Player : IComponentData
    {
        public int Id;
    }

    public struct Player1Tag : IComponentData { }
    public struct Player2Tag : IComponentData { }
}

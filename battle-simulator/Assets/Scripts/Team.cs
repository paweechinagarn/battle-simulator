using Unity.Entities;

namespace BattleSimulator
{
    public struct Team : IComponentData
    {
        public int Id;
    }

    public struct Team1Tag : IComponentData { }
    public struct Team2Tag : IComponentData { }
}

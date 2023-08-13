using Unity.Entities;

namespace BattleSimulator
{
    public struct Attack : IComponentData
    {
        public int Damage;
        public float Speed;
        public float Range;
        public float CooldownTimer;

        public float CooldownTime => 1f / Speed;
    }
}

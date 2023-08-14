namespace BattleSimulator
{
    public struct TeamSelectedEvent : IDomainEvent
    {
        public int PlayerId;
        public int TeamId;
    }
}

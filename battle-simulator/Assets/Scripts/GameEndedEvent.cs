namespace BattleSimulator
{
    public struct GameEndedEvent : IDomainEvent
    {
        public bool IsWon;
    }
}

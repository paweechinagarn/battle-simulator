using Unity.Entities;

namespace BattleSimulator
{
    public struct GameState : IComponentData
    {
        public enum State
        {
            PreGame,
            InGame,
            PostGame
        }

        public State Value;
    }

    public struct PreGameStateTag : IComponentData { }
    public struct InGameStateTag : IComponentData { }
    public struct PostGameStateTag : IComponentData { }
}

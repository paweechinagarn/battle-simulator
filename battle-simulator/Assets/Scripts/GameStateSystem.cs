using Unity.Entities;

namespace BattleSimulator
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class GameStateSystem : SystemBase, IDomainEventHandler<GameStartedEvent>
    {
        protected override void OnCreate()
        {
            RequireForUpdate<GameState>();
        }

        protected override void OnStartRunning()
        {
            DomainEvents.RegisterDomainEventHandler(this);

            var gameState = SystemAPI.GetSingleton<GameState>();
            ChangeGameState(gameState.Value);
        }

        protected override void OnStopRunning()
        {
            DomainEvents.UnregisterDomainEventHandler(this);
        }

        protected override void OnUpdate()
        {
        }

        private void ChangeGameState(GameState.State newGameState)
        {
            var entity = SystemAPI.GetSingletonEntity<GameState>();
            var gameState = SystemAPI.GetSingleton<GameState>();

            var commandBufferSystem = SystemAPI.GetSingleton<EndInitializationEntityCommandBufferSystem.Singleton>();
            var commandBuffer = commandBufferSystem.CreateCommandBuffer(World.Unmanaged);

            switch (newGameState)
            {
                case GameState.State.PreGame:
                    commandBuffer.AddComponent<PreGameStateTag>(entity);
                    commandBuffer.RemoveComponent<InGameStateTag>(entity);
                    commandBuffer.RemoveComponent<PostGameStateTag>(entity);
                    break;
                case GameState.State.InGame:
                    commandBuffer.RemoveComponent<PreGameStateTag>(entity);
                    commandBuffer.AddComponent<InGameStateTag>(entity);
                    commandBuffer.RemoveComponent<PostGameStateTag>(entity);
                    break;
                case GameState.State.PostGame:
                    commandBuffer.RemoveComponent<PreGameStateTag>(entity);
                    commandBuffer.RemoveComponent<InGameStateTag>(entity);
                    commandBuffer.AddComponent<PostGameStateTag>(entity);
                    break;
            }

            gameState.Value = newGameState;
        }

        public void Handle(GameStartedEvent evt)
        {
            ChangeGameState(GameState.State.InGame);
        }
    }
}

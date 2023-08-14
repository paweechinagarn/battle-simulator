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
            ChangeGameState(gameState.Value, true);
        }

        protected override void OnStopRunning()
        {
            DomainEvents.UnregisterDomainEventHandler(this);
        }

        protected override void OnUpdate()
        {
            var gameState = SystemAPI.GetSingleton<GameState>();
            if (gameState.Value == GameState.State.InGame)
            {
                if (CheckGameEnds())
                {
                    UnityEngine.Debug.Log($"Game ends.");
                    ChangeGameState(GameState.State.PostGame, false);
                }
            }
        }

        private bool CheckGameEnds()
        {
            var player1EntityQuery = SystemAPI.QueryBuilder()
                .WithAll<Player1Tag>()
                .Build();

            var player2EntityQuery = SystemAPI.QueryBuilder()
                .WithAll<Player2Tag>()
                .Build();

            return player1EntityQuery.IsEmpty || player2EntityQuery.IsEmpty;
        }

        private void ChangeGameState(GameState.State newGameState, bool forceUpdate)
        {
            var entity = SystemAPI.GetSingletonEntity<GameState>();
            var gameState = SystemAPI.GetSingleton<GameState>();

            if (gameState.Value == newGameState && !forceUpdate)
                return;

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

            commandBuffer.SetComponent(entity, new GameState { Value = newGameState });
        }

        public void Handle(GameStartedEvent evt)
        {
            ChangeGameState(GameState.State.InGame, false);
        }
    }
}

using Unity.Entities;

namespace BattleSimulator
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class GameStateSystem : SystemBase, 
        IDomainEventHandler<StartGameRequestEvent>,
        IDomainEventHandler<PreGameRequestEvent>
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
                var player1EntityQuery = SystemAPI.QueryBuilder()
                    .WithAll<Player1Tag>()
                    .Build();

                var player2EntityQuery = SystemAPI.QueryBuilder()
                    .WithAll<Player2Tag>()
                    .Build();

                if (player1EntityQuery.IsEmpty || player2EntityQuery.IsEmpty)
                {
                    UnityEngine.Debug.Log($"Game ends.");
                    ChangeGameState(GameState.State.PostGame, false);

                    var isWon = !player1EntityQuery.IsEmpty && player2EntityQuery.IsEmpty;
                    DomainEvents.Raise(new GameEndedEvent
                    {
                        IsWon = isWon
                    });
                }
            }
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

        public void Handle(StartGameRequestEvent evt)
        {
            ChangeGameState(GameState.State.InGame, false);
        }

        public void Handle(PreGameRequestEvent evt)
        {
            ChangeGameState(GameState.State.PreGame, false);
            DomainEvents.Raise(new PreGameEvent());
        }
    }
}

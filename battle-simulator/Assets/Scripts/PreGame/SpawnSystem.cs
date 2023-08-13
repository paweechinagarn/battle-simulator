using System.Runtime.InteropServices;
using Unity.Entities;
using Unity.Transforms;

namespace BattleSimulator
{
    public partial struct SpawnSystem : ISystem, ISystemStartStop, IDomainEventHandler<TeamSelectedEvent>
    {
        [StructLayout(LayoutKind.Auto)]
        private partial struct SpawnJob : IJobEntity
        {
            public float DeltaTime;

            public void Execute(ref LocalTransform transform, in MovementSpeed movementSpeed)
            {
                //var direction = new float3(1f, 0f, 0f);
                //transform = transform.Translate(direction * movementSpeed.Value * DeltaTime);
            }
        }

        private bool enabled;
        private uint index;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PreGameStateTag>();
            state.RequireForUpdate<Spawner>();
            enabled = true;
        }

        public void OnStartRunning(ref SystemState state)
        {
            DomainEvents.RegisterDomainEventHandler(this);
        }

        public void OnStopRunning(ref SystemState state)
        {
            DomainEvents.UnregisterDomainEventHandler(this);
        }

        public void OnUpdate(ref SystemState state)
        {
            if (!enabled)
                return;

            enabled = false;

            var commandBufferSystem = SystemAPI.GetSingleton<EndInitializationEntityCommandBufferSystem.Singleton>();
            var commandBuffer = commandBufferSystem.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();

            foreach (var spawner in SystemAPI.Query<Spawner>().WithChangeFilter<Spawner>())
            {
                //var config = spawner.Config;

                //for (var i = 0; i < 2; i++)
                //{
                //    var instance = commandBuffer.Instantiate(i, spawner.Prefab);
                //    var random = Random.CreateFromIndex(index++);

                //    commandBuffer.SetComponent(i, instance, LocalTransform.Identity.WithPosition(random.NextInt(-3, 3), 1f, random.NextInt(-3, 3)));

                //    commandBuffer.SetComponent(i, instance, new MovementSpeed
                //    {
                //        Value = random.NextFloat(1f, 5f)
                //    });
                //}
            }
        }

        public void Handle(TeamSelectedEvent evt)
        {
            UnityEngine.Debug.Log($"Select team {evt.Id}");
        }
    }
}

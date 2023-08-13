using System.Runtime.InteropServices;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace BattleSimulator
{
    [BurstCompile]
    public partial struct SpawnSystem : ISystem
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

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<Spawner>();
            enabled = true;
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            if (!enabled)
                return;

            enabled = false;

            var commandBufferSystem = SystemAPI.GetSingleton<EndInitializationEntityCommandBufferSystem.Singleton>();
            var commandBuffer = commandBufferSystem.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();

            foreach (var spawner in SystemAPI.Query<RefRO<Spawner>>())
            {
                var prefab = spawner.ValueRO.Prefab;

                for (var i = 0; i < spawner.ValueRO.Amount; i++)
                {
                    var instance = commandBuffer.Instantiate(i, prefab);
                    var random = Random.CreateFromIndex(index++);

                    commandBuffer.SetComponent(i, instance, LocalTransform.Identity.WithPosition(random.NextInt(-3, 3), 1f, random.NextInt(-3, 3)));

                    commandBuffer.SetComponent(i, instance, new MovementSpeed
                    {
                        Value = random.NextFloat(1f, 5f)
                    });
                }
            }
        }
    }
}

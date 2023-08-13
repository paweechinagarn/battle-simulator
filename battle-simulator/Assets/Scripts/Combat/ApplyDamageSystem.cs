using Unity.Entities;
using Unity.Mathematics;

namespace BattleSimulator
{
    [UpdateAfter(typeof(AttackSystem))]
    public partial struct ApplyDamageSystem : ISystem
    {
        private int index;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<InGameStateTag>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var commandBufferSystem = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            var commandBuffer = commandBufferSystem.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();

            foreach (var (damageBuffer, health, entity) in SystemAPI.Query<DynamicBuffer<DamageBuffer>, RefRW<Health>>().WithEntityAccess())
            {
                foreach (var damage in damageBuffer)
                {
                    health.ValueRW.Value = math.max(0, health.ValueRO.Value - damage.Value);
                    if (health.ValueRO.Value == 0)
                    {
                        UnityEngine.Debug.Log($"{entity} dies.");
                        commandBuffer.DestroyEntity(index++, entity);

                        var linkEntityGroupBuffer = SystemAPI.GetBuffer<LinkedEntityGroup>(entity);
                        foreach (var linkEntityGroup in linkEntityGroupBuffer)
                        {
                            commandBuffer.DestroyEntity(index++, linkEntityGroup.Value);
                        }
                        break;
                    }
                }
                damageBuffer.Clear();
            }
        }
    }
}

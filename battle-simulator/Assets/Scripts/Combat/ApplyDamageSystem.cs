using System;
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

                        if (SystemAPI.HasComponent<Owner>(entity))
                        {
                            var owner = SystemAPI.GetComponent<Owner>(entity);
                            var unitBuffer = SystemAPI.GetBuffer<UnitBuffer>(owner.Value);
                            var index = FindUnitIndex(unitBuffer, entity);
                            unitBuffer.RemoveAt(index);
                        }

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

        private int FindUnitIndex(in DynamicBuffer<UnitBuffer> unitBuffer, in Entity entity)
        {
            for (var i = 0; i < unitBuffer.Length; i++)
            {
                if (unitBuffer[i].Value == entity)
                    return i;
            }

            throw new InvalidOperationException("Entity is not in unit buffer.");
        }
    }
}

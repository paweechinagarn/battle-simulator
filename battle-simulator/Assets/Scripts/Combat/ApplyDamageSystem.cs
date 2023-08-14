using System;
using System.Runtime.InteropServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace BattleSimulator
{
    [BurstCompile]
    [UpdateAfter(typeof(AttackSystem))]
    public partial struct ApplyDamageSystem : ISystem
    {
        [BurstCompile]
        [StructLayout(LayoutKind.Auto)]
        private partial struct ApplyDamageJob : IJobEntity
        {
            public EntityCommandBuffer CommandBuffer;
            [ReadOnly] public ComponentLookup<Owner> OwnerLookup;
            public BufferLookup<UnitBuffer> UnitBufferLookup;
            [ReadOnly] public BufferLookup<LinkedEntityGroup> LinkedEntityGroupBufferLookup;

            [BurstCompile]
            public void Execute(in Entity entity, DynamicBuffer<DamageBuffer> damageBuffer, ref Health health)
            {
                foreach (var damage in damageBuffer)
                {
                    health.Value = math.max(0, health.Value - damage.Value);
                    if (health.Value != 0)
                    {
                        continue;
                    }
                    UnityEngine.Debug.Log($"{entity} dies.");

                    if (OwnerLookup.TryGetComponent(entity, out var owner)
                        && UnitBufferLookup.TryGetBuffer(owner.Value, out var unitBuffer))
                    {
                        var index = FindUnitIndex(unitBuffer, entity);
                        unitBuffer.RemoveAt(index);
                    }

                    if (LinkedEntityGroupBufferLookup.TryGetBuffer(entity, out var linkedEntityGroupBuffer))
                    {
                        foreach (var linkEntityGroup in linkedEntityGroupBuffer)
                        {
                            CommandBuffer.DestroyEntity(linkEntityGroup.Value);
                        }
                    }

                    CommandBuffer.DestroyEntity(entity);
                    break;
                }
                damageBuffer.Clear();
            }

            private static int FindUnitIndex(in DynamicBuffer<UnitBuffer> unitBuffer, in Entity entity)
            {
                for (var i = 0; i < unitBuffer.Length; i++)
                {
                    if (unitBuffer[i].Value == entity)
                        return i;
                }

                throw new InvalidOperationException("Entity is not in unit buffer.");
            }
        }

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<InGameStateTag>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var commandBufferSystem = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            var commandBuffer = commandBufferSystem.CreateCommandBuffer(state.WorldUnmanaged);
            var ownerLookup = SystemAPI.GetComponentLookup<Owner>(true);
            var unitBufferLookup = SystemAPI.GetBufferLookup<UnitBuffer>(false);
            var linkedEntityGroupBufferLookup = SystemAPI.GetBufferLookup<LinkedEntityGroup>(true);

            var job = new ApplyDamageJob
            {
                CommandBuffer = commandBuffer,
                OwnerLookup = ownerLookup,
                UnitBufferLookup = unitBufferLookup,
                LinkedEntityGroupBufferLookup = linkedEntityGroupBufferLookup
            };

            job.Schedule();
        }
    }
}

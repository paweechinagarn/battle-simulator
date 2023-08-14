using System.Runtime.InteropServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace BattleSimulator
{
    [BurstCompile]
    [UpdateAfter(typeof(UpdateTargetPositionSystem))]
    public partial struct AttackSystem : ISystem
    {
        [BurstCompile]
        [StructLayout(LayoutKind.Auto)]
        private partial struct AttackJob : IJobEntity
        {
            public EntityCommandBuffer.ParallelWriter CommandBuffer;
            [ReadOnly] public EntityStorageInfoLookup EntityStorageInfoLookup;
            public float DeltaTime;

            [BurstCompile]
            public void Execute([ChunkIndexInQuery] int sortKey, in Target target, in LocalTransform transform, ref Attack attack)
            {
                if (attack.CooldownTimer > 0f)
                    attack.CooldownTimer -= DeltaTime;

                if (attack.CooldownTimer > 0f)
                    return;

                if (target.Entity == Entity.Null || !EntityStorageInfoLookup.Exists(target.Entity))
                    return;

                var sqrDistance = math.distancesq(transform.Position, target.Position);
                if (sqrDistance > attack.Range * attack.Range)
                    return;

                var damage = new DamageBuffer { Value = attack.Damage };
                CommandBuffer.AppendToBuffer(sortKey, target.Entity, damage);

                attack.CooldownTimer = attack.CooldownTime;
            }
        }

        private EntityStorageInfoLookup entityStorageInfoLookup;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<InGameStateTag>();
            entityStorageInfoLookup = SystemAPI.GetEntityStorageInfoLookup();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            entityStorageInfoLookup.Update(ref state);

            var commandBufferSystem = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            var commandBuffer = commandBufferSystem.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();

            var job = new AttackJob
            {
                CommandBuffer = commandBuffer,
                EntityStorageInfoLookup = entityStorageInfoLookup,
                DeltaTime = SystemAPI.Time.DeltaTime
            };

            job.ScheduleParallel();
        }
    }
}

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
    public partial struct MoveToTargetSystem : ISystem
    {
        [BurstCompile]
        [StructLayout(LayoutKind.Auto)]
        private partial struct MoveToTargetJob : IJobEntity
        {
            [ReadOnly] public EntityStorageInfoLookup EntityStorageInfoLookup;
            public float DeltaTime;

            [BurstCompile]
            public void Execute(ref LocalTransform transform, in Target target, in Movement movementSpeed, in Attack attack)
            {
                if (target.Entity == Entity.Null || !EntityStorageInfoLookup.Exists(target.Entity))
                    return;

                var sqrDistance = math.distancesq(target.Position, transform.Position);
                if (sqrDistance <= attack.Range * attack.Range)
                {
                    return;
                }

                var direction = math.normalize(target.Position - transform.Position);
                var translation = direction * movementSpeed.Speed * DeltaTime;
                transform.Position += translation;
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

            var job = new MoveToTargetJob
            {
                EntityStorageInfoLookup = entityStorageInfoLookup,
                DeltaTime = SystemAPI.Time.DeltaTime
            };

            job.ScheduleParallel();
        }
    }
}

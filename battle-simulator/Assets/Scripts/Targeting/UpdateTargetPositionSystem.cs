using System.Runtime.InteropServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

namespace BattleSimulator
{
    [BurstCompile]
    [UpdateAfter(typeof(FindRandomTargetSystem))]
    public partial struct UpdateTargetPositionSystem : ISystem
    {
        [BurstCompile]
        [StructLayout(LayoutKind.Auto)]
        private partial struct UpdateTargetTransformJob : IJobEntity
        {
            [ReadOnly] public ComponentLookup<LocalTransform> TransformLookup;
            [ReadOnly] public EntityStorageInfoLookup EntityStorageInfoLookup;

            [BurstCompile]
            public void Execute(ref Target target)
            {
                if (target.Entity == Entity.Null 
                    || !EntityStorageInfoLookup.Exists(target.Entity) 
                    || !TransformLookup.TryGetComponent(target.Entity, out var targetTransform))
                    return;

                target.Position = targetTransform.Position;
            }
        }

        private ComponentLookup<LocalTransform> transformLookup;
        private EntityStorageInfoLookup entityStorageInfoLookup;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<InGameStateTag>();
            transformLookup = state.GetComponentLookup<LocalTransform>(true);
            entityStorageInfoLookup = SystemAPI.GetEntityStorageInfoLookup();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            transformLookup.Update(ref state);
            entityStorageInfoLookup.Update(ref state);

            var job = new UpdateTargetTransformJob
            {
                TransformLookup = transformLookup,
                EntityStorageInfoLookup = entityStorageInfoLookup
            };

            job.ScheduleParallel();
        }

        // a single thread version of OnUpdate

        //[BurstCompile]
        //public void OnUpdate(ref SystemState state)
        //{
        //    foreach (var target in SystemAPI.Query<RefRW<Target>>())
        //    {
        //        if (target.ValueRO.Entity == Entity.Null || !SystemAPI.Exists(target.ValueRO.Entity))
        //            continue;

        //        var targetTransform = SystemAPI.GetComponent<LocalTransform>(target.ValueRO.Entity);
        //        target.ValueRW.Position = targetTransform.Position;
        //    }
        //}
    }
}

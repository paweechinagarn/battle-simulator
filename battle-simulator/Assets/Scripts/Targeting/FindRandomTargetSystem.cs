using System.Runtime.InteropServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace BattleSimulator
{
    [BurstCompile]
    public partial struct FindRandomTargetSystem : ISystem
    {
        [BurstCompile]
        [StructLayout(LayoutKind.Auto)]
        private partial struct FindRandomTargetJob : IJobEntity
        {
            [ReadOnly] public EntityStorageInfoLookup EntityStorageInfoLookup;
            [ReadOnly] public NativeArray<Entity> Player1TargetUnits;
            [ReadOnly] public NativeArray<Entity> Player2TargetUnits;

            [BurstCompile]
            public void Execute([EntityIndexInQuery] int sortKey, ref Target target, in Player player, in Entity entity)
            {
                if (target.Entity != Entity.Null && EntityStorageInfoLookup.Exists(target.Entity))
                    return;

                var entities = player.Id switch
                {
                    1 => Player1TargetUnits,
                    2 => Player2TargetUnits,
                    _ => throw new System.NotSupportedException($"Player id {player.Id} is not currently supported. [{entity}]"),
                };

                if (entities.Length == 0)
                {
                    UnityEngine.Debug.Log($"{entity} get no target.");
                    return;
                }

                var length = entities.Length;

                var random = Random.CreateFromIndex((uint)sortKey);
                var index = random.NextInt(0, length);
                target.Entity = entities[index];
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
            var player1TargetUnits = SystemAPI.QueryBuilder().WithAll<Player>().WithAbsent<Player1Tag>().Build().ToEntityArray(Allocator.TempJob);
            var player2TargetUnits = SystemAPI.QueryBuilder().WithAll<Player>().WithAbsent<Player2Tag>().Build().ToEntityArray(Allocator.TempJob);

            var job = new FindRandomTargetJob
            {
                EntityStorageInfoLookup = entityStorageInfoLookup,
                Player1TargetUnits = player1TargetUnits,
                Player2TargetUnits = player2TargetUnits
            };

            job.ScheduleParallel();
        }
    }
}

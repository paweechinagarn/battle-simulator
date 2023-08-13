using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace BattleSimulator
{
    public partial struct FindRandomTargetSystem : ISystem
    {
        private uint randomIndex;

        public void OnUpdate(ref SystemState state)
        {
            foreach (var (targetData, team, entity) in SystemAPI.Query<RefRW<TargetData>, RefRO<Team>>().WithEntityAccess())
            {
                if (targetData.ValueRO.Target != Entity.Null && SystemAPI.Exists(targetData.ValueRO.Target))
                    continue;

                var query = team.ValueRO.Id switch
                {
                    1 => SystemAPI.QueryBuilder().WithAll<Team>().WithAbsent<Team1Tag>().Build(),
                    2 => SystemAPI.QueryBuilder().WithAll<Team>().WithAbsent<Team2Tag>().Build(),
                    _ => throw new System.NotSupportedException($"Team id {team.ValueRO.Id} is not currently supported. [{entity}]"),
                };

                var entities = query.ToEntityArray(Allocator.Temp);
                if (entities.Length == 0)
                {
                    UnityEngine.Debug.Log($"{entity} get no target.");
                    continue;
                }

                var length = entities.Length;
                var random = Random.CreateFromIndex(randomIndex++);

                var index = random.NextInt(0, length);
                targetData.ValueRW.Target = entities[index];
                UnityEngine.Debug.Log($"{entity} get new target {targetData.ValueRO.Target}");
            }
        }
    }
}

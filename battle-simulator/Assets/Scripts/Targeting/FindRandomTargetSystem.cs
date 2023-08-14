using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace BattleSimulator
{
    public partial struct FindRandomTargetSystem : ISystem
    {
        private uint randomIndex;
        
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<InGameStateTag>();
        }

        public void OnUpdate(ref SystemState state)
        {
            foreach (var (target, player, entity) in SystemAPI.Query<RefRW<Target>, RefRO<Player>>().WithEntityAccess())
            {
                if (target.ValueRO.Value != Entity.Null && SystemAPI.Exists(target.ValueRO.Value))
                    continue;

                var query = player.ValueRO.Id switch
                {
                    1 => SystemAPI.QueryBuilder().WithAll<Player>().WithAbsent<Player1Tag>().Build(),
                    2 => SystemAPI.QueryBuilder().WithAll<Player>().WithAbsent<Player2Tag>().Build(),
                    _ => throw new System.NotSupportedException($"Player id {player.ValueRO.Id} is not currently supported. [{entity}]"),
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
                target.ValueRW.Value = entities[index];
            }
        }
    }
}

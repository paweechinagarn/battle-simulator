using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace BattleSimulator
{
    public partial struct MoveToTargetSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<InGameStateTag>();
        }

        public void OnUpdate(ref SystemState state)
        {
            foreach (var (target, transform, movementSpeed, attack) in SystemAPI.Query<RefRW<Target>, RefRW<LocalTransform>, RefRO<MovementSpeed>, RefRO<Attack>>())
            {
                if (target.ValueRO.Value == Entity.Null || !SystemAPI.Exists(target.ValueRO.Value))
                    continue;
                
                var targetTransform = SystemAPI.GetComponent<LocalTransform>(target.ValueRO.Value);
                var sqrDistance = math.distancesq(targetTransform.Position, transform.ValueRO.Position);
                if (sqrDistance <= attack.ValueRO.Range * attack.ValueRO.Range)
                {
                    continue;
                }

                var direction = math.normalize(targetTransform.Position - transform.ValueRO.Position);
                var translation = direction * movementSpeed.ValueRO.Value * SystemAPI.Time.DeltaTime;
                transform.ValueRW.Position = transform.ValueRO.Position + translation;
            }
        }
    }
}

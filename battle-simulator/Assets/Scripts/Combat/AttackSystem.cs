using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace BattleSimulator
{
    public partial struct AttackSystem : ISystem
    {
        private int sortKey;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<InGameStateTag>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var commandBufferSystem = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            var commandBuffer = commandBufferSystem.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();

            foreach (var (target, transform, attack) in SystemAPI.Query<RefRO<Target>, RefRO<LocalTransform>, RefRW<Attack>>())
            {
                if (target.ValueRO.Value == Entity.Null || !SystemAPI.Exists(target.ValueRO.Value))
                    continue;

                var targetTransform = SystemAPI.GetComponent<LocalTransform>(target.ValueRO.Value);

                var sqrDistance = math.distancesq(transform.ValueRO.Position, targetTransform.Position);
                if (sqrDistance > attack.ValueRO.Range * attack.ValueRO.Range)
                {
                    continue;
                }

                attack.ValueRW.CooldownTimer += SystemAPI.Time.DeltaTime;

                if (attack.ValueRO.CooldownTimer < attack.ValueRO.CooldownTime)
                {
                    continue;
                }

                attack.ValueRW.CooldownTimer = 0f;

                var damage = new DamageBuffer { Value = attack.ValueRO.Damage };
                commandBuffer.AppendToBuffer(sortKey++, target.ValueRO.Value, damage);
            }
        }
    }
}

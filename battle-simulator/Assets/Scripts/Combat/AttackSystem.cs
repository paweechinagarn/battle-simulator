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

            foreach (var (targetData, transform, attack) in SystemAPI.Query<RefRO<TargetData>, RefRO<LocalTransform>, RefRW<Attack>>())
            {
                if (targetData.ValueRO.Target == Entity.Null || !SystemAPI.Exists(targetData.ValueRO.Target))
                    continue;

                var target = targetData.ValueRO.Target;
                var targetTransform = SystemAPI.GetComponent<LocalTransform>(targetData.ValueRO.Target);

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
                commandBuffer.AppendToBuffer(sortKey++, target, damage);
            }
        }
    }
}

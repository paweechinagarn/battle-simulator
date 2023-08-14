using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace BattleSimulator
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public partial class UpdateHealthUISystem : SystemBase
    {
        private static readonly float3 offsetPosition = new float3(0f, 2.75f, 0f);

        protected override void OnCreate()
        {
            RequireForUpdate<InGameStateTag>();
        }

        protected override void OnStartRunning()
        {
            foreach (var (uiComponent, entity) in SystemAPI.Query<HealthUIComponentData>().WithEntityAccess())
            {
                uiComponent.HealthUI = HealthUIPool.Pool.Get();
            }
        }

        protected override void OnUpdate()
        {
            foreach (var (uiComponent, health, transform, entity) in SystemAPI.Query<HealthUIComponentData, RefRO<Health>, RefRO<LocalTransform>>().WithEntityAccess())
            {
                if (health.ValueRO.Value > 0)
                {
                    uiComponent.HealthUI.Health = health.ValueRO.Value;
                    uiComponent.HealthUI.transform.position = transform.ValueRO.Position + offsetPosition;
                }
                else
                {
                    HealthUIPool.Pool.Release(uiComponent.HealthUI);
                    uiComponent.HealthUI = null;
                }
            }
        }
    }
}

using Unity.Entities;
using UnityEngine;

namespace BattleSimulator
{
    public class HealthAuthoring : MonoBehaviour
    {
        private class Baker : Baker<HealthAuthoring>
        {
            public override void Bake(HealthAuthoring authoring)
            {
                var entity = GetEntity(authoring, TransformUsageFlags.Dynamic);

                var component = new Health
                {
                    Value = authoring.Value,
                    MaxValue = authoring.Value
                };

                AddComponent(entity, component);
                AddBuffer<DamageBuffer>(entity);
            }
        }

        [Min(1)]
        public int Value;
    }
}

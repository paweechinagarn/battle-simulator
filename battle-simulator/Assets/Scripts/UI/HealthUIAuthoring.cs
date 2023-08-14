using Unity.Entities;
using UnityEngine;

namespace BattleSimulator
{
    public class HealthUIAuthoring : MonoBehaviour
    {
        private class Baker : Baker<HealthUIAuthoring>
        {
            public override void Bake(HealthUIAuthoring authoring)
            {
                var entity = GetEntity(authoring, TransformUsageFlags.Dynamic);

                var component = new HealthUIComponentData();

                AddComponentObject(entity, component);
            }
        }
    }
}

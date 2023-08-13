using Unity.Entities;
using UnityEngine;

namespace BattleSimulator
{
    public class TargetDataAuthoring : MonoBehaviour
    {
        private class Baker : Baker<TargetDataAuthoring>
        {
            public override void Bake(TargetDataAuthoring authoring)
            {
                var entity = GetEntity(authoring, TransformUsageFlags.Dynamic);

                var component = new TargetData();

                AddComponent(entity, component);
            }
        }
    }
}

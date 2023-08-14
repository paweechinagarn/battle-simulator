using Unity.Entities;
using UnityEngine;

namespace BattleSimulator
{
    public class TargetAuthoring : MonoBehaviour
    {
        private class Baker : Baker<TargetAuthoring>
        {
            public override void Bake(TargetAuthoring authoring)
            {
                var entity = GetEntity(authoring, TransformUsageFlags.Dynamic);

                var component = new Target();

                AddComponent(entity, component);
            }
        }
    }
}

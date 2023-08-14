using Unity.Entities;
using UnityEngine;

namespace BattleSimulator
{
    public class SpawnSettingsAuthoring : MonoBehaviour
    {
        private class Baker : Baker<SpawnSettingsAuthoring>
        {
            public override void Bake(SpawnSettingsAuthoring authoring)
            {
                var entity = GetEntity(authoring, TransformUsageFlags.None);

                var component = new SpawnSettings
                {
                    GridSize = authoring.GridSize
                };

                AddComponent(entity, component);
            }
        }

        [Min(0.1f)]
        public float GridSize = 1f;
    }
}

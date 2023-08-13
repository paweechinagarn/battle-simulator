using Unity.Entities;
using UnityEngine;

namespace BattleSimulator
{
    public class SpawnerAuthoring : MonoBehaviour
    {
        private class Baker : Baker<SpawnerAuthoring>
        {
            public override void Bake(SpawnerAuthoring authoring)
            {
                var entity = GetEntity(authoring, TransformUsageFlags.None);
                var component = new Spawner
                {
                    Prefab = GetEntity(authoring.Prefab, TransformUsageFlags.None),
                    PlayerConfig = authoring.Config
                };

                AddComponentObject(entity, component);
            }
        }

        public GameObject Prefab;
        public SpawnScriptableObject Config;
    }
}

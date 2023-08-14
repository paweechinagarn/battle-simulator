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
                    PlayerId = authoring.PlayerId,
                    TeamId = authoring.TeamId,
                    Config = authoring.Config,
                };

                AddComponentObject(entity, component);
                AddBuffer<UnitBuffer>(entity);
            }
        }

        public GameObject Prefab; 
        public int PlayerId;
        public int TeamId;
        public SpawnScriptableObject Config;
    }
}

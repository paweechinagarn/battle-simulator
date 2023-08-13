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
                var entity = GetEntity(TransformUsageFlags.None);
                var component = new Spawner
                {
                    Prefab = GetEntity(authoring.Prefab, TransformUsageFlags.None),
                    Amount = authoring.Amount
                };

                AddComponent(entity, component);
            }
        }

        public GameObject Prefab;
        public int Amount;
    }
}

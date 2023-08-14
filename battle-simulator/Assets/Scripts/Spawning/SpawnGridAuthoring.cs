using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace BattleSimulator
{
    public class SpawnGridAuthoring : MonoBehaviour
    {
        private class Baker : Baker<SpawnGridAuthoring>
        {
            public override void Bake(SpawnGridAuthoring authoring)
            {
                var entity = GetEntity(authoring, TransformUsageFlags.None);

                var component = new SpawnGrid
                {
                    Width = authoring.Width,
                    Height = authoring.Height,
                    OriginPosition = authoring.OriginPosition
                };

                AddComponent(entity, component);
            }
        }

        public int Width;
        public int Height;
        public float3 OriginPosition;
    }
}

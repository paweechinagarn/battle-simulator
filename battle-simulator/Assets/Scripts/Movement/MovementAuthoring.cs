using Unity.Entities;
using UnityEngine;

namespace BattleSimulator
{
    public class MovementSpeedAuthoring : MonoBehaviour
    {
        private class Baker : Baker<MovementSpeedAuthoring>
        {
            public override void Bake(MovementSpeedAuthoring authoring)
            {
                var entity = GetEntity(authoring, TransformUsageFlags.Dynamic);

                var component = new Movement
                {
                    Speed = authoring.Speed
                };

                AddComponent(entity, component);
            }
        }

        [Min(0.1f)]
        public float Speed;
    }
}

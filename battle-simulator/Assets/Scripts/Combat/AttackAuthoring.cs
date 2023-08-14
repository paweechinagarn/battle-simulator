using Unity.Entities;
using UnityEngine;

namespace BattleSimulator
{
    public class AttackAuthoring : MonoBehaviour
    {
        private class Baker : Baker<AttackAuthoring>
        {
            public override void Bake(AttackAuthoring authoring)
            {
                var entity = GetEntity(authoring, TransformUsageFlags.Dynamic);

                var component = new Attack
                {
                    Damage = authoring.Damage,
                    Speed = authoring.Speed,
                    Range = authoring.Range,
                };

                AddComponent(entity, component);
            }
        }

        public int Damage = 10;
        [Min(0.1f)]
        public float Speed = 1f;
        [Min(0.1f)]
        public float Range = 1f;
    }
}

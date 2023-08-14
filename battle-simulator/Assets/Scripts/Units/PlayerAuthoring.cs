using Unity.Entities;
using UnityEngine;

namespace BattleSimulator
{
    public class PlayerAuthoring : MonoBehaviour
    {
        private class Baker : Baker<PlayerAuthoring>
        {
            public override void Bake(PlayerAuthoring authoring)
            {
                var entity = GetEntity(authoring, TransformUsageFlags.Dynamic);

                AddComponent(entity, new Player { Id = authoring.Id });

                switch (authoring.Id)
                {
                    case 1:
                        AddComponent(entity, new Player1Tag());
                        break;
                    case 2:
                        AddComponent(entity, new Player2Tag());
                        break;
                    default:
                        throw new System.NotSupportedException($"Team id {authoring.Id} is not currently supported. [{entity}]");
                }
            }
        }

        public int Id = 1;
    }
}

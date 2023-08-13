using Unity.Entities;
using UnityEngine;

namespace BattleSimulator
{
    public class TeamAuthoring : MonoBehaviour
    {
        private class Baker : Baker<TeamAuthoring>
        {
            public override void Bake(TeamAuthoring authoring)
            {
                var entity = GetEntity(authoring, TransformUsageFlags.Dynamic);

                AddComponent(entity, new Team { Id = authoring.Id });

                switch (authoring.Id)
                {
                    case 1:
                        AddComponent(entity, new Team1Tag());
                        break;
                    case 2:
                        AddComponent(entity, new Team2Tag());
                        break;
                    default:
                        throw new System.NotSupportedException($"Team id {authoring.Id} is not currently supported. [{entity}]");
                }
            }
        }

        public int Id = 1;
    }
}

using Unity.Entities;
using UnityEngine;

namespace BattleSimulator
{
    public class GameStateAuthoring : MonoBehaviour
    {
        private class Baker : Baker<GameStateAuthoring>
        {
            public override void Bake(GameStateAuthoring authoring)
            {
                var entity = GetEntity(authoring, TransformUsageFlags.None);

                var component = new GameState
                {
                    Value = authoring.Value
                };

                AddComponent(entity, component);
            }
        }

        public GameState.State Value;
    }
}

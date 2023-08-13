using Unity.Entities;

namespace BattleSimulator
{
    public class Spawner : IComponentData, IEnableableComponent
    {
        public Entity Prefab;
        public SpawnScriptableObject PlayerConfig;
        public SpawnScriptableObject EnemyConfig;
        public int EnemyTeamId;
    }
}

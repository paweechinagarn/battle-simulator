using UnityEngine;

namespace BattleSimulator
{
    [CreateAssetMenu(fileName = "PlayerSpawnSetup", menuName = "BattleSimulator/PlayerSpawnSetup")]
    public class PlayerSpawnSetup : ScriptableObject
    {
        public SpawnSetupTeam[] Teams;
    }
}

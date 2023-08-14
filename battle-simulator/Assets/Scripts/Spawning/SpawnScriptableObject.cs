using UnityEngine;

namespace BattleSimulator
{
    [CreateAssetMenu(fileName = "SpawnSetup", menuName = "BattleSimulator/SpawnSetup")]
    public class SpawnScriptableObject : ScriptableObject
    {
        public SpawnSetupTeam[] Teams;
    }
}

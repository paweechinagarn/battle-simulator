using System;

namespace BattleSimulator
{
    [Serializable]
    public class SpawnSetupTeam
    {
        public int Id;
        public string Name;
        public SpawnSetupUnit[] Units;
    }
}

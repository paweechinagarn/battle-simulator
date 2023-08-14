using System;
using UnityEngine;

namespace BattleSimulator
{
    [Serializable]
    public class SpawnSetupUnit
    {
        public int Id;
        public int Health;
        public int AttackDamage;
        public float AttackSpeed;
        public float AttackRange;
        public float MovementSpeed;

        [Min(0)]
        public int StartXPosition;
        [Min(0)]
        public int StartYPosition;
    }
}

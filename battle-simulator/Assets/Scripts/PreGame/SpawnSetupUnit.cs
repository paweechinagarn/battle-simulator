using System;
using UnityEngine;

namespace BattleSimulator
{
    [Serializable]
    public class SpawnSetupUnit
    {
        public int Health;
        public int AttackDamage;
        public float AttackSpeed;
        public float AttackRange;
        public float MovementSpeed;

        [Range(0, 2)]
        public int StartXPosition;
        [Range(0, 2)]
        public int StartYPosition;
    }
}

﻿using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace BattleSimulator
{
    public struct Attack : IComponentData
    {
        public int Damage;
        public float CooldownTimer;

        // Prevent speed to go below 0.1
        [Min(0.1f)]
        [SerializeField] private float speed;
        public float Speed
        {
            get { return speed; }
            set { speed = math.max(0.1f, value); }
        }

        // Prevent range to go below 0.1
        [Min(0.1f)]
        [SerializeField] private float range; 
        public float Range
        {
            get { return range; }
            set { range = math.max(0.1f, value); }
        }

        public float CooldownTime => 1f / Speed;
    }
}

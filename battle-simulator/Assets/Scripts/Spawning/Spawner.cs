﻿using Unity.Entities;

namespace BattleSimulator
{
    public class Spawner : IComponentData
    {
        public Entity Prefab;
        public int PlayerId;
        public int TeamId;
        public PlayerSpawnSetup Config;
        
        public bool NeedsUpdate { get; set; }
    }
}

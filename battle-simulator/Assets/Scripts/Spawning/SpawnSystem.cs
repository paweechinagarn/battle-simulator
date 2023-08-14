using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace BattleSimulator
{
    public partial class SpawnSystem : SystemBase, IDomainEventHandler<TeamSelectedEvent>
    {
        private const float gridSize = 2f;

        private NativeArray<int> spawnGridArray;

        protected override void OnCreate()
        {
            RequireForUpdate<PreGameStateTag>();
            RequireForUpdate<Spawner>();
        }

        protected override void OnStartRunning()
        {
            DomainEvents.RegisterDomainEventHandler(this);

            foreach (var spawner in SystemAPI.Query<Spawner>())
            {
                spawner.NeedsUpdate = true;
            }

            CreateOrUpdate();
        }

        protected override void OnStopRunning()
        {
            DomainEvents.UnregisterDomainEventHandler(this);
        }

        protected override void OnUpdate()
        {
        }

        private void CreateOrUpdate()
        {
            var commandBufferSystem = SystemAPI.GetSingleton<EndInitializationEntityCommandBufferSystem.Singleton>();
            var commandBuffer = commandBufferSystem.CreateCommandBuffer(World.Unmanaged).AsParallelWriter();

            foreach (var (spawner, unitBuffer, spawnGrid, spawnerEntity) in 
                SystemAPI.Query<Spawner, DynamicBuffer<UnitBuffer>, RefRO<SpawnGrid>>()
                .WithEntityAccess())
            {
                if (!spawner.NeedsUpdate)
                    continue;

                spawner.NeedsUpdate = false;

                var config = spawner.Config;
                var team = System.Array.Find(config.Teams, team => team.Id == spawner.TeamId);

                if (team == null || team.Units.Length == 0)
                    continue;

                spawnGridArray = new NativeArray<int>(spawnGrid.ValueRO.Width * spawnGrid.ValueRO.Height, Allocator.Temp);

                for (var i = 0; i < team.Units.Length; i++)
                {
                    var unitData = team.Units[i];

                    if (unitData.StartXPosition < 0 ||
                        unitData.StartYPosition < 0 ||
                        unitData.StartXPosition >= spawnGrid.ValueRO.Width ||
                        unitData.StartYPosition >= spawnGrid.ValueRO.Height)
                    {
                        UnityEngine.Debug.LogError($"player {spawner.PlayerId} team {spawner.TeamId} unit id {unitData.Id} is placed outside grid at [{unitData.StartXPosition}, {unitData.StartYPosition}]");
                        continue;
                    }
                    var arrayIndex = unitData.StartYPosition * spawnGrid.ValueRO.Width + unitData.StartXPosition;
                    if (spawnGridArray[arrayIndex] != 0)
                    {
                        UnityEngine.Debug.LogError($"player {spawner.PlayerId} team {spawner.TeamId} unit id {unitData.Id} is placed at the same position with unit id {spawnGridArray[arrayIndex]}");
                        continue;
                    }

                    spawnGridArray[arrayIndex] = unitData.Id;

                    var instance = unitBuffer.Length > i
                        ? unitBuffer[i].Value
                        : commandBuffer.Instantiate(i, spawner.Prefab);

                    var position = new float3(unitData.StartXPosition * gridSize, 1f, unitData.StartYPosition * gridSize);
                    commandBuffer.SetComponent(i, instance, LocalTransform.Identity.WithPosition(position + spawnGrid.ValueRO.OriginPosition));

                    commandBuffer.SetComponent(i, instance, new Health
                    {
                        MaxValue = unitData.Health,
                        Value = unitData.Health
                    });

                    commandBuffer.SetComponent(i, instance, new Movement
                    {
                        Speed = unitData.MovementSpeed
                    });

                    commandBuffer.SetComponent(i, instance, new Attack
                    {
                        Damage = unitData.AttackDamage,
                        Speed = unitData.AttackSpeed,
                        Range = unitData.AttackRange
                    });

                    // In case of new unit
                    if (unitBuffer.Length <= i)
                    {
                        commandBuffer.SetComponent(i, instance, new Player
                        {
                            Id = spawner.PlayerId
                        });

                        switch (spawner.PlayerId)
                        {
                            case 1:
                                commandBuffer.AddComponent(i, instance, new Player1Tag());
                                commandBuffer.RemoveComponent<Player2Tag>(i, instance);
                                break;
                            case 2:
                                commandBuffer.RemoveComponent<Player1Tag>(i, instance);
                                commandBuffer.AddComponent(i, instance, new Player2Tag());
                                break;
                            default:
                                throw new System.NotSupportedException($"Team id {spawner.PlayerId} is not currently supported. [{spawnerEntity}]");
                        }

                        commandBuffer.AddComponent(i, instance, new Owner
                        {
                            Value = spawnerEntity
                        });

                        commandBuffer.AppendToBuffer(i, spawnerEntity, new UnitBuffer { Value = instance });
                    }
                }

                if (unitBuffer.Length > team.Units.Length)
                {
                    for (var i = team.Units.Length; i < unitBuffer.Length; i++)
                    {
                        commandBuffer.DestroyEntity(i, unitBuffer[i].Value);
                    }
                    unitBuffer.RemoveRange(team.Units.Length, unitBuffer.Length - team.Units.Length);
                }
            }
        }

        public void Handle(TeamSelectedEvent evt)
        {
            foreach (var spawner in SystemAPI.Query<Spawner>())
            {
                if (spawner.PlayerId != evt.PlayerId)
                    continue;

                if (spawner.TeamId == evt.TeamId)
                    continue;

                spawner.TeamId = evt.TeamId;
                spawner.NeedsUpdate = true;
            }

            CreateOrUpdate();
        }
    }
}

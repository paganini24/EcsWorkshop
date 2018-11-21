using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace Jumper
{
    public struct PlayerCheck : IComponentData { }
    public class PlayerCheckRemoveBarrier : BarrierSystem { }

    public struct PlayerScore : IComponentData
    {
        public int score;
    }
    public class PlayerCheckSystem : JobComponentSystem
    {

        struct IslandState
        {
            public readonly int Length;
            public ComponentDataArray<IslandSpawnState> islandSpawnState;
        }
        [Inject]
        private PlayerCheckRemoveBarrier playerCheckBarrier;
        [Inject]
        IslandState stateGroup;

        // [BurstCompile]
        struct CheckPlayerJob : IJobProcessComponentDataWithEntity<Position, PlayerCheck, PlayerScore>
        {
            public float3 islandPos, islandScale;
            public EntityCommandBuffer commands;
            public void Execute(Entity entity, int index, ref Position position, ref PlayerCheck playercheck, ref PlayerScore score)
            {
                commands.RemoveComponent<PlayerCheck>(entity);

                var playerOn = IsPlayerOnIsland(position.Value, islandPos, islandScale);
                if (playerOn)
                    score.score += 1;
                else
                {
                    //score.score = 0;
                    commands.DestroyEntity(entity);
                }
            }

            private bool IsPlayerOnIsland(float3 position, float3 islandP, float3 islandS)
            {
                var xScale = islandS.x / 2f;
                var minx = islandP.x - xScale;
                var maxx = islandP.x + xScale;

                return position.x > minx && position.x < maxx;
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            return new CheckPlayerJob
            {
                islandPos = stateGroup.islandSpawnState[0].lastPosition,
                islandScale = stateGroup.islandSpawnState[0].lastSize,
                commands = playerCheckBarrier.CreateCommandBuffer()
            }.ScheduleSingle(this, inputDeps);
        }
    }
}

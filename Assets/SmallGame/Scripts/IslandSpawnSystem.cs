using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Random = UnityEngine.Random;

namespace SmallGame
{
    public struct Island : IComponentData {public int Index;}

    public struct IslandSpawnState : IComponentData
    {
        public int islandCount;
        public float3 lastPosition;
        public float3 lastSize;
    }

    public class IslandSpawnSystem : ComponentSystem
    {
        struct IslandState
        {
            public readonly int Length;
            public ComponentDataArray<IslandSpawnState> islandSpawnState;
        }

        struct PlayerState
        {
            public readonly int Length;
            public ComponentDataArray<PlayerScore> playerScore;
        }

        [Inject]
        IslandState islandStateGroup;
        [Inject]
        PlayerState playerStateGroup;

        protected override void OnCreateManager()
        {
            var entityManager = World.Active.GetOrCreateManager<EntityManager>();
            var arch = entityManager.CreateArchetype(typeof(IslandSpawnState));
            var entity = entityManager.CreateEntity(arch);
            entityManager.SetComponentData(entity, new IslandSpawnState());
        }

        protected override void OnUpdate()
        {
            var islandSpawnState = islandStateGroup.islandSpawnState[0];
            var isAlive = playerStateGroup.playerScore.Length > 0;
            if (isAlive)
            {
                var playerState = playerStateGroup.playerScore[0];
                if (islandSpawnState.islandCount < playerState.score + 2)
                {
                    SpawnIsland();
                }
            }
            else // Destroy Islands
            {
                var group = GetComponentGroup(typeof(Island));
                var entities = group.GetEntityArray();
                for (int i = 0; i < entities.Length; i++)
                {
                    var entity = entities[i];
                    PostUpdateCommands.DestroyEntity(entity);
                }

                var newState = new IslandSpawnState();
                islandStateGroup.islandSpawnState[0] = newState;
            }
        }

        void SpawnIsland()
        {
            var state = islandStateGroup.islandSpawnState[0];
            PostUpdateCommands.CreateEntity(Bootstrap.islandArchetype);
            var spawnLocation = state.islandCount == 0 ? new float3(0f, -0.5f, 0f) : state.lastPosition + GetSpawnLocation();
            PostUpdateCommands.SetComponent(new Position() { Value = spawnLocation });
            var randomScale = GetRandomScale();
            PostUpdateCommands.SetComponent(new Scale() { Value = randomScale });
            PostUpdateCommands.SetComponent(new Island());

            PostUpdateCommands.AddSharedComponent(Bootstrap.islandLook);

            state.islandCount++;
            state.lastPosition = spawnLocation;
            state.lastSize = randomScale;

            islandStateGroup.islandSpawnState[0] = state;
        }

        private float3 GetRandomScale()
        {
            var range = Random.Range(Bootstrap.settings.randomSize.x, Bootstrap.settings.randomSize.y);
            return new float3(range
                , 0.2f, range);
        }

        private float3 GetSpawnLocation()
        {
            return new float3(Random.Range(Bootstrap.settings.randomDistance.x, Bootstrap.settings.randomDistance.y)
                , 0f, 0f);
        }
    }
}

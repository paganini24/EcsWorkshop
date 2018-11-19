using System;
using System.Threading.Tasks;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Rendering;

namespace SmallGame
{
    public class Bootstrap : MonoBehaviour
    {
        [SerializeField]private Settings _settings;
        [SerializeField] private BooleanVariable _boolNewGame;
        public static EntityArchetype playerArchetype,islandArchetype;
        public static MeshInstanceRenderer playerLook,islandLook;
        public static Settings settings;

        private void Awake()
        {
            Initialize();
        }

        public void Initialize()
        {
            var manager = World.Active.GetOrCreateManager<EntityManager>();
            playerArchetype = manager.CreateArchetype(typeof(Position), typeof(Scale), typeof(PlayerInput), typeof(PlayerJump), typeof(PlayerScore));
            islandArchetype = manager.CreateArchetype(typeof(Position), typeof(Scale), typeof(Island));

            playerLook = GetLookFromResources("Player");
            islandLook = GetLookFromResources("Island");
            settings = _settings;

            _boolNewGame.ValueChanged += b => { if(b) NewGame(); };

            World.Active.GetOrCreateManager<GameHud>();
        }

        private static MeshInstanceRenderer GetLookFromResources(string resourceName)
        {
            var proto = Resources.Load<GameObject>(resourceName);
            return new MeshInstanceRenderer()
            {
                material = proto.GetComponent<MeshRenderer>().sharedMaterial,
                mesh = proto.GetComponent<MeshFilter>().sharedMesh,
                receiveShadows = true,
                castShadows = ShadowCastingMode.On
            };
        }

        private async void NewGame()
        {
            // Cant figure out, when you do pointer up from new game button PlayerInputSystem is initialized
            // and Input.GetMouseButtonUp(0) line is being called which makes player jump immediately 
            // TODO find a better way than adding delay
            await Task.Delay(TimeSpan.FromSeconds(0.1f));

            var entityManager = World.Active.GetOrCreateManager<EntityManager>();

            var player = entityManager.CreateEntity(playerArchetype);
            entityManager.SetComponentData(player, new Position {Value = float3.zero});
            entityManager.SetComponentData(player, new Scale {Value = new float3(0.3f,0.8f,0.3f)});
            entityManager.SetComponentData(player, new PlayerJump());
            entityManager.SetComponentData(player, new PlayerScore {score = 0});
            entityManager.SetComponentData(player, new PlayerInput() {isJumping = 0});

            entityManager.AddSharedComponentData(player,playerLook);

            World.Active.GetOrCreateManager<IslandSpawnSystem>();
        }
    }
}

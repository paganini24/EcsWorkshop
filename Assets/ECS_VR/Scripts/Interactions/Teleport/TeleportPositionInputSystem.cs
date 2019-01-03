using Unity.Collections;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Rendering;

namespace Unity.Entities.VR.Interactions.Teleport
{
    [DisableAutoCreation]
    public class TeleportPositionInputSystem : ComponentSystem
    {
        private ComponentGroup _vrTeleportControllerGroup, _vrTeleportPositionBeginGroup;
        private EntityArchetype _teleportPositionArchetype, _teleportPositionBeginArchetype;
        private MeshInstanceRenderer _teleportIndicator;
        protected override void OnCreateManager()
        {
            var entityManager = World.Active.GetOrCreateManager<EntityManager>();
            _teleportPositionArchetype = entityManager.CreateArchetype(typeof(VrTeleportPosition));
            _teleportPositionBeginArchetype = entityManager.CreateArchetype(typeof(VrTeleportPositionIndicator),
                typeof(Position),typeof(Rotation),typeof(MeshInstanceRenderer),typeof(Scale));
           
            _vrTeleportControllerGroup = GetComponentGroup(typeof(VrNodeData), typeof(Rotation), typeof(LocalToWorld),
                typeof(VrTeleport), typeof(Position));

            _vrTeleportPositionBeginGroup = GetComponentGroup(typeof(Position), typeof(VrTeleportPositionIndicator));
            _teleportIndicator = new MeshInstanceRenderer()
            {
                castShadows = ShadowCastingMode.Off,
                material = Resources.Load<Material>("LaserMaterial"),
                receiveShadows = false,
                mesh = Resources.Load<GameObject>("LaserPointer").GetComponent<MeshFilter>().mesh
            };
        }

        protected override void OnUpdate()
        {
            var controllers = _vrTeleportControllerGroup.GetComponentDataArray<VrNodeData>();
            for (int i = 0; i < controllers.Length; i++)
            {
                var rot = _vrTeleportControllerGroup.GetComponentDataArray<Rotation>()[i];
                var pos = _vrTeleportControllerGroup.GetComponentDataArray<LocalToWorld>()[i].Value.c3.xyz;
                // Hold
                // A bit dirty here we can seperate here to another system actually
                if (Input.GetButtonDown($"{controllers[i].xrNode} Trackpad Touch"))
                {
                    PostUpdateCommands.CreateEntity(_teleportPositionBeginArchetype);
                    PostUpdateCommands.SetComponent(new Position() {Value = float3.zero});
                    PostUpdateCommands.SetComponent(new Rotation() {Value = rot.Value});
                    PostUpdateCommands.SetComponent(new VrTeleportPositionIndicator());
                    PostUpdateCommands.SetSharedComponent(_teleportIndicator);
                }
                else if (Input.GetButton($"{controllers[i].xrNode} Trackpad Touch"))
                {
                    Vector3 hitPos;
                    float distance;
                    CastRay(pos, rot, out hitPos, out distance);
                    var entity = _vrTeleportPositionBeginGroup.GetEntityArray()[0];
                    PostUpdateCommands.SetComponent(entity,new Position() {Value = pos + math.forward(rot.Value) * distance/2});
                    PostUpdateCommands.SetComponent(entity,new Rotation() {Value = rot.Value});
                    PostUpdateCommands.SetComponent(entity, new Scale(){
                        Value = new float3(.02f,.02f, distance )
                    });
                }
                else if (Input.GetButtonUp($"{controllers[i].xrNode} Trackpad Touch"))
                {
                    var entity = _vrTeleportPositionBeginGroup.GetEntityArray()[0];
                    PostUpdateCommands.DestroyEntity(entity);
                }
                // Click
                if (Input.GetButtonDown($"{controllers[i].xrNode} Trackpad"))
                {
                    Vector3 hitPos;
                    if (!CastRay(pos, rot, out hitPos)) continue;
                    PostUpdateCommands.CreateEntity(_teleportPositionArchetype);
                    PostUpdateCommands.SetComponent(new VrTeleportPosition() {position = hitPos});
                }
                
            }
        }

        private bool CastRay(float3 pos, Rotation rot, out Vector3 hitPoint)
        {
            var results = new NativeArray<RaycastHit>(1, Allocator.TempJob);
            var commands = new NativeArray<RaycastCommand>(1, Allocator.TempJob);
            // Set the data of the first command
            Vector3 origin = pos;
            Vector3 direction = math.forward(rot.Value);
            commands[0] = new RaycastCommand(origin, direction,100f);
            // Schedule the batch of raycasts
            var handle = RaycastCommand.ScheduleBatch(commands, results, 1);
            // Wait for the batch processing job to complete
            handle.Complete();
            // Copy the result. If batchedHit.collider is null there was no hit
            var batchedHit = results[0];
            hitPoint = batchedHit.point;
            // Dispose the buffers
            results.Dispose();
            commands.Dispose();
            return batchedHit.collider != null;
        }
        private bool CastRay(float3 pos, Rotation rot, out Vector3 hitPoint, out float distance)
        {
            var results = new NativeArray<RaycastHit>(1, Allocator.TempJob);
            var commands = new NativeArray<RaycastCommand>(1, Allocator.TempJob);
            // Set the data of the first command
            Vector3 origin = pos;
            Vector3 direction = math.forward(rot.Value);
            commands[0] = new RaycastCommand(origin, direction,100f);
            // Schedule the batch of raycasts
            var handle = RaycastCommand.ScheduleBatch(commands, results, 1);
            // Wait for the batch processing job to complete
            handle.Complete();
            // Copy the result. If batchedHit.collider is null there was no hit
            var batchedHit = results[0];
            hitPoint = batchedHit.point;
            distance = batchedHit.distance;
            // Dispose the buffers
            results.Dispose();
            commands.Dispose();
            return batchedHit.collider != null;
        }
    }
}
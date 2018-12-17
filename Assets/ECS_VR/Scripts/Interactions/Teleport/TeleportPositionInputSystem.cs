using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Unity.Entities.VR.Interactions.Teleport
{
    public class TeleportPositionInputSystem : ComponentSystem
    {
        private ComponentGroup _vrTeleportControllerGroup;
        private EntityArchetype _teleportPositionArchetype;
        
        protected override void OnCreateManager()
        {
            var entityManager = World.Active.GetOrCreateManager<EntityManager>();
            _teleportPositionArchetype = entityManager.CreateArchetype(typeof(VrTeleportPosition));
            _vrTeleportControllerGroup = GetComponentGroup(typeof(VrNodeData), typeof(Rotation), typeof(LocalToWorld),
                typeof(VrTeleport));
        }

        protected override void OnUpdate()
        {
            var controllers = _vrTeleportControllerGroup.GetComponentDataArray<VrNodeData>();
            for (int i = 0; i < controllers.Length; i++)
            {
                if (Input.GetButtonDown($"{controllers[i].xrNode} Touch"))
                {
                    Debug.Log("Touch");
                }
                else if (Input.GetButtonUp($"{controllers[i].xrNode} Touch"))
                {
                    Debug.Log("Touch ended");
                }
                if (Input.GetButtonDown($"{controllers[i].xrNode} Trackpad"))
                {
                    var rot = _vrTeleportControllerGroup.GetComponentDataArray<Rotation>()[i];
                    var pos = _vrTeleportControllerGroup.GetComponentDataArray<LocalToWorld>()[i];
                    Vector3 hitPos;
                    if (!CastRay(pos.Value.c3.xyz, rot, out hitPos)) continue;
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
    }
}
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.XR;

namespace Unity.Entities.VR.Interactions.Teleport
{
    public class TeleportPositionInputSystem : ComponentSystem
    {
        private ComponentGroup _teleportGroup;
        private ComponentGroup _vrControllerGroup;
        private EntityManager _entityManager;
        protected override void OnCreateManager()
        {
            _entityManager = World.Active.GetOrCreateManager<EntityManager>();

            _entityManager.CreateEntity(typeof(Position), typeof(Rotation), typeof(VrTeleport));
            
            _teleportGroup = GetComponentGroup(typeof(Position), typeof(VrTeleport), typeof(Rotation));
            _vrControllerGroup = GetComponentGroup(typeof(VrNodeData), typeof(Rotation), typeof(Position));
        }

        protected override void OnUpdate()
        {
            if (Input.GetButtonDown("Right Controller Trackpad Touch"))
            {
                Debug.Log("Touch");
            }
            else if (Input.GetButtonUp("Right Controller Trackpad Touch"))
            {
                Debug.Log("Touch ended");
            }

            if (Input.GetButtonDown("Right Controller Trackpad"))
            {
                Debug.Log(XRNode.RightHand);

                var controllers = _vrControllerGroup.GetComponentDataArray<VrNodeData>();
                for (int i = 0; i < controllers.Length; i++)
                {
                    if (controllers[i].xrNode == XRNode.RightHand)
                    {
                        var rot = _vrControllerGroup.GetComponentDataArray<Rotation>()[i];
                        var pos = _vrControllerGroup.GetComponentDataArray<Position>()[i];
                        Vector3 hitPos;
                        if (CastRay(pos, rot, out hitPos))
                        {
                            Debug.Log(hitPos);

                            var entityArray = _teleportGroup.GetEntityArray();
                            if (entityArray.Length == 0) return;

                            var entity = entityArray[0];
                            _entityManager.AddComponentData(entity, new TeleportPosition(){ position = hitPos});
                        }
                    }
                }
            }
        }

        private bool CastRay(Position pos, Rotation rot, out Vector3 hitPoint)
        {
            var results = new NativeArray<RaycastHit>(1, Allocator.TempJob);
            var commands = new NativeArray<RaycastCommand>(1, Allocator.TempJob);
            // Set the data of the first command
            Vector3 origin = pos.Value;
            Vector3 direction = math.forward(rot.Value);
            commands[0] = new RaycastCommand(origin, direction);

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

        struct TeleportJob : IJobProcessComponentDataWithEntity<Position, VrTeleport>
        {
            public void Execute(Entity entity, int index, ref Position data0, ref VrTeleport data1)
            {
            }
        }
    }
}
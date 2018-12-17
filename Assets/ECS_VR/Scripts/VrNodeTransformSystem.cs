using Unity.Collections;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.XR;

namespace Unity.Entities.VR
{
    public class VrNodeTransformSystem : ComponentSystem
    {
        private ComponentGroup _groupControllers;
        [ReadOnly]
        private ArchetypeChunkComponentType<Position> _positionType;
        [ReadOnly]
        private ArchetypeChunkComponentType<Rotation> _rotationType;
        [ReadOnly]
        private ArchetypeChunkComponentType<VrNodeData> _xrNodeType;

        protected override void OnCreateManager()
        {
            _groupControllers = GetComponentGroup(typeof(VrNodeData), typeof(Position), typeof(Rotation));
        }

        protected override void OnUpdate()
        {
            _positionType = GetArchetypeChunkComponentType<Position>();
            _rotationType = GetArchetypeChunkComponentType<Rotation>();
            _xrNodeType = GetArchetypeChunkComponentType<VrNodeData>();

            var chunks = _groupControllers.CreateArchetypeChunkArray(Allocator.TempJob);
            for (int i = 0; i < chunks.Length; i++)
            {
                ProcessControllerChunks(chunks[i]);
            }
            chunks.Dispose();
        }

        private void ProcessControllerChunks(ArchetypeChunk archetypeChunk)
        {
            var positions = archetypeChunk.GetNativeArray(_positionType);
            var rotations = archetypeChunk.GetNativeArray(_rotationType);
            var xrNodes = archetypeChunk.GetNativeArray(_xrNodeType);

            for (int i = 0; i < archetypeChunk.Count; i++)
            {
                positions[i] = new Position { Value = InputTracking.GetLocalPosition(xrNodes[i].xrNode) };
                rotations[i] = new Rotation { Value = InputTracking.GetLocalRotation(xrNodes[i].xrNode) };
            }
        }
    }
}

using Unity.Transforms;
using UnityEngine;

namespace Unity.Entities.VR.Interactions.Teleport
{
    public class TeleportPositionSystem : ComponentSystem
    {
        private ComponentGroup _teleportComponentGroup;
        private ComponentGroup _vrPlayerGroup;
        private ComponentGroup _vrCameraGroup;

        protected override void OnCreateManager()
        {
            _teleportComponentGroup = GetComponentGroup(typeof(VrTeleportPosition));
            _vrPlayerGroup = GetComponentGroup(typeof(Position), ComponentType.ReadOnly(typeof(VrPlayer)));
            _vrCameraGroup = GetComponentGroup(typeof(Position), typeof(Rotation), ComponentType.ReadOnly(typeof(VrCamera)));
        }

        protected override void OnUpdate()
        {
            var positions =  _teleportComponentGroup.GetComponentDataArray<VrTeleportPosition>();
            if(positions.Length == 0)return;

            var vrPlayerPos = _vrPlayerGroup.GetComponentDataArray<Position>();
            var vrPlayerPosition = vrPlayerPos[0];
            var vrCameraPos = _vrCameraGroup.GetComponentDataArray<Position>();
            var diff = vrCameraPos[0].Value - vrPlayerPosition.Value;
            diff.y = 0f;
            var newPosition = positions[0].position - diff;
            newPosition.y = vrPlayerPosition.Value.y;
            // Update pos
            vrPlayerPos[0] = new Position() {Value = newPosition};
            // Delete entity
            PostUpdateCommands.DestroyEntity(_teleportComponentGroup.GetEntityArray()[0]);
        }
    }
}
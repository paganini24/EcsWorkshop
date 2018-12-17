using Unity.Transforms;

namespace Unity.Entities.VR.Interactions.Teleport
{
    public class TeleportPositionSystem : ComponentSystem
    {
        private ComponentGroup _teleportComponentGroup;
        private ComponentGroup _vrPlayerGroup;

        protected override void OnCreateManager()
        {
            _teleportComponentGroup = GetComponentGroup(typeof(Position), typeof(TeleportPosition));
            _vrPlayerGroup = GetComponentGroup(typeof(Position), ComponentType.ReadOnly(typeof(VrPlayer)));
        }

        protected override void OnUpdate()
        {
            var positions =  _teleportComponentGroup.GetComponentDataArray<TeleportPosition>();
            if(positions.Length == 0)return;

            var vrPos = _vrPlayerGroup.GetComponentDataArray<Position>();
            vrPos[0] = new Position() {Value = positions[0].position};
            PostUpdateCommands.RemoveComponent<TeleportPosition>(_teleportComponentGroup.GetEntityArray()[0]);
        }
    }
}
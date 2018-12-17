using Jumper;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

namespace Common.SmallGame
{
    [AlwaysUpdateSystem]
    public class CameraMoveSystem : ComponentSystem
    {
        struct CameraMoveData
        {
            public readonly int Length;
            [ReadOnly]
            public SharedComponentDataArray<CameraMove> cameraComponentDataArray;
        }

        [Inject]
        private CameraMoveData _data;
        ComponentGroup _group;

        protected override void OnCreateManager()
        {
            _group = GetComponentGroup(typeof(Position), typeof(PlayerInput));
        }
        protected override void OnUpdate()
        {
            var entities = _group.GetComponentDataArray<Position>();
            for (int i = 0; i < entities.Length; i++)
            {
                var entityPosition = entities[i];
                var componentData = _data.cameraComponentDataArray[0];
                var cameraTransformPosition = componentData.cameraTransform.position;
                cameraTransformPosition.x = entityPosition.Value.x + componentData.offset;
                componentData.cameraTransform.position = cameraTransformPosition;
            }
        }
    }
}

using SmallGame;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

namespace Assets.Scripts.SmallGame
{
    [AlwaysUpdateSystem]
    public class CameraMoveSystem : ComponentSystem
    {
        struct CameraMoveData
        {
            public readonly int Length;
            [ReadOnly]public SharedComponentDataArray<CameraMove> cameraComponentDataArray;
        }

        [Inject]private CameraMoveData _data;

        protected override void OnUpdate()
        {
            var group = GetComponentGroup(typeof(Position), typeof(PlayerInput));
            var entities = group.GetComponentDataArray<Position>();
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

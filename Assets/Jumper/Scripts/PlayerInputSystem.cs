using Unity.Entities;
using UnityEngine;
namespace Jumper
{
    public struct PlayerInput : IComponentData
    {
        public float power;
        public int isJumping;
        public bool IsJumping => isJumping == 1;
    }
    public class PlayerInputSystem : ComponentSystem
    {
        private float _counter;

        struct PlayerInputData
        {
            public readonly int Length;
            public ComponentDataArray<PlayerInput> playerInput;
            public ComponentDataArray<PlayerJump> playerJump;
        }

        [Inject]private PlayerInputData playerInputData;

        protected override void OnUpdate()
        {
            for (int i = 0; i < playerInputData.Length; ++i)
            {
                var entity = playerInputData.playerInput[i];
                var jump = playerInputData.playerJump[i];
                if (Input.GetMouseButtonDown(0))
                {
                    entity.isJumping = 0;
                    _counter = 0f;
                }
                else if (Input.GetMouseButton(0))
                {
                    entity.isJumping = 0;
                    if(_counter < Bootstrap.settings.maxJumperHoldTime)
                        _counter += Time.deltaTime;
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    entity.power = _counter;
                    jump.time = Time.time;
                    entity.isJumping = 1;
                }

                playerInputData.playerInput[i] = entity;
                playerInputData.playerJump[i] = jump;
            }
        }
    }
}

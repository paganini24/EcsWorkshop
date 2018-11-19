using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;
namespace SmallGame
{
    public struct PlayerJump : IComponentData
    {
        public float time;
    }

    public class PlayerCheckBarrier : BarrierSystem{}

    public class PlayerMoveSystem : JobComponentSystem
    {
        [Inject]private PlayerCheckBarrier playerCheckBarrier;

        //[BurstCompile]
        struct PlayerMoveJob : IJobProcessComponentDataWithEntity<Position, PlayerInput, PlayerJump>
        {
            public float dt;
            public float time;
            public float jumpTime;
            public float jumpSpeed;
            public EntityCommandBuffer Commands;
            public void Execute(Entity entity, int index, ref Position position, ref PlayerInput input, ref PlayerJump jump)
            {
                if (input.IsJumping)
                {
                    var timeDiff = time - jump.time;
                    position.Value.x += dt * jumpSpeed * input.power;
                    position.Value.y = Easings.SineEaseOut(timeDiff * 2f / jumpTime);
                    if (timeDiff >= jumpTime)
                    {
                        input.isJumping = 0;
                        position.Value.y = 0f;
                        Commands.AddComponent(entity, new PlayerCheck());
                    }
                }
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            return new PlayerMoveJob
            {
                dt = Time.deltaTime,
                time = Time.time,
                jumpSpeed = Bootstrap.settings.jumpSpeed,
                jumpTime = Bootstrap.settings.jumpTime,
                Commands = playerCheckBarrier.CreateCommandBuffer()
            }.ScheduleSingle(this, inputDeps);
        }
    }
}

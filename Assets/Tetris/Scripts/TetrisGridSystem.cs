using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using UnityEngine;

namespace Tetris
{
    public class DeleteRowBarrierSystem : BarrierSystem{}

    public class TetrisGridSystem : ComponentSystem
    {
        public static readonly int Width = 10;
        public static readonly int Height = 20;

        private static readonly float2 BoundsX = new float2(-Width / 2f, Width / 2f);
        private static readonly float2 BoundsY = new float2(0f, -Height);

        public ComponentGroup blocksGroup;
        private DeleteRowBarrierSystem _deleteRowBarrierSystem;

        protected override void OnCreateManager()
        {
            blocksGroup = GetComponentGroup(typeof(Position), typeof(Block));
            
            if (World.Active != null)
                _deleteRowBarrierSystem = World.Active.GetOrCreateManager<DeleteRowBarrierSystem>();
        }

        protected override void OnUpdate()
        {
        }

        public bool InsideBorder(float2 pos)
        {
            return ((int) pos.x >= BoundsX.x && (int) pos.x <= BoundsX.y && (int) pos.y >= BoundsY.y);
        }

        public bool IsRowFull(int y)
        {
            var positions = blocksGroup.GetComponentDataArray<Position>();
            var count = 0;
            for (int i = 0; i < positions.Length; i++)
            {
                var pos = positions[i];
                if (math.abs(pos.Value.y - y) > 0.002f) continue;
                count++;
            }

            return count == Width;
        }

        public void DecreaseRowsAbove(int y)
        {
            var job = new DecreaseRowJob() {y = y};
            job.Schedule(this).Complete();
        }

        private void DeleteRow(int y)
        {
            var job = new DeleteRowJob() {y = y, commandBuffer = _deleteRowBarrierSystem.CreateCommandBuffer()};
            job.Schedule(this).Complete();
        }

        private void DeleteFullRows()
        {
            for (var y = -Height; y <= 0; y++)
            {
                if (IsRowFull(y))
                {
                    DeleteRow(y);
                    DecreaseRowsAbove(y + 1);
                    y--;
                }
            }
        }
        [BurstCompile]
        struct DecreaseRowJob : IJobProcessComponentData<Position, Block>
        {
            public int y;
            public void Execute(ref Position position, ref Block data1)
            {
                var pos = position.Value;
                if (pos.y > y)
                    pos.y -= 1f;
                var component = new Position() {Value = pos};
                position = component;
            }
        }

        struct DeleteRowJob : IJobProcessComponentDataWithEntity<Position, Block>
        {
            public int y;
            [ReadOnly] public EntityCommandBuffer commandBuffer;

            public void Execute(Entity entity, int index, ref Position position, ref Block data1)
            {
                var pos = position.Value;
                if (math.abs(pos.y - y) < 0.002f)
                {
                    commandBuffer.DestroyEntity(entity);
                }
            }
        }
    }
}
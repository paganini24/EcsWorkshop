using NUnit.Framework;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

namespace Tetris.Tests
{
    public class GridSystemTests
    {

        [Test]
        public void Is_Inside_Borders()
        {
            var world = new World("test");

            var manager = world.GetOrCreateManager<EntityManager>();
            var gridSystem = world.GetOrCreateManager<TetrisGridSystem>();
            EntityArchetype archetype = manager.CreateArchetype(typeof(Position), typeof(Rotation), typeof(Block),
                typeof(MeshInstanceRenderer));
            var row = -5;

            var entity = manager.CreateEntity(archetype);
            var componentData = new Position() {Value = new float3(0, row, 0f)};
            manager.SetComponentData(entity, componentData);

            Assert.True(gridSystem.InsideBorder(manager.GetComponentData<Position>(entity).Value.xy));
            
            
            var entityOut = manager.CreateEntity(archetype);
            var componentDataOut = new Position() {Value = new float3(-6, row, 0f)};
            manager.SetComponentData(entityOut, componentDataOut);
            
            Assert.False(gridSystem.InsideBorder(manager.GetComponentData<Position>(entityOut).Value.xy));

            
            world.DestroyManager(gridSystem);
            world.Dispose();
        }

        [Test]
        public void Is_Row_Full()
        {
            var world = new World("test");

            var manager = world.GetOrCreateManager<EntityManager>();
            var gridSystem = world.GetOrCreateManager<TetrisGridSystem>();
            EntityArchetype archetype = manager.CreateArchetype(typeof(Position), typeof(Rotation), typeof(Block),
                typeof(MeshInstanceRenderer));
            var row = -5;
            var entities = new NativeArray<Entity>(TetrisGridSystem.Width, Allocator.Temp);
            for (var i = -TetrisGridSystem.Width / 2; i < TetrisGridSystem.Width / 2; i++)
            {
                manager.CreateEntity(archetype,entities);
                manager.SetComponentData(entities[i+TetrisGridSystem.Width / 2], new Position() {Value = new float3(i, row, 0f)});
            }

            Assert.True(gridSystem.IsRowFull(row));
            world.DestroyManager(gridSystem);
            world.Dispose();
        }
        
        [Test]
        public void Is_Rows_Decreased()
        {
            var world = new World("test");
            var manager = world.GetOrCreateManager<EntityManager>();
            var gridSystem = world.GetOrCreateManager<TetrisGridSystem>();
            EntityArchetype archetype = manager.CreateArchetype(typeof(Position), typeof(Rotation), typeof(Block),
                typeof(MeshInstanceRenderer));
            var row = -5;
            for (var i = -TetrisGridSystem.Width / 2; i < TetrisGridSystem.Width / 2; i++)
            {
                var entity= manager.CreateEntity(archetype );
                manager.SetComponentData(entity, new Position() {Value = new float3(i, row, 0f)});
            }
            
            gridSystem.DecreaseRowsAbove(-6);
            
            var positions = gridSystem.blocksGroup.GetComponentDataArray<Position>();
            for (int i = 0; i < positions.Length; i++)
            {
                Assert.AreEqual(-6f , positions[i].Value.y);
            }
            
            world.DestroyManager(gridSystem);
            world.Dispose();
        }
        
        [Test]
        public void Is_Row_Deleted()
        {
            var world = new World("test");
            var manager = world.GetOrCreateManager<EntityManager>();
            var gridSystem = world.GetOrCreateManager<TetrisGridSystem>();
            EntityArchetype archetype = manager.CreateArchetype(typeof(Position), typeof(Rotation), typeof(Block),
                typeof(MeshInstanceRenderer));
            var row = -5;
            for (var i = -TetrisGridSystem.Width / 2; i < TetrisGridSystem.Width / 2; i++)
            {
                var entity= manager.CreateEntity(archetype );
                manager.SetComponentData(entity, new Position() {Value = new float3(i, row, 0f)});
            }
            var buff = new EntityCommandBuffer(Allocator.TempJob);
            
            new TestDeleteRowJob() {y = row, commandBuffer =buff}.Schedule(gridSystem).Complete();
            
            buff.Playback(manager);
            buff.Dispose();

            var entityArray = gridSystem.blocksGroup.GetEntityArray();
            Assert.AreEqual(0,entityArray.Length);
            
            world.DestroyManager(gridSystem);
            world.Dispose();
        }
        
        [Test]
        public void Is_FullRows_Deleted()
        {
            var world = new World("test");
            //dWorld.Active = world;
            var manager = world.GetOrCreateManager<EntityManager>();
            var gridSystem = world.GetOrCreateManager<TetrisGridSystem>();
            EntityArchetype archetype = manager.CreateArchetype(typeof(Position), typeof(Rotation), typeof(Block),
                typeof(MeshInstanceRenderer));
            var row = -5;
            for (var i = -TetrisGridSystem.Width / 2; i < TetrisGridSystem.Width / 2; i++)
            {
                var entity= manager.CreateEntity(archetype );
                manager.SetComponentData(entity, new Position() {Value = new float3(i, row, 0f)});
            }
            
            for (var y = -TetrisGridSystem.Height; y <= 0; y++)
            {
                if (gridSystem.IsRowFull(y))
                {
                    Debug.Log(y);
                    var buff = new EntityCommandBuffer(Allocator.TempJob);
                    new TestDeleteRowJob() {y = row, commandBuffer =buff}.Schedule(gridSystem).Complete();
                    buff.Playback(manager);
                    buff.Dispose();
                    // Decrease
                    gridSystem.DecreaseRowsAbove(y + 1);
                    y--;
                }
            }

            var entityArray = gridSystem.blocksGroup.GetEntityArray();
            Assert.AreEqual(0,entityArray.Length);
            
            world.DestroyManager(gridSystem);
            world.Dispose();
        }
        
        // Separated for test we need command buffer
        struct TestDeleteRowJob : IJobProcessComponentDataWithEntity<Position, Block>
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
using System.Collections.Generic;
using NUnit.Framework;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
namespace Tetris.Tests
{
    public class SpawnSystemTests 
    {
        [Test]
        public void Is_World_Created()
        {
            var world = new World("Test World");
            Assert.IsNotNull(world);
            
            world.Dispose();
        }

        [Test]
        public void Is_BlockData_Loaded()
        {
            var data = Resources.Load<TetrisTransformContainer>(BlockSpawnSystem.Block_O);
            Assert.IsNotNull(data);
        }
        
        [Test]
        public void Are_Block_Entities_Created()
        {
            var world = new World("Test World");
            var manager = world.GetOrCreateManager<EntityManager>();
            
            var data = Resources.Load<TetrisTransformContainer>(BlockSpawnSystem.Block_O);
            var entities = new NativeArray<Entity>(data.transformDatas.Count, Allocator.Temp);
            EntityArchetype archetype = manager.CreateArchetype(typeof(Position), typeof(Rotation), typeof(Block),
                typeof(MeshInstanceRenderer));
            
            for (var index = 0; index < data.transformDatas.Count; index++)
            {
                manager.CreateEntity(archetype, entities);
            }
            
            Assert.AreEqual(entities.Length, data.transformDatas.Count);

            for (var index = 0; index < entities.Length; index++)
            {
                var entity = entities[index];
                var blockData = data.transformDatas[index];
                manager.SetComponentData(entities[index], new Position() {Value = blockData.position});
                manager.SetComponentData(entities[index], new Rotation() {Value = blockData.rotation});
                manager.SetComponentData(entities[index], new Block() );
                manager.SetSharedComponentData(entities[index], data.renderer);
                
                var renderer = manager.GetSharedComponentData<MeshInstanceRenderer>(entity);
                Assert.IsNotNull(renderer);

                var pos = manager.GetComponentData<Position>(entity).Value;
                Debug.Log(pos);
                Assert.AreEqual(data.transformDatas[index].position, pos);
            }

            entities.Dispose();
            world.Dispose();
        }
        [Test]
        public void Are_Blocks_Not_Overlaping()
        {
            var world = new World("Test World");
            var manager = world.GetOrCreateManager<EntityManager>();
            
            var data = Resources.Load<TetrisTransformContainer>(BlockSpawnSystem.Block_O);
            var entities = new NativeArray<Entity>(data.transformDatas.Count, Allocator.Temp);
            EntityArchetype archetype = manager.CreateArchetype(typeof(Position), typeof(Rotation), typeof(Block),
                typeof(MeshInstanceRenderer));
            
            for (var index = 0; index < data.transformDatas.Count; index++)
            {
                manager.CreateEntity(archetype, entities);
            }
            
            Assert.AreEqual(entities.Length, data.transformDatas.Count);
            var positions = new List<float3>();
            for (var index = 0; index < entities.Length; index++)
            {
                var entity = entities[index];
                var blockData = data.transformDatas[index];
                manager.SetComponentData(entities[index], new Position() {Value = blockData.position});
                manager.SetComponentData(entities[index], new Rotation() {Value = blockData.rotation});
                manager.SetComponentData(entities[index], new Block());
                manager.SetSharedComponentData(entities[index], data.renderer);
                
                var pos = manager.GetComponentData<Position>(entity).Value;
                Debug.Log(pos);
                Assert.False(positions.Contains(pos));
                positions.Add(pos);
            }

            entities.Dispose();
            world.Dispose();
        }
    }
}
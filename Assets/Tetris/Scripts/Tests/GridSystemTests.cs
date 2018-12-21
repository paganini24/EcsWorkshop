using NUnit.Framework;
using Unity.Entities;

namespace Tetris.Tests
{
    public class GridSystemTests 
    {
        [Test]
        public void Is_Grid_Size_200()
        {
            var world = new World("test");

            var manager = world.GetOrCreateManager<EntityManager>();
            var archetype = manager.CreateArchetype(typeof(TetrisGrid));
            var entity = manager.CreateEntity(archetype);
            manager.SetComponentData(entity, new TetrisGrid(){sizeY = TetrisGridSystem.SizeY, sizeX = TetrisGridSystem.SizeX});


            var grid = manager.GetComponentData<TetrisGrid>(entity);
            
            Assert.AreEqual(200, grid.sizeX * grid.sizeY);
            
            world.Dispose();
        }
    }
}
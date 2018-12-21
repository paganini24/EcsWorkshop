using Unity.Entities;

namespace Tetris
{
    public struct TetrisGrid : IComponentData
    {
        public int sizeX;
        public int sizeY;
    }
}
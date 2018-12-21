using Unity.Entities;
using Unity.Mathematics;

namespace Tetris
{
    public struct Block : IComponentData
    {
        public float2 position;
    }
}
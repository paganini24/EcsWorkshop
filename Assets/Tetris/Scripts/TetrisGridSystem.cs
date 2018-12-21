using Unity.Entities;
using Unity.Mathematics;

namespace Tetris
{
    public class TetrisGridSystem : ComponentSystem
    {
        public static readonly int SizeX = 10;
        public static readonly int SizeY = 20;

        private static readonly float2 BoundsX = new float2(-SizeX /2f , SizeX /2f);
        private static readonly float2 BoundsY = new float2(0f, -SizeY);

        public float2[,] grid = new float2[SizeX,SizeY];
        
        protected override void OnUpdate()
        {
            
        }
        
        public static bool InsideBorder(float2 pos) {
            return ((int)pos.x >=  BoundsX.x && (int)pos.x <= BoundsX.y && (int)pos.y >= -BoundsY.y);
        }
    }
}
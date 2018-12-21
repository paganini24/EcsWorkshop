using System.Collections.Generic;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

namespace Tetris
{
    public class TetrisTransformContainer : ScriptableObject
    {
        public MeshInstanceRenderer renderer;
        public List<TetrisTransform> transformDatas;
    }
    [System.Serializable]
    public struct TetrisTransform
    {
        public float3 position;
        public quaternion rotation;
    }
}
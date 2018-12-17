using System;
using UnityEngine;
using UnityEngine.Rendering;
namespace Unity.Entities.VR.Rendering
{
    [Serializable]
    public struct TrackedMeshInstanceRenderer : ISharedComponentData
    {
        public Mesh                 mesh;
        public Material             material;
        public int                  subMesh;

        public ShadowCastingMode    castShadows;
        public bool                 receiveShadows;
    }

    public class TrackedMeshInstanceRendererComponent : SharedComponentDataWrapper<TrackedMeshInstanceRenderer> { }
}

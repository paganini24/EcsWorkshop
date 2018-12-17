using UnityEngine;

namespace Unity.Entities.VR
{
    [System.Serializable]
    public struct VrCamera : ISharedComponentData
    {
        public Camera camera;
    }
    public class VrCameraComponent : SharedComponentDataWrapper<VrCamera> { }
}
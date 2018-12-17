using UnityEngine.XR;

namespace Unity.Entities.VR
{
    public struct VrNodeData : IComponentData
    {
        public XRNode xrNode;
    }

    public struct Tracked : IComponentData { }
}

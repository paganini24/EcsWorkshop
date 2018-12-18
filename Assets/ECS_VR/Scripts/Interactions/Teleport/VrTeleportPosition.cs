using Unity.Mathematics;
using UnityEngine;

namespace Unity.Entities.VR.Interactions.Teleport
{
    public struct VrTeleportPosition : IComponentData
    {
        public float3 position;
    }

    public struct VrTeleportPositionIndicator : IComponentData
    {
        public float3 start, end;
    }
    public struct VrTeleport : IComponentData{}
}
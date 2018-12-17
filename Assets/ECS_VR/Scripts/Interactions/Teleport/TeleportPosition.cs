using Unity.Mathematics;
using UnityEngine;

namespace Unity.Entities.VR.Interactions.Teleport
{
    public struct TeleportPosition : IComponentData
    {
        public float3 position;
    }
    public struct TeleportBegin : IComponentData{    }
    public struct VrTeleport : IComponentData{    }
}
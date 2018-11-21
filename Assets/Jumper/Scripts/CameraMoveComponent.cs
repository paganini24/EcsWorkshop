using System;
using Unity.Entities;
using UnityEngine;
namespace Assets.Scripts.SmallGame
{
    [Serializable]
    public struct CameraMove : ISharedComponentData
    {
        public Transform cameraTransform;
        public float offset;
    }
    public class CameraMoveComponent : SharedComponentDataWrapper<CameraMove> { }
}

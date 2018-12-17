namespace Unity.Entities.VR
{
    [System.Serializable]
    public struct VrPlayer : ISharedComponentData{}
    public class VrPlayerComponent : SharedComponentDataWrapper<VrPlayer> { }
}
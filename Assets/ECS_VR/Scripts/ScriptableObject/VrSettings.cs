using UnityEngine;
namespace Unity.Entities.VR
{
    [CreateAssetMenu(fileName = "Ecs Vr Settings",menuName = "ECS Workshop/Vr Settings")]
    public class VrSettings : ScriptableObject
    {
        public Transform playerPrefab;
        public VrControllerModel vrControllerRenderer;
    }

    [System.Serializable]
    public class VrControllerModel
    {
        public Mesh mesh;
        public Material material;
    }
}

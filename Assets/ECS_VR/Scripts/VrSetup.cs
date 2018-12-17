using Unity.Entities.VR.Rendering;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR;

namespace Unity.Entities.VR
{
    public class VrSetup : MonoBehaviour
    {
        public VrSettings vrSettings;

        void Awake()
        {
            var manager = World.Active.GetOrCreateManager<EntityManager>();

            var vrControllerArchetype = manager.CreateArchetype(typeof(Position), typeof(Rotation),
                typeof(VrNodeData),typeof(TrackedMeshInstanceRenderer));

            // Controllers
            var controllerModel = new TrackedMeshInstanceRenderer
            {
                material = vrSettings.vrControllerRenderer.material,
                mesh = vrSettings.vrControllerRenderer.mesh,
                castShadows = ShadowCastingMode.On,
                receiveShadows = true
            };
            var leftController = manager.CreateEntity(vrControllerArchetype);
            manager.SetComponentData(leftController, new VrNodeData() {xrNode = XRNode.LeftHand});
            manager.SetSharedComponentData(leftController, controllerModel);

            var rightController = manager.CreateEntity(vrControllerArchetype);
            manager.SetComponentData(rightController, new VrNodeData() {xrNode = XRNode.RightHand});
            manager.SetSharedComponentData(rightController, controllerModel);

            // Player
            var player = CreatePlayer();
            
            // Attach
            var attachLeft = manager.CreateEntity(typeof(Attach));
            var attachRight = manager.CreateEntity(typeof(Attach));
            manager.SetComponentData(attachLeft, new Attach {Parent = player, Child = leftController });
            manager.SetComponentData(attachRight, new Attach { Parent = player, Child = rightController});
        }
        /// <summary>
        /// Creates hybrid vr player with camera
        /// </summary>
        /// <returns></returns>
        private Entity CreatePlayer()
        {
            // Player
            var playerGameobject = new GameObject("VrPlayer");
            var gameObjectEntity = playerGameobject.AddComponent<GameObjectEntity>();
            playerGameobject.AddComponent<PositionComponent>();
            playerGameobject.AddComponent<RotationComponent>();
            playerGameobject.AddComponent<CopyTransformToGameObjectComponent>();
            playerGameobject.AddComponent<VrPlayerComponent>();
            
            // Camera transform
            var vrCamera = new GameObject("Camera");
            vrCamera.transform.SetParent(playerGameobject.transform);
            vrCamera.AddComponent<AudioListener>();
            vrCamera.AddComponent<GameObjectEntity>();
            vrCamera.AddComponent<PositionComponent>();
            vrCamera.AddComponent<RotationComponent>();
            // Camera
            var cam = vrCamera.AddComponent<Camera>();
            cam.stereoTargetEye = StereoTargetEyeMask.Both;
            cam.nearClipPlane = 0.01f;
            return gameObjectEntity.Entity;
        }
    }
}
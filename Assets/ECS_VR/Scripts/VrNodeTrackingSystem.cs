using Unity.Burst;
using Unity.Collections;
using Unity.Transforms;
using UnityEngine.XR;

namespace Unity.Entities.VR
{
    public class TrackingBarrier : BarrierSystem { }
    public class VrNodeTrackingSystem : ComponentSystem
    {
        private TrackingBarrier _trackingBarrier;
        protected override void OnCreateManager()
        {
            InputTracking.trackingAcquired += TrackingAcquired;
            InputTracking.trackingLost += TrackingLost;
            _trackingBarrier = World.Active.GetOrCreateManager<TrackingBarrier>();
        }

        protected override void OnUpdate(){}

        protected override void OnDestroyManager()
        {
            InputTracking.trackingAcquired -= TrackingAcquired;
            InputTracking.trackingLost -= TrackingLost;
        }

        private void TrackingAcquired(XRNodeState xrNodeState)
        {
            var job = new TrackJob {state = xrNodeState, command = _trackingBarrier.CreateCommandBuffer(),tracked = true};
            job.ScheduleSingle(this).Complete();
        }

        private void TrackingLost(XRNodeState xrNodeState)
        {
            var job = new TrackJob { state = xrNodeState, command = _trackingBarrier.CreateCommandBuffer() ,tracked = false};
            job.ScheduleSingle(this).Complete();
        }

        struct TrackJob : IJobProcessComponentDataWithEntity<VrNodeData, Position>
        {
            public XRNodeState state;
            public bool tracked;
            [ReadOnly]public EntityCommandBuffer command; // Not supported by Burst currently
            public void Execute(Entity entity, int index, ref VrNodeData vrNodeData, ref Position data1)
            {
                if (state.nodeType != vrNodeData.xrNode)
                    return;
                if (tracked) // If tracked add tracked component
                    command.AddComponent(entity, new Tracked());
                else // Otherwise remove it 
                    command.RemoveComponent<Tracked>(entity);
            }
        }
    }
}
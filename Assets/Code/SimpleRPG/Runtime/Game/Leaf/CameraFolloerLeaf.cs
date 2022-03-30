using NullFramework.Runtime;
using UnityEngine;

namespace SimpleRPG.Runtime
{
    public class CameraFollowLeaf : Leaf<CameraFollowLeafData>
    {
        private GameObject camera;
        private Vector3 currentVelocity;
        
        public override void OnReciveDataFinish()
        {
            camera = leafData.InstantiateCamera();
        }

        protected override void InitListeners()
        {
            AddMsgListener(GameMsgKind.FollowTarget, FollowTarget);
        }

        private System.Action FollowTarget(Msg msg)
        {
            var msgData = msg.GetData<MsgData_FollowTarget>();
            var target = msgData.lookTarget;
            if(!target)
            {
                return null;
            }
            var targetPosition = target.transform.position;
            targetPosition -= camera.transform.forward * leafData.zdistance;
            var current = camera.transform.position;
            current = Vector3.SmoothDamp(current, targetPosition, ref currentVelocity, leafData.smoothTime, leafData.maxSpeed, Root.Instance.DeltaTime);
            camera.transform.position = current;
            return null;
        }

    }
}


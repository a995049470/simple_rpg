using NullFramework.Runtime;
using UnityEngine;

namespace SimpleRPG.Runtime
{
    public class CameraFollowLeaf : Leaf<CameraFollowLeafData>
    {
        private GameObject camera;
        private Vector3 currentVelocity;

        private float zdistance;
        private float smoothTime;
        private float maxSpeed;
        private Transform lookTarget;
        
        
        public override void OnReciveDataFinish()
        {
            camera = leafData.InstantiateCamera();
        }

        protected override void OnObjectInstantiate(MsgData_ObjectInstantiate data)
        {
            base.OnObjectInstantiate(data);
            if(data.obj is GameObject go)
            {
                lookTarget = go.transform.Find("look");
            }
        }

        protected override void InitListeners()
        {
            AddMsgListener(GameMsgKind.FollowTarget, FollowTarget);
            this.zdistance = leafData.zdistance;
            this.smoothTime = leafData.smoothTime;
            this.maxSpeed = leafData.maxSpeed;
        }
        
        private System.Action FollowTarget(Msg msg)
        {
            var msgData = msg.GetData<MsgData_FollowTarget>();
            if(!lookTarget)
            {
                return null;
            }
            var targetPosition = lookTarget.position;
            targetPosition -= camera.transform.forward * zdistance;
            var current = camera.transform.position;
            current = Vector3.SmoothDamp(current, targetPosition, ref currentVelocity, smoothTime, maxSpeed, root.RealDeltaTime);
            camera.transform.position = current;
            return null;
        }

    }
}


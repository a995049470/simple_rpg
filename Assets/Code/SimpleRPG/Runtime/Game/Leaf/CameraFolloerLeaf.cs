using NullFramework.Runtime;
using UnityEngine;

namespace SimpleRPG.Runtime
{
    public class CameraFollowLeaf : Leaf<CameraFollowLeafData>, IUnityObjectLoader, IUnityObjectSetter
    {
        private GameObject camera;
        private GameObject target;
        private Vector3 currentVelocity;
        public void LoadUnityObject()
        {
            camera = data.InstantiateCamera();
        }

        public void SetUnityObject(Object obj)
        {
            target = (obj as GameObject).transform.Find("look").gameObject;
        }

        protected override void InitListeners()
        {
            AddMsgListener(MsgKind.custom, FollowTarget);
        }

        private void FollowTarget(Msg msg)
        {
            var targetPosition = target.transform.position;
            targetPosition -= camera.transform.forward * data.zdistance;
            var current = camera.transform.position;
            current = Vector3.SmoothDamp(current, targetPosition, ref currentVelocity, data.smoothTime, data.maxSpeed, Root.Instance.DeltaTime);
            camera.transform.position = current;
        }

        
    }
}


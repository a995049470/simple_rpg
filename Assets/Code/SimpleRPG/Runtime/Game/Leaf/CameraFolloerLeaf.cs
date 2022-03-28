using NullFramework.Runtime;
using UnityEngine;

namespace SimpleRPG.Runtime
{
    public class CameraFollowLeaf : Leaf<CameraFollowLeafData>, ILeafReciver
    {
        private GameObject camera;
        private GameObject target;
        private Vector3 currentVelocity;
        

        public void ReciveLeaf(Leaf leaf)
        {
            if(leaf is PlayerTress playerTress)
            {
                target = playerTress.Player.transform.Find("look").gameObject;
            }
        }

        public override void OnReciveDataFinish()
        {
            camera = leafData.InstantiateCamera();
        }

        protected override void InitListeners()
        {
            AddMsgListener(GameMsgKind.custom, FollowTarget);
        }

        private void FollowTarget(Msg msg)
        {
            var targetPosition = target.transform.position;
            targetPosition -= camera.transform.forward * leafData.zdistance;
            var current = camera.transform.position;
            current = Vector3.SmoothDamp(current, targetPosition, ref currentVelocity, leafData.smoothTime, leafData.maxSpeed, Root.Instance.DeltaTime);
            camera.transform.position = current;
        }

    }
}


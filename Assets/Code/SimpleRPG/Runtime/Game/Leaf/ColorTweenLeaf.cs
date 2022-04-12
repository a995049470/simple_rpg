using NullFramework.Runtime;
using UnityEngine;

namespace SimpleRPG.Runtime
{
    public class ColorTweenLeaf : Leaf<ColorTweenLeafData>
    {
        private Color endColor;
        private float time;
        private AnimationCurve curve;   

        //只存在一个
        private MsgData_ColorTween cache;
        
        protected override void InitListeners()
        {
            base.InitListeners();
            AddMsgListeners
            (
                (GameMsgKind.ColorTween, ColorTween),
                (GameMsgKind.Hit, Hit)
            ); 
        }

        private System.Action ColorTween(Msg msg)
        {
            var msgData = msg.GetData<MsgData_ColorTween>();
            if(msgData != cache) return emptyAction;
            bool isComplete = msgData.Update(root.RealDeltaTime);
            
            if(!isComplete)
            {
                root.AddNextFrameMsg(msg);
            }
            else
            {
            #if UNITY_EDITOR
                Debug.Assert(cache == msgData);
            #endif
                cache = null;
            }
            return emptyAction;
        }

        private System.Action Hit(Msg msg)
        {
            var msgData = msg.GetData<MsgData_Hit>();
            var obj = msgData.unitObj;
            System.Action cb = null;
            if(obj == null || msgData.effectCount <= 0) return cb;
            var colorChanger = obj.GetComponentInChildren<IColorChanger>();
            if (colorChanger == null) return cb;
            if(cache != null)
            {
                cache.Compelete();
                cache = null;
            }
            cache = new MsgData_ColorTween();
            cache.curve = curve;
            cache.endColor = endColor;
            cache.startColor = colorChanger.GetColor();
            cache.time = time;
            cache.timer = 0;
            cache.colorChanger = colorChanger;
            var msg_colorTween = new Msg(GameMsgKind.ColorTween, cache, this, this);
            root.AddNextFrameMsg(msg_colorTween);
            msgData.effectCount --;
            return cb;
        }

        public override void OnReciveDataFinish()
        {
            base.OnReciveDataFinish();
            this.endColor = leafData.endColor;
            this.time = leafData.time;
            this.curve = leafData.curve;
        }


    }   
}


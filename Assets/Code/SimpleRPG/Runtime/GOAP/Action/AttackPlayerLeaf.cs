using System;
using NullFramework.Runtime;

namespace SimpleRPG.Runtime
{
    public class MoveToAttackTargetLeaf : GoapActionLeaf<MoveToAttackTargetLeafData>
    {
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override bool Execute(StateSet worldStates)
        {
            return base.Execute(worldStates);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool IsDone()
        {
            return base.IsDone();
        }

        public override void LoadData(LeafData data)
        {
            base.LoadData(data);
        }

        public override void OnDisable(Leaf nextHandle = null)
        {
            base.OnDisable(nextHandle);
        }

        public override void OnEnable(Leaf lastLeaf = null)
        {
            base.OnEnable(lastLeaf);
        }

        public override void OnFree()
        {
            base.OnFree();
        }

        public override void OnReciveDataFinish()
        {
            base.OnReciveDataFinish();
        }

        public override bool OnUpdate(Msg msg)
        {
            return base.OnUpdate(msg);
        }

        public override void SetLeafData(LeafData data)
        {
            base.SetLeafData(data);
        }

        public override string ToString()
        {
            return base.ToString();
        }

        protected override void InitListeners()
        {
            base.InitListeners();
        }

        protected override void Reset()
        {
            base.Reset();
        }
    }
    //发起攻击
    public class AttackPlayerLeaf : GoapActionLeaf<AttackPlayerLeafData>
    {
        private MsgData_Attack cacheData;
        private int attackState;
        private float attackStartFrame;
        private const int maxIntervalFrame = 4;

        protected override void Reset()
        {
            cacheData = null;
            attackStartFrame = root.CurrentTime;
        }
        
        public override bool IsDone()
        {
            return attackState == RunState.Success;
        }

        private bool IsTimeOut()
        {
            return root.Frame - attackStartFrame > maxIntervalFrame;
        }

        private bool IsContineExecute(int state)
        {
            bool IsContineExecute = true;
            if((state == RunState.None && IsTimeOut()) || 
               state == RunState.Fail)
            {
                IsContineExecute = false;
            }
            return IsContineExecute;
        }


        public override bool Execute(StateSet worldStates)
        {
            bool isContinue = true;
            if(cacheData == null)
            {   
                attackStartFrame = root.Frame;
                //发起战斗
                var msgdata = new MsgData_Attack();
                cacheData = msgdata;
                msgdata.attackerFilter = UnitKindHelper.AllKinds();
                var launcher = worldStates[((int)GameStateType.Launcher)].Get<BattleUnit>();
                var msg = new Msg(GameMsgKind.Attack, msgdata, this, launcher.leaf);
                root.AddNextFrameMsg(msg);
            }
            else
            {     
                isContinue = IsContineExecute(cacheData.attackState);
            }
            return isContinue;
        }
    }
}
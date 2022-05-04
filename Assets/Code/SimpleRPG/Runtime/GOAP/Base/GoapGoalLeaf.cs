using System.Collections;
using System.Collections.Generic;
using NullFramework.Runtime;
using UnityEngine;

namespace SimpleRPG.Runtime
{
    public class GoapGoalLeaf<T> : BaseGoapGoalLeaf where T : LeafData, IGoapGoalLeafData
    {
        protected T leafData;

        public virtual void OnReciveDataFinish() {}

        public virtual void SetLeafData(LeafData data)
        {
            this.leafData = data as T;
            var goalData = data as IGoapGoalLeafData;
            goalStates = goalData.CreateGoalStates();
            priority = goalData.GetPriority();
        }

        protected void Collect(GameStateType stateType, int msgKind, StateSet worldStates, Leaf reciver = null)
        {
            int key = ((int)stateType);
            if(worldStates.ContainsKey(key)) return; 
            var msgdata = new MsgData_Collect();
            var state = new State(key);
            msgdata.state = state;
            msgdata.worldStates = worldStates;
            var msg = new Msg(msgKind, msgdata, this, reciver);
            root.SyncExecuteMsg(msg);
            if(state.IsVaild)
            {
                worldStates[key] = state;
            }
        }
    }
}

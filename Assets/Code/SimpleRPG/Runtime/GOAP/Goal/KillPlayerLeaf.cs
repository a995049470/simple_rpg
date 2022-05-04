using System;
using NullFramework.Runtime;

namespace SimpleRPG.Runtime
{
    public class KillPlayerLeaf : GoapGoalLeaf<KillPlayerLeafData>
    {
        protected override Action CollectWorldState(Msg msg)
        {
            var msgdata = msg.GetData<MsgData_CollectWorldState>();
            var worldStates = msgdata.worldStates;

            //收集发起者
            Collect(GameStateType.Launcher, GameMsgKind.Collect_Launcher, worldStates, msgdata.launcher);

            Collect(GameStateType.AttackTarget, GameMsgKind.Collect_Player, worldStates);
            
            //前置attacktarget launcher
            Collect(GameStateType.InAttackRange, GameMsgKind.Collect_InAttackRange, worldStates, msgdata.launcher);

            Collect(GameStateType.AttackCoolDown, GameMsgKind.Collect_AttackCoolDown, worldStates, msgdata.launcher);

            return emptyAction;
        }
    }
}
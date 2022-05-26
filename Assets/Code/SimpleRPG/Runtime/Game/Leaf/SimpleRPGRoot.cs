using NullFramework.Runtime;
using UnityEngine;
namespace SimpleRPG.Runtime
{
    public class SimpleRPGRoot : Root
    {
        private int[] lastFrames = new int[8];
        
        protected override void AddFixedMsgs()
        {
            base.AddFixedMsgs();
            //发起一个摄像机跟随的固定指令
            {
                var data_followTarget = new MsgData_FollowTarget();
                AddMsg(new Msg(GameMsgKind.FollowTarget, data_followTarget, this));
                var data_rigidUpdate = new MsgData_RigidUpdate();
                AddMsg(new Msg(BaseMsgKind.RigidUpdate, data_rigidUpdate, this));
            }
            //发起一个ai运行的指令
            {
                var msgdata = new MsgData_GoapUpdate();
                msgdata.filter = UnitKindHelper.GetUnitKinds(UnitKind.Enemy);
                AddMsg(new Msg(BaseMsgKind.GoapUpdate, msgdata, this));
            }
        }

        protected override void InputProcess()
        {
            base.InputProcess();
            Vector3 dir = Vector3.zero;
            if(GetKey(0, KeyCode.W, KeyCode.UpArrow))
            {
                dir.z += 1;
            }
            if(GetKey(1, KeyCode.S, KeyCode.DownArrow))
            {
                dir.z -= 1;
            }
            if(GetKey(2, KeyCode.A, KeyCode.LeftArrow))
            {
                dir.x -= 1;
            }
            if(GetKey(3, KeyCode.D, KeyCode.RightArrow))
            {
                dir.x += 1;
            }
            if(dir.magnitude > 0)
            {
                if(dir.magnitude > 1) dir = dir.normalized;
                var data_move = new MsgData_Move();
                data_move.dir = dir;
                data_move.strength = 1;
                data_move.unitType = UnitKindHelper.GetUnitKinds(UnitKind.Player);
                AddMsg(new Msg(GameMsgKind.Move, data_move, this));
            }

            if(GetKeyDown(4, KeyCode.J, KeyCode.DownArrow))
            {
                var msgdata = new MsgData_Attack();
                msgdata.attackerFilter = UnitKindHelper.GetUnitKinds(UnitKind.Player);
                AddMsg(new Msg(GameMsgKind.Attack, msgdata, this));
            }
        }

        private bool GetKey(int index, params KeyCode[] keyCodes)
        {
            return IsLeaglInput(Input.GetKey, index, keyCodes);
        }

        private bool GetKeyDown(int index, params KeyCode[] keyCodes)
        {
            return IsLeaglInput(Input.GetKeyDown, index, keyCodes);
        }

        

        private bool IsLeaglInput(System.Func<KeyCode, bool> func, int index, params KeyCode[] keyCodes)
        {
            var isGetKeyDown = false;

            if(lastFrames[index] != root.Frame && func != null)
            {
                foreach (var keyCode in keyCodes)
                {
                    if(func.Invoke(keyCode))
                    {
                        isGetKeyDown = true;
                        lastFrames[index] = root.Frame;
                        break;
                    }
                }
            }
            return isGetKeyDown;
        }
    }
}


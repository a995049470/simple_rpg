using NullFramework.Runtime;
using UnityEngine;
namespace SimpleRPG.Runtime
{
    public class SimpleRPGRoot : Root
    {
        
        protected override void BeforeUpdate()
        {
            base.BeforeUpdate();
            AddMsg(new Msg(GameMsgKind.custom));
        }
    }
}


using NullFramework.Runtime;
using Sirenix.OdinInspector;
using UnityEngine;
namespace SimpleRPG.Runtime
{

    [SerializeField]
    public class StateData
    {
        [SerializeField]
        private GameStateType stateKind;
        [ShowIf("isInt")][SerializeField]
        private int value_int;
        [ShowIf("isFloat")][SerializeField]
        private float vlaue_float;
        [ShowIf("isBool")][SerializeField]
        private bool value_bool;
        //[ShowIf("isObject")][SerializeField]
        private object value_object;
        [SerializeField]
        private ValueType type = ValueType.Bool;
        public bool isInt { get => type == ValueType.Int; }
        public bool isBool { get => type == ValueType.Bool; }
        public bool isFloat { get => type == ValueType.Float; }
        public bool isObject { get => type == ValueType.Object; }

        public State Create()
        {
            State state;
            if(isInt) state = new State((int)stateKind, value_int);
            else if(isBool) state = new State((int)stateKind, value_bool);
            else if(isFloat) state = new State((int)stateKind, vlaue_float);
            else if(isObject) state = new State((int)stateKind, value_object);
            else state = new State((int)stateKind);
            return state;
        }
    }
}

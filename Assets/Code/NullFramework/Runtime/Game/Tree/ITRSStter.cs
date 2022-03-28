using UnityEngine;

namespace NullFramework.Runtime
{
    public interface ITRSSetter
    {
        void SetTRS(Vector3 _position, Quaternion _rotation, Vector3 _scale);
    }
}

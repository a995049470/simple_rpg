using NullFramework.Runtime;
using UnityEngine;
namespace SimpleRPG.Runtime
{
    [CreateAssetMenu(fileName = "ColorTweenLeafData", menuName = "SimpleRPG/ColorTweenLeafData")]
    public class ColorTweenLeafData : LeafData<ColorTweenLeaf>
    {
        public float time;
        public Color endColor;
        public AnimationCurve curve;
    }
}


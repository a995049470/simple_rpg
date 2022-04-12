using NullFramework.Runtime;
using UnityEngine;

namespace SimpleRPG.Runtime
{
    public class MsgData_ColorTween : MsgData
    {   
        public Color startColor;
        public Color endColor;
        public float timer;
        public float time;
        public AnimationCurve curve;
        public IColorChanger colorChanger;

        public bool Update(float deltaTime)
        {
            timer += deltaTime;
            UpdateColor();
            
            return timer >= time;
        }
        public void Compelete()
        {
            timer = time;
            UpdateColor();
        }

        private void UpdateColor()
        {
            float t = timer / time;
            t = Mathf.Clamp01(t);
            t = curve.Evaluate(t);
            var color = Color.Lerp(startColor, endColor, t);
            colorChanger.SetColor(color);
        }
    }
}

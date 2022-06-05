using LitJson;
using UnityEngine;
namespace NullFramework.Runtime
{
    public static class ExtensionMethod
    {
        public static int ConvertToInt(this JsonData jsonData)
        {
            return ((int)jsonData);
        }

        public static double ConvertToDouble(this JsonData jsonData)
        {
            return ((double)jsonData);
        }

        public static float ConvertToFloat(this JsonData jsonData)
        {
            return (float)(double)jsonData;
        }

        public static long ConvertToLong(this JsonData jsonData)
        {
            return (long)jsonData;
        }

        public static string ConvertToString(this JsonData jsonData)
        {
            return (string)jsonData;
        }
        
        public static Matrix4x4 Mulit(this Matrix4x4 m, float f)
        {
            Matrix4x4 res = Matrix4x4.zero;
            for (int i = 0; i < 16; i++)
            {
                res[i] = m[i] * f;
            }
            return res;
        }

        public static Matrix4x4 Add(this Matrix4x4 left, Matrix4x4 right)
        {
            Matrix4x4 res = Matrix4x4.zero;
            for (int i = 0; i < 16; i++)
            {
                res[i] = left[i] + right[i];
            }
            return res;
        }

        public static Matrix4x4 Sub(this Matrix4x4 left, Matrix4x4 right)
        {
            Matrix4x4 res = Matrix4x4.zero;
            for (int i = 0; i < 16; i++)
            {
                res[i] = left[i] - right[i];
            }
            return res;
        }

        public static Quaternion Add(this Quaternion left, Quaternion right)
        {
            Quaternion res = Quaternion.identity;
            for (int i = 0; i < 4; i++)
            {
                res[i] = left[i] + right[i];
            }
            return res;
        }

        public static float Trace(this Matrix4x4 m)
        {
            // var t = 0.0f;
            // for (int i = 0; i < 4; i++)
            // {
            //     t += m[i, i];
            // }
            // return t;
            return m[0, 0] + m[1, 1] + m[2, 2] + m[3, 3];
        }

        
    }
}
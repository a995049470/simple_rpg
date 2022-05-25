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
            for (int i = 0; i < 16; i++)
            {
                m[i] *= f;
            }
            return m;
        }

        public static Matrix4x4 Add(this Matrix4x4 left, Matrix4x4 right)
        {
            for (int i = 0; i < 16; i++)
            {
                left[i] += right[i];
            }
            return right;
        }

        public static Matrix4x4 Sub(this Matrix4x4 left, Matrix4x4 right)
        {
            for (int i = 0; i < 16; i++)
            {
                left[i] -= right[i];
            }
            return right;
        }

        public static Quaternion Add(this Quaternion left, Quaternion right)
        {
            for (int i = 0; i < 4; i++)
            {
                left[i] += right[i];
            }
            return left;
        }

        
    }
}
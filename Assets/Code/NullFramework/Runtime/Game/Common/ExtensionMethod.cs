using LitJson;

namespace NullFramework.Runtime
{
    public static class ExtensionMethod {
        public static int ConvertToInt(this JsonData jsonData)
        {
            return ((int)jsonData);
        }

        public static double ConvertToDouble(this JsonData jsonData)
        {
            return ((double)jsonData);
        }

        public static double ConvertToFloat(this JsonData jsonData)
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
    }
}
using System;
using System.Runtime.InteropServices;

namespace NullFramework.Runtime
{
    public static class StructUtility
    {
        public static byte[] StructToBytes<T>(T value) where T : unmanaged
        {
            int size = Marshal.SizeOf(value);
            byte[] arr = new byte[size];

            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(value, ptr, true);
            Marshal.Copy(ptr, arr, 0, size);
            Marshal.FreeHGlobal(ptr);
            return arr;
        }

        public static T BytesToStruct<T>(byte[] arr) where T : unmanaged
        {
            T str = new T();

            int size = Marshal.SizeOf(str);
            IntPtr ptr = Marshal.AllocHGlobal(size);

            Marshal.Copy(arr, 0, ptr, size);

            str = Marshal.PtrToStructure<T>(ptr);
            Marshal.FreeHGlobal(ptr);

            return str;
        }

        public static byte[] ArrayToBytes<T>(T[] array) where T : unmanaged
        {
            int len = array.Length;
            int size = Marshal.SizeOf(array[0]);
            byte[] res = new byte[size * len];
            IntPtr ptr = Marshal.AllocHGlobal(size);
            for (int i = 0; i < len; i++)
            {
                Marshal.StructureToPtr(array[i], ptr, true);
                Marshal.Copy(ptr, res, size * i, size);
            }
            Marshal.FreeHGlobal(ptr);
            return res;
        }

        public static T[] BytesToArray<T>(byte[] arr) where T : unmanaged
        {
            var total = arr.Length;
            var size = Marshal.SizeOf<T>();
            var len = total / size;
            var res = new T[len];
            IntPtr ptr = Marshal.AllocHGlobal(size);
            for (int i = 0; i < len; i++)
            {
                Marshal.Copy(arr, i * size, ptr, size);
                var value = Marshal.PtrToStructure<T>(ptr);
                res[i] = value;
            }
            Marshal.FreeHGlobal(ptr);
            return res;
        }
    }
}
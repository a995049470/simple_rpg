using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using NullFramework.Editor;
using LitJson;
using SimpleRPG.Runtime;
using System.Runtime.InteropServices;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using NullFramework.Runtime;

namespace Tests
{
    

    public class SimpleTest
    {
        float RadicalInverse_VdC(uint bits) 
        {
            bits = (bits << 16) | (bits >> 16);
            bits = ((bits & 0x55555555u) << 1) | ((bits & 0xAAAAAAAAu) >> 1);
            bits = ((bits & 0x33333333u) << 2) | ((bits & 0xCCCCCCCCu) >> 2);
            bits = ((bits & 0x0F0F0F0Fu) << 4) | ((bits & 0xF0F0F0F0u) >> 4);
            bits = ((bits & 0x00FF00FFu) << 8) | ((bits & 0xFF00FF00u) >> 8);
            return (float)(bits * 2.3283064365386963e-10); // / 0x100000000
        }
        // ----------------------------------------------------------------------------
        Vector2 Hammersley(uint i, uint N)
        {
            return new Vector2((float)i/(float)N, RadicalInverse_VdC(i));
        }  
        
        [Test]
        public void HammersleyTest()
        {
            int count = 4096;
            string str = "";
            for (int i = 0; i < count; i++)
            {
                var h = Hammersley((uint)i, (uint)count);
                str += $"{h.x}\t{h.y}\n";
            }
            string path = "D:\\StudyData\\UnityPorjects\\SimpleRPG\\Assets\\Code\\SimpleRPG\\Tests\\Editor\\reslut.txt";
            System.IO.File.WriteAllText(path, str);
            UnityEditor.AssetDatabase.Refresh();
        }

        [Test]
        public void BytesTest()
        {
            CubeGBuffer buffer = new CubeGBuffer();
            buffer.ao = 1;
            var bytes = StructUtility.StructToBytes(buffer);
            buffer = StructUtility.BytesToStruct<CubeGBuffer>(bytes);
            Assert.AreEqual(buffer.ao, 1);

            CubeGBuffer[] buffers = new CubeGBuffer[512];
            buffers[0].ao = 100;
            buffers[1].ao = 200;
            bytes = StructUtility.ArrayToBytes(buffers);
            buffers = StructUtility.BytesToArray<CubeGBuffer>(bytes);
            Assert.AreEqual(buffers[0].ao, 100);
            Assert.AreEqual(buffers[1].ao, 200);
            
        }

        


        
       

       
    }
}

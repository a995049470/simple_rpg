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
    public class JsonWindowTest : JsonWindow<JsonWindowTest>
    {
        public int value1;
        public string value2;
        public float value3;
        public Texture2D pic;
    }

    public class SimpleTest
    {
        
        [Test]
        public void JsonTest()
        {
            JsonWindowTest t = new JsonWindowTest();
            t.LoadData();
            Debug.Log(t.value1);
            Debug.Log(t.value2);
            Debug.Log(t.value3);
            Debug.Log(t.pic.name);

            Debug.Log(typeof(UnityEngine.Object).IsAssignableFrom(typeof(MonoBehaviour)));
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

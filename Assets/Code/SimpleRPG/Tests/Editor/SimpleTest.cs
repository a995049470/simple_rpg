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

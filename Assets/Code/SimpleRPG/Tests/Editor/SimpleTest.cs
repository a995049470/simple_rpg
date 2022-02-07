using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using NullFramework.Editor;
using LitJson;

namespace Tests
{
    public class SimpleTest
    {
        
        [Test]
        public void JsonTest()
        {
            var value1 = SerializationHelper.GetValue<float>("test", "value1");
            var value2 = SerializationHelper.GetValue<string>("test", "value2");
            var value3 = SerializationHelper.GetValue<int>("test", "value3");
            var value4 = SerializationHelper.GetValue<float[]>("test", "value4");
            var value5 = SerializationHelper.GetValue<int[]>("test", "value5");
            var value6 = SerializationHelper.GetValue<string[]>("test", "value6");
            

        }

       
    }
}

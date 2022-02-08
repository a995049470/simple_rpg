using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using NullFramework.Editor;
using LitJson;

namespace Tests
{
    public class JsonWindowTest : JsonWindow
    {
        public int value1;
        public string value2;
        public float value3;
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
        }

       
    }
}

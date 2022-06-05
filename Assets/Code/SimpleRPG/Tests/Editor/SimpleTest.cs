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

        [Test]
        public void RingArrayTest()
        {
            var ring = new RingArray<int>(4);
            for (int i = 0; i < 20; i++)
            {
                if(i > 4 && UnityEngine.Random.value < 0.5f )
                {
                    ring.Dequeue();
                }
                else
                {
                    ring.Push(i);
                }
                Debug.Log(ring);
            }

        }

        public class A
        {
            public static int value;
        }

        public class B : A
        {

        }

        [Test]
        public void TestCalss()
        {
            A.value = 10;
            B.value = 20;
            Debug.Log(A.value + "  " + B.value);
        }

        public class TestVM : VM
        {
            public void OutPut()
            {
                var str = "";
                foreach (var kvp in symbolDic)
                {
                    str += $"{kvp.Key} : {kvp.Value}";
                }
                Debug.Log(str);
            }

            [VMMethod]
            public void Wait()
            {
                var methodPtr = GetCurrentMethodPtr();
                var time = PopInt();
                var timer = PopInt();
                Debug.Log($"第{timer}帧率");
                timer++;
                
                if (timer < time)
                {
                    SetPrt(methodPtr);
                    Push(timer);
                    Push(time);
                }
            }

            [VMMethod]
            public void A0()
            {

            }
             
            [VMMethod]
            public void A1()
            {

            }

            [VMMethod]
            public void C0()
            {

            }
        }

        [Test]
        public void VMTest()
        {
            var vm = new TestVM();
            vm.OutPut();
            string code = "num 0 num +20 num -10 add wait";
            vm.ReadCode(code);
            vm.Restart();
            vm.Excute();
        }

        [Test]
        public void DoubleTest()
        {
            int count = 1 << 8;
            for (int i = 0; i < count; i++)
            {
                Assert.AreEqual(i, (int)double.Parse(i.ToString()));
            }
        }

        [Test]
        public void LuaTest()
        {
            // var luaLeaf = new LuaLeaf();
            // var path = "Assets/Code/SimpleRPG/Tests/Editor/testLua.lua";
            // var luaCode = System.IO.File.ReadAllText(path);
            // luaLeaf.SetLuaCode(luaCode);
            
            // luaLeaf.ExecuteLuaScript();
            // luaLeaf.ExecuteLuaScript();
            // luaLeaf.ExecuteLuaScript();
            // luaLeaf.ExecuteLuaScript();
            // luaLeaf.ExecuteLuaScript();
            // luaLeaf.ExecuteLuaScript();
        }

        [Test]
        public void LuaTest2()
        {
            // var luaLeaf = new LuaLeaf();
          
            // var luaCode = "require 'battle_attack'";
            // luaLeaf.SetLuaCode(luaCode);
            
            // luaLeaf.ExecuteLuaScript();
        }

        public class K
        {
            public int value; 
        }

        public class KK
        {
            public K k;
            public KK(int v)
            {
                k = new K();
                k.value = v;
            }
        }

        [Test]
        public void RefTest()
        {
            var kk1  = new KK(1);
            var kk2 = new KK(2);
            var kk3 = new KK(3);
            ref K rk = ref kk1.k;
            rk = kk2.k;
            ref int v = ref kk1.k.value;
            int c = v;
            c = 100;
            Debug.Log(kk1.k.value);
        } 

        [Test]
        public void ForTest()
        {
            int rank;
            for (rank = 10; rank >= 1; rank --)
            {
                
            }
            Assert.AreEqual(rank, 0);
        }
        [Test]
        public void MatrixTest()
        {
            var v1 = Vector3.one * 1;
            var v2 = Vector3.one * 2;
            var v3 = Vector3.one * 3;
            var m1 = new Matrix4x4(v1, v2, v3, Vector3.zero);
            Debug.Log(m1);
            var m2 = Matrix4x4.zero;
            m2[0, 0] = v1[0];
            m2[0, 1] = v1[1];
            m2[0, 2] = v1[2];

            m2[1, 0] = v2[0];
            m2[1, 1] = v2[1];
            m2[1, 2] = v2[2];

            m2[2, 0] = v3[0];
            m2[2, 1] = v3[1];
            m2[2, 2] = v3[2];
            Debug.Log(m2);
        }
       
    }
}

using UnityEngine;
using System.IO;
using System;

namespace NullFramework.Runtime
{
    public class FVMLeaf : Leaf<FVMLeafData>
    {
        private Vector3[] vertices;
        private Vector3[] velocity;
        private Vector3[] sum_f;
        private int[] sum_n;
        private Tet[] tet;
        private string elePath;
        private string nodePath;
        private Mesh mesh;
        private Material material;
        //能量密度相关参数
        private float stiffness_0 = 20000.0f;
        private float stiffness_1 = 5000.0f;

        private float damp = 0.999f;
        private Vector3 gravity = 9.8f * Vector3.down;
        private float mass = 1.0f;
        private int num_tet;
        private int num_vert;
        private Vector3 origin;

        protected override void InitListeners()
        {
            base.InitListeners();
            AddMsgListener(BaseMsgKind.RigidUpdate, RigidUpdate);
        }

        public override void OnReciveDataFinish()
        {
            base.OnReciveDataFinish();
            material = leafData.material;
            elePath = leafData.elePath;
            nodePath = leafData.nodePath;
            origin = leafData.position;
            Init();
        }

        
        private System.Action RigidUpdate(Msg msg)
        {
            if(mesh == null) return emptyAction;
            var dt = root.FrameDeltaTime;
            //初始化
            for (int i = 0; i < num_vert; i++)
            {
                //清理
                sum_f[i] = Vector3.zero;
                sum_n[i] = 0;
            }
            
            //更新形变力
            for (int i = 0; i < num_tet; i++)
            {
                var t = tet[i];
                var f = t.GetDeformation(vertices);
                var g = (f.transpose * f).Sub(Matrix4x4.identity).Mulit(0.5f);
                var tr = g.Trace();
                var s = g.Mulit(2 * stiffness_0).Add(Matrix4x4.identity.Mulit(stiffness_1 * tr));
                var p = f * s;
                var force =  (p * t.dm_ti).Mulit(-1.0f / 6.0f / t.det_dm_i);
                var arr_f = new Vector3[4];
                arr_f[1] = new Vector3(force[0, 0], force[1, 0], force[2, 0]);
                arr_f[2] = new Vector3(force[0, 1], force[1, 1], force[2, 1]);
                arr_f[3] = new Vector3(force[0, 2], force[1, 2], force[2, 2]);

                arr_f[0] = -(arr_f[1] + arr_f[2] + arr_f[3]);

                for (int j = 0; j < 4; j++)
                {
                    var id = t.id[j];
                    sum_f[id] += arr_f[j];
                    sum_n[id] ++;

                }
            }
            //更新 v x 处理碰撞
            for (int i = 0; i < num_vert; i++)
            {
                var f = gravity * mass + sum_f[i];
                var a = f / mass;
                velocity[i] = (velocity[i] + a * dt) * damp;
                vertices[i] += velocity[i] * dt;
                //处理碰撞
                PointCollisionPlane(i, dt, Vector3.zero, Vector3.up);
            }
            
            //update mesh
            mesh.vertices = vertices;

            return emptyAction;
        }

        private void PointCollisionPlane(int i, float dt, Vector3 p, Vector3 n)
        {
            var x = vertices[i];
            var dis = Vector3.Dot(x - p, n);
            if(dis >= 0) return;
            var dx = -dis * n;
            vertices[i] += dx;
            velocity[i] += dx / dt;
        }

        public void Init()
        {
            {
                var content = File.ReadAllText(elePath);
                var res = content.Split(new char[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
    
                num_tet = int.Parse(res[0]);
                tet = new Tet[num_tet];
                for (int i = 0; i < num_tet; i++)
                {
                    var t = new Tet();
                    t.id[0] = int.Parse(res[i * 5 + 4]) - 1;
                    t.id[1] = int.Parse(res[i * 5 + 5]) - 1;
                    t.id[2] = int.Parse(res[i * 5 + 6]) - 1;
                    t.id[3] = int.Parse(res[i * 5 + 7]) - 1;
                    tet[i] = t;
                }
            }

            {
                string content = File.ReadAllText(nodePath);
                string[] res = content.Split(new char[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                num_vert = int.Parse(res[0]);
                vertices = new Vector3[num_vert];
                velocity = new Vector3[num_vert];
                sum_f = new Vector3[num_vert];
                sum_n = new int[num_vert];
                float scale = 0.1f;
                for (int i = 0; i < num_vert; i++)
                {
                    vertices[i].x = float.Parse(res[i * 5 + 5]) * scale;
                    vertices[i].y = float.Parse(res[i * 5 + 6]) * scale;
                    vertices[i].z = float.Parse(res[i * 5 + 7]) * scale;
                }
                var center = Vector3.zero;
                Array.ForEach(vertices, p => center += p / num_vert);
                var rotation = Quaternion.Euler(new Vector3(-90, 0, 0));
                for (int i = 0; i < num_vert; i++)
                {
                    vertices[i] = rotation * (vertices[i] - center) + origin;
                }
                Array.ForEach(tet, t => t.Init(vertices));
                var num_index = num_tet * 12;
                var indices = new int[num_index];
                for (int i = 0; i < num_tet; i++)
                {
                    var t = tet[i];
                    var start = i * 12;
                    indices[start++] = t.id[0];
                    indices[start++] = t.id[2];
                    indices[start++] = t.id[1];

                    indices[start++] = t.id[0];
                    indices[start++] = t.id[3];
                    indices[start++] = t.id[2];

                    indices[start++] = t.id[0];
                    indices[start++] = t.id[1];
                    indices[start++] = t.id[3];
                    
                    indices[start++] = t.id[1];
                    indices[start++] = t.id[2];
                    indices[start++] = t.id[3];
                }

                mesh = new Mesh();
                mesh.vertices = vertices;
                mesh.SetIndices(indices, MeshTopology.Triangles, 0);
                mesh.RecalculateNormals();
                
                var go = new GameObject("FVM");
                go.AddComponent<MeshFilter>().sharedMesh = mesh;
                go.AddComponent<MeshRenderer>().sharedMaterial = material;

                
            }
        }    

    }
}


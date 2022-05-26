using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NullFramework.Runtime
{

    public class RigidLeaf : Leaf<RigidLeafData>
    {
        
        //private const float zero = 0.5f;
        private float mass;
        //惯性矩阵
        private Matrix4x4 originInertia;
        private Vector3 position;
        private Vector3 velocity;
        //角速度
        private Vector3 rotateVelocity;
        private Quaternion rotation;
        
        //弹性
        private float elasticity;
        //摩擦相关的系数
        private float u_t;

        //使用mesh当碰撞体
        private Mesh mesh;
        private Transform target;
        

        public override void OnReciveDataFinish()
        {
            base.OnReciveDataFinish();
            mass = leafData.Mass;
            elasticity = leafData.elasticity;
            u_t = leafData.u_t;
        }

        protected override void InitListeners()
        {
            base.InitListeners();
            AddMsgListeners(
                (BaseMsgKind.RigidUpdate, RigidUpdate)
            );
        }

        protected override void OnObjectInstantiate(MsgData_ObjectInstantiate data)
        {
            if(data.obj is GameObject go)
            {
                var meshFliter = go.GetComponentInChildren<MeshFilter>();
                var sharedMesh = meshFliter?.sharedMesh;
                if(sharedMesh != null && sharedMesh != mesh)
                {
                    mesh = sharedMesh;
                    CalculateInertia();
                }
                else
                {
                    mesh = sharedMesh;
                }
                target = go.transform;
                
            }
        }
        
        private System.Action RigidUpdate(Msg msg)
        {
            if(target == null || mesh == null)
            {
                return emptyAction;
            }
            //先同步属性
            position = target.position;
            rotation = target.rotation;

            float deltaTime = root.RealDeltaTime;
            //更新速度 暂时外力只有重力...
            var f = new Vector3(0, -9.8f, 0) * mass;
            velocity += f / mass * deltaTime;

            velocity *= 0.99f;
            rotateVelocity *= 0.98f;

            //处理碰撞 更新速度和角速度
            CollisionPlane(Vector3.zero, Vector3.up);

            //更新坐标
            position += deltaTime * velocity;
            //跟新旋转
            var q = new Quaternion
            (
                rotateVelocity.x * deltaTime / 2, 
                rotateVelocity.y * deltaTime / 2, 
                rotateVelocity.z * deltaTime / 2, 
                0
            ) * rotation;
            rotation = rotation.Add(q).normalized;

            //同步属性
            target.position = position;
            target.rotation = rotation;
            return emptyAction;
        }

        

        //检测与平面发生碰撞的有效碰撞点
        private void CollisionPlane(Vector3 p, Vector3 n)
        {
            var r_total = Vector3.zero;
            //var v_total = Vector3.zero;
            var pointCount = 0;
            var vertexCount = mesh.vertexCount;
            var vertices = mesh.vertices;
            var rotateMatrix = Matrix4x4.Rotate(rotation);
            var inertia = rotateMatrix * originInertia * rotateMatrix.transpose;
            inertia = originInertia;
            for (int i = 0; i < vertexCount; i++)
            {
                var ri_origin = vertices[i];
                var ri_current = (Vector3)(rotateMatrix * ri_origin);
                var v_vert = velocity + Vector3.Cross(rotateVelocity, ri_current);
                var VdotN = Vector3.Dot(v_vert, n);
                //速度和法线同向 碰撞不成立
                if(VdotN >= 0) continue;
                var dis = Vector3.Dot(ri_current + position - p, n);
                //点在平面上方 碰撞不成立
                if(dis >= 0) continue;
                r_total += ri_current;
                //v_total += v_vert;
                pointCount ++;
            }
            if(pointCount == 0) return;
            var r_avg = r_total / pointCount;
            var v_avg = velocity + Vector3.Cross(rotateVelocity, r_avg);
            var v_n = Vector3.Dot(v_avg, n) * n;
            var v_t = v_avg - v_n;
            
            var u_n = elasticity;
            //滑动摩擦系数
            var a = 1 - u_t * (1 + u_n) * v_n.magnitude / Mathf.Max(v_t.magnitude, 0.01f);
            a = Mathf.Max(a, 0);

            //强行减少抽搐
            v_n *= -u_n;
            v_t *= a;          
            var v_current = v_n + v_t;
            var m = GetCrossMatrix(r_avg);
            var k = Matrix4x4.identity.Mulit(1.0f / mass);
            k = k.Sub(m * inertia.inverse * m);
            var j = (Vector3)(k.inverse * (v_current - v_avg));
            //更新速度和角速度
            velocity += j / mass;
            rotateVelocity += (Vector3)(inertia.inverse * (Vector3.Cross(r_avg, j)));
            
        }

        private Matrix4x4 GetCrossMatrix(Vector3 v)
        {
            var m = Matrix4x4.zero;
            m[0, 1] = -v.z;
            m[0, 2] = v.y;
            m[1, 0] = v.z;
            m[1, 2] = -v.x;
            m[2, 0] = -v.y;
            m[2, 1] = v.x;
            m[3, 3] = 1;
            return m;
        }

        //计算初始的惯性矩阵
        private void CalculateInertia()
        {
            originInertia = Matrix4x4.zero;
            var vertexCount = mesh.vertexCount;
            var m = mass / vertexCount;
            var vertices = mesh.vertices;
            for (int i = 0; i < vertexCount; i++)
            {
                var v = vertices[i];
                var d = v.sqrMagnitude;
                originInertia.m00 += m * d;
                originInertia.m11 += m * d;
                originInertia.m22 += m * d;

                originInertia.m00 -= m * v.x * v.x;
                originInertia.m01 -= m * v.x * v.y;
                originInertia.m02 -= m * v.x * v.z;

                originInertia.m10 -= m * v.y * v.x;
                originInertia.m11 -= m * v.y * v.y;
                originInertia.m12 -= m * v.y * v.z;
                
                originInertia.m20 -= m * v.z * v.x;
                originInertia.m21 -= m * v.z * v.y;
                originInertia.m22 -= m * v.z * v.z;
            }
            originInertia[3, 3] = 1;


            
        }
        
        
    }
}


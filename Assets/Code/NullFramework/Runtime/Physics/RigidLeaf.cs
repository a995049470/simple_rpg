using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NullFramework.Runtime
{
    public class RigidLeaf : Leaf
    {
        
        private float mass;
        //惯性矩阵
        private Matrix4x4 inertia;
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
        
        private void RigidUpdate()
        {
            float deltaTime = root.RealDeltaTime;
            //更新速度 暂时外力只有重力...
            var f = new Vector3(0, -9.8f, 0) * mass;
            velocity += f / mass * deltaTime;

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
        }

        //检测与平面发生碰撞的有效碰撞点
        private void CollisionPlane(Vector3 p, Vector3 n)
        {
            var rs = new List<Vector3>();
            var vertexCount = mesh.vertexCount;
            var vertices = mesh.vertices;
            var rotateMatrix = Matrix4x4.Rotate(rotation);
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
                rs.Add(ri_current);
            }
            if(rs.Count == 0)return;
            var r = Vector3.zero;
            var pointsCount = rs.Count;
            rs.ForEach(x=>
            {
                r += x / pointsCount;
            });
            var v_origin = velocity +  Vector3.Cross(rotateVelocity, r); 
            var v_n = Vector3.Dot(v_origin, n) * n;
            var v_t = v_origin - v_n;
            
            var u_n = elasticity;
            //滑动摩擦系数
            var a = 1 - u_t * (1 + u_n) * v_n.magnitude / v_t.magnitude;
            a = Mathf.Max(a, 0);

            v_n *= -u_n;
            v_t *= a;
            
            var v_current = v_n + v_t;
            var m = GetCrossMatrix(r);
            var k = Matrix4x4.identity.Mulit(1.0f / mass).Sub(m * inertia.inverse * m);
            var j = (Vector3)(k.inverse * (v_current - v_origin));

            //更新速度和角速度
            velocity += j / mass;
            rotateVelocity += (Vector3)(inertia.inverse * (Vector3.Cross(r, j)));

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
            return m;
        }

        //计算初始的惯性矩阵
        private void CalculateInertia()
        {
            inertia = Matrix4x4.zero;
            var vertexCount = mesh.vertexCount;
            var m = mass / vertexCount;
            var vertices = mesh.vertices;
            for (int i = 0; i < vertexCount; i++)
            {
                var v = vertices[i];
                var d = v.sqrMagnitude;
                inertia.m00 += m * d;
                inertia.m11 += m * d;
                inertia.m22 += m * d;

                inertia.m00 -= m * v.x * v.x;
                inertia.m01 -= m * v.x * v.y;
                inertia.m02 -= m * v.x * v.z;

                inertia.m10 -= m * v.y * v.x;
                inertia.m11 -= m * v.y * v.y;
                inertia.m12 -= m * v.y * v.z;
                
                inertia.m20 -= m * v.z * v.x;
                inertia.m21 -= m * v.z * v.y;
                inertia.m22 -= m * v.z * v.z;
            }
        }
        
        
    }
}


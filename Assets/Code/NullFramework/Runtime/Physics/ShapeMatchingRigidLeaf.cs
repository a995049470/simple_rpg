using UnityEngine;
using System;

namespace NullFramework.Runtime
{
    public class ShapeMatchingRigidLeaf : Leaf<ShapeMatchingRigidLeafData>
    {
        private Vector3[] positionWSArray;
        private Vector3[] positionOSArray;
        private Vector3[] velocityArray;
        private Matrix4x4 qqt;
        private Transform target;
        private Mesh mesh;
        private Vector3 centerOffset = Vector3.zero;
        
        //弹性
        private float elasticity = 0.0f;
        //摩擦相关的系数
        private float u_t = 0.2f;

        public override void OnReciveDataFinish()
        {
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
            if(data.obj is GameObject go && target == null)
            {
                var meshFliter = go.GetComponentInChildren<MeshFilter>();
                var targetMesh = meshFliter.mesh;
                mesh = targetMesh;
                target = meshFliter.transform;
                Init();
            }
        }

        private System.Action RigidUpdate(Msg msg)
        {
            var dt = 0.015f;
            if(target == null || mesh == null) 
            {
                return emptyAction;
            }
            centerOffset = Vector3.zero;
            //首先更新位置
            var count = positionWSArray.Length;
            var a = 10 * Vector3.down;
            for (int i = 0; i < count; i++)
            {
                var posWS = positionWSArray[i];
                var velocity = velocityArray[i];
                velocity += a * dt;
                velocityArray[i] = velocity * 0.99f;
                posWS += velocity * dt; 
                positionWSArray[i] = posWS;
            }
            //处理碰撞
            CollisionPlane(Vector3.zero, Vector3.up, dt);

            // //重新约束顶点
            ConstraintVertices(1.0f / dt);

            return emptyAction;

        }

        private void ConstraintVertices(float inv_dt)
        {
        
            var center = Vector3.zero;
            var count = positionWSArray.Length;
            Array.ForEach(positionWSArray, x=>
            {
                center += x;
            });
            center /= count;
            var A = Matrix4x4.zero;
            var step = 128;
            for (int i = 0; i < count; i += step)
            {
                var v1 = positionWSArray[i] - center;
                var v2 = positionOSArray[i];

                A[0, 0] += v1[0] * v2[0] * step;
                A[0, 1] += v1[0] * v2[1] * step;
                A[0, 2] += v1[0] * v2[2] * step;

                A[1, 0] += v1[1] * v2[0] * step;
                A[1, 1] += v1[1] * v2[1] * step;
                A[1, 2] += v1[1] * v2[2] * step;

                A[2, 0] += v1[2] * v2[0] * step;
                A[2, 1] += v1[2] * v2[1] * step;
                A[2, 2] += v1[2] * v2[2] * step;
            }
            A[3, 3] = 1;
            A = A * qqt.inverse;
            var r = Get_Rotation(A);
            center += centerOffset;
            Update_Mesh(center, r, inv_dt);
        }

        private Matrix4x4 VectorMulitVectorTranspose(Vector3 v1, Vector3 v2)
        {
            var m = Matrix4x4.identity;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    m[i, j] = v1[i] * v2[j];
                }
            }
            return m;
        }
               
        private void CollisionPlane(Vector3 p, Vector3 n, float dt)
        {
            var count = positionWSArray.Length;
            
            for (int i = 0; i < count; i++)
            {
                var point = positionWSArray[i];
                point += centerOffset;
                var dis = Vector3.Dot(point - p, n);
                if(dis >= 0) continue;
                var v = velocityArray[i];
                var VdotN = Vector3.Dot(v, n);
                centerOffset += -dis * n;
                if(VdotN >= 0) continue;
                var v_n =  Vector3.Dot(v, n) * n;
                var v_t = v - v_n;
                var u_n = elasticity;
                var a = 1 - u_t * (1 + u_n) * v_n.magnitude / Mathf.Max(v_t.magnitude, 0.01f);
                a = Mathf.Max(a, 0);
                v_n *= -u_n;
                v_t *= a;
                var v_current = v_n + v_t;
                velocityArray[i] = v_current;
                
               
                // var move = dis * n;
                // point += move;
                // positionWSArray[i] = point;
            }
        }

        private void Update_Mesh(Vector3 c, Matrix4x4 r, float inv_dt)
        {
            for (int i = 0; i < positionOSArray.Length; i++)
            {
                Vector3 posWS = (Vector3)(r * positionOSArray[i]) + c;
                velocityArray[i] += (posWS - positionWSArray[i]) * inv_dt;
                positionWSArray[i] = posWS;
            }
            mesh.vertices = positionWSArray;
            //可能需要重新设置AABB
            //mesh.bounds = new Bounds(c, Vector3.one * 10);
        }

        private void Init()
        {
            var verticesCount = mesh.vertexCount;
            positionOSArray = mesh.vertices;
            positionWSArray = mesh.vertices;
            velocityArray = new Vector3[verticesCount];
            var center = Vector3.zero;
            Array.ForEach(positionOSArray, x=>
            {
                center += x / verticesCount;
            });
            qqt = Matrix4x4.zero;
            for (int i = 0; i < verticesCount; i++)
            {
                var posOS = positionOSArray[i] - center;
                positionOSArray[i] = posOS;
                qqt[0, 0] += posOS[0] * posOS[0];
                qqt[0, 1] += posOS[0] * posOS[1];
                qqt[0, 2] += posOS[0] * posOS[2];

                qqt[1, 0] += posOS[1] * posOS[0];
                qqt[1, 1] += posOS[1] * posOS[1];
                qqt[1, 2] += posOS[1] * posOS[2];

                qqt[2, 0] += posOS[2] * posOS[0];
                qqt[2, 1] += posOS[2] * posOS[1];
                qqt[2, 2] += posOS[2] * posOS[2];
            }
            qqt[3, 3] = 1;
            var currentCenter = target.position;
            var rotateMatrix = Matrix4x4.Rotate(target.rotation);
            target.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            for (int i = 0; i < verticesCount; i++)
            {
                var posOS = positionOSArray[i];
                positionWSArray[i] = currentCenter + (Vector3)(rotateMatrix * posOS);
            }
            Update_Mesh(currentCenter, rotateMatrix, 0);
        }

        Matrix4x4 Get_Rotation(Matrix4x4 F)
        {
            Matrix4x4 C = Matrix4x4.zero;
            for (int ii = 0; ii < 3; ii++)
                for (int jj = 0; jj < 3; jj++)
                    for (int kk = 0; kk < 3; kk++)
                        C[ii, jj] += F[kk, ii] * F[kk, jj];

            Matrix4x4 C2 = Matrix4x4.zero;
            for (int ii = 0; ii < 3; ii++)
                for (int jj = 0; jj < 3; jj++)
                    for (int kk = 0; kk < 3; kk++)
                        C2[ii, jj] += C[ii, kk] * C[jj, kk];

            float det = F[0, 0] * F[1, 1] * F[2, 2] +
                            F[0, 1] * F[1, 2] * F[2, 0] +
                            F[1, 0] * F[2, 1] * F[0, 2] -
                            F[0, 2] * F[1, 1] * F[2, 0] -
                            F[0, 1] * F[1, 0] * F[2, 2] -
                            F[0, 0] * F[1, 2] * F[2, 1];

            float I_c = C[0, 0] + C[1, 1] + C[2, 2];
            float I_c2 = I_c * I_c;
            float II_c = 0.5f * (I_c2 - C2[0, 0] - C2[1, 1] - C2[2, 2]);
            float III_c = det * det;
            float k = I_c2 - 3 * II_c;

            Matrix4x4 inv_U = Matrix4x4.zero;
            if (k < 1e-10f)
            {
                float inv_lambda = 1 / Mathf.Sqrt(I_c / 3);
                inv_U[0, 0] = inv_lambda;
                inv_U[1, 1] = inv_lambda;
                inv_U[2, 2] = inv_lambda;
            }
            else
            {
                float l = I_c * (I_c * I_c - 4.5f * II_c) + 13.5f * III_c;
                float k_root = Mathf.Sqrt(k);
                float value = l / (k * k_root);
                if (value < -1.0f) value = -1.0f;
                if (value > 1.0f) value = 1.0f;
                float phi = Mathf.Acos(value);
                float lambda2 = (I_c + 2 * k_root * Mathf.Cos(phi / 3)) / 3.0f;
                float lambda = Mathf.Sqrt(lambda2);

                float III_u = Mathf.Sqrt(III_c);
                if (det < 0) III_u = -III_u;
                float I_u = lambda + Mathf.Sqrt(-lambda2 + I_c + 2 * III_u / lambda);
                float II_u = (I_u * I_u - I_c) * 0.5f;


                float inv_rate, factor;
                inv_rate = 1 / (I_u * II_u - III_u);
                factor = I_u * III_u * inv_rate;

                Matrix4x4 U = Matrix4x4.zero;
                U[0, 0] = factor;
                U[1, 1] = factor;
                U[2, 2] = factor;

                factor = (I_u * I_u - II_u) * inv_rate;
                for (int i = 0; i < 3; i++)
                    for (int j = 0; j < 3; j++)
                        U[i, j] += factor * C[i, j] - inv_rate * C2[i, j];

                inv_rate = 1 / III_u;
                factor = II_u * inv_rate;
                inv_U[0, 0] = factor;
                inv_U[1, 1] = factor;
                inv_U[2, 2] = factor;

                factor = -I_u * inv_rate;
                for (int i = 0; i < 3; i++)
                    for (int j = 0; j < 3; j++)
                        inv_U[i, j] += factor * U[i, j] + inv_rate * C[i, j];
            }

            Matrix4x4 R = Matrix4x4.zero;
            for (int ii = 0; ii < 3; ii++)
                for (int jj = 0; jj < 3; jj++)
                    for (int kk = 0; kk < 3; kk++)
                        R[ii, jj] += F[ii, kk] * inv_U[kk, jj];
            R[3, 3] = 1;
            return R;
        }

    }
}


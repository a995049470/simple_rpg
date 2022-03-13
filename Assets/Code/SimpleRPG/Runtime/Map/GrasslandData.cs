using UnityEngine;

namespace NullFramework.Editor
{
    //草丛的范围数据
    [System.Serializable]
    public class GrasslandData
    {
        private Rect grassRect;
        private Texture2D grassMask;
        [SerializeField]
        private Texture2D grassColorTex;
        [SerializeField]
        private Mesh grassMesh;
        [SerializeField]
        private float xmin;
        [SerializeField]
        private float xmax;
        [SerializeField]
        private float zmin;
        [SerializeField]
        private float zmax;
        [SerializeField]
        private int width;
        [SerializeField]
        private int height;
        [SerializeField]
        private ComputeShader grassComputeShader;
        [SerializeField]
        //所有草的数据
        private ComputeBuffer grassBuffer;
        private bool isDirtyData = true;
        

        private void Init()
        {
            grassRect = new Rect(xmin, zmin, xmax - xmin, zmax - zmin);
            if (grassMask != null) Object.DestroyImmediate(grassMask);
            grassMask = new Texture2D(width, height);
            if(grassBuffer != null) grassBuffer.Dispose();
            grassBuffer = new ComputeBuffer(width * height, GrassData.SIZE);
        }

        //画圆形区域
        private void Draw(Vector3 center, float radius)
        {  
            isDirtyData = true;
        }

    }

}


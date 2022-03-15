using System.Collections;
using System.Collections.Generic;
using NullFramework.Editor;
using UnityEngine;

namespace LPipeline.Editor
{
    [ExecuteAlways]
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class TestMesh : MonoBehaviour
    {
        private Mesh CreateCube()
        {
            var mesh = new Mesh();
            var vertices = new Vector3[]
            {
                //   5
                // 1 2 3 4
                //   6 
                new Vector3(-1, 1, -1),
                new Vector3(-1, 1, 1),
                new Vector3(-1, -1, 1),
                new Vector3(-1, -1, -1),

                new Vector3(-1, 1, 1),
                new Vector3(1, 1, 1),
                new Vector3(1, -1, 1),
                new Vector3(-1, -1, 1),

                new Vector3(1, 1, 1),
                new Vector3(1, 1, -1),
                new Vector3(1, -1, -1),
                new Vector3(1, -1, 1),

                new Vector3(1, 1, -1),
                new Vector3(-1, 1, -1),
                new Vector3(-1, -1, -1),
                new Vector3(1, -1, -1),
                
                new Vector3(-1, 1, -1),
                new Vector3(1, 1, -1),
                new Vector3(1, 1, 1),
                new Vector3(-1, 1, 1),

                new Vector3(-1, -1, 1),
                new Vector3(1, -1, 1),
                new Vector3(1, -1, -1),
                new Vector3(-1, -1, -1),
            };
            var uv = new Vector2[]
            {
                new Vector2(0.00f, 0.66f),
                new Vector2(0.25f, 0.66f),
                new Vector2(0.25f, 0.33f),
                new Vector2(0.00f, 0.33f),

                new Vector2(0.25f, 0.66f),
                new Vector2(0.50f, 0.66f),
                new Vector2(0.50f, 0.33f),
                new Vector2(0.25f, 0.33f),

                new Vector2(0.50f, 0.66f),
                new Vector2(0.75f, 0.66f),
                new Vector2(0.75f, 0.33f),
                new Vector2(0.50f, 0.33f),

                new Vector2(0.75f, 0.66f),
                new Vector2(1.00f, 0.66f),
                new Vector2(1.00f, 0.33f),
                new Vector2(0.75f, 0.33f),

                new Vector2(0.25f, 0.99f),
                new Vector2(0.50f, 0.99f),
                new Vector2(0.50f, 0.66f),
                new Vector2(0.25f, 0.66f),

                new Vector2(0.25f, 0.33f),
                new Vector2(0.50f, 0.33f),
                new Vector2(0.50f, 0.00f),
                new Vector2(0.25f, 0.00f),
            };
            var indices = new int[]
            {
                0, 2, 1,
                0, 3, 2,

                4, 6, 5,
                4, 7, 6,

                8, 10, 9,
                8, 11, 10,

                12, 14, 13,
                12, 15, 14,

                16, 18, 17,
                16, 19, 18,

                20, 22, 21,
                20, 23, 22,
            };
            mesh.vertices = vertices;
            mesh.uv = uv;
            mesh.SetIndices(indices, MeshTopology.Triangles, 0);
            return mesh;
        }

        private void OnEnable() {
            this.GetComponent<MeshFilter>().sharedMesh = CreateCube();
        }
    }

}

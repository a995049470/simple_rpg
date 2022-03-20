using UnityEngine;

namespace LPipeline.Editor
{
    public static class MeshUitily
    {
        private static Mesh cube;
        public static Mesh CreateCube()
        {
            if(cube != null) return cube;
            cube = new Mesh();
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
                new Vector2(0.00f, 0.6666f),
                new Vector2(0.25f, 0.6666f),
                new Vector2(0.25f, 0.3333f),
                new Vector2(0.00f, 0.3333f),

                new Vector2(0.25f, 0.6666f),
                new Vector2(0.50f, 0.6666f),
                new Vector2(0.50f, 0.3333f),
                new Vector2(0.25f, 0.3333f),

                new Vector2(0.50f, 0.6666f),
                new Vector2(0.75f, 0.6666f),
                new Vector2(0.75f, 0.3333f),
                new Vector2(0.50f, 0.3333f),

                new Vector2(0.75f, 0.6666f),
                new Vector2(1.00f, 0.6666f),
                new Vector2(1.00f, 0.3333f),
                new Vector2(0.75f, 0.3333f),

                new Vector2(0.25f, 1.0000f),
                new Vector2(0.50f, 1.0000f),
                new Vector2(0.50f, 0.6666f),
                new Vector2(0.25f, 0.6666f),

                new Vector2(0.25f, 0.3333f),
                new Vector2(0.50f, 0.3333f),
                new Vector2(0.50f, 0.0000f),
                new Vector2(0.25f, 0.0000f),
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
            cube.vertices = vertices;
            cube.uv = uv;
            cube.SetIndices(indices, MeshTopology.Triangles, 0);
            return cube;
        }
    }
    

}

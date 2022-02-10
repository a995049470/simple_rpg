using UnityEngine;

namespace SimpleRPG.Editor
{
    public class GBufferTextures
    {
        public string name;
        public Texture2D albedoTexture;
        public Texture2D normalTexture;
        public Texture2D metallicTexture;
        public Texture2D roughnessTexture;
        public Texture2D aoTexture;

        
        public Color GetAlbedo(int i, int j)
        {
            var pixel = albedoTexture == null ? Color.white : albedoTexture.GetPixel(i, j);
            return pixel;
        }

        public Vector3 GetNormalTS(int i, int j)
        {
            var pixel = normalTexture == null ? new Color(0.5f, 0.5f, 1.0f) : normalTexture.GetPixel(i, j);
            return new Vector3(pixel.r, pixel.g, pixel.b);
        }

        public float GetMetallic(int i, int j)
        {
            return metallicTexture == null ? 0.5f : metallicTexture.GetPixel(i, j).r;
        }

        public float GetRoughness(int i, int j)
        {
            return roughnessTexture == null ? 0.5f : roughnessTexture.GetPixel(i, j).r;
        }

        public float GetAO(int i, int j)
        {
            return aoTexture == null ? 1.0f : aoTexture.GetPixel(i, j).r;
        }
    }
}


using System.Numerics;
using _3DGame.Core.Graphics;

namespace _3DGame.Core;

public class Material
{
    public Vector3 DiffuseColor { get; set; }
    public Vector3 SpecularColor { get; set; }
    public Vector3 AmbientColor { get; set; }

    public float Shininess { get; set; }

    public Dictionary<TextureType, List<Texture>> textures;

    public Material()
    {
        textures = new Dictionary<TextureType, List<Texture>>();
    }

    public void Draw(Renderer renderer)
    {
        renderer.Shader.Use();
    }
}

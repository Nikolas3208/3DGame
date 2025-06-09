using _3DGame.Core.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace _3DGame.Core;

public class Material
{
    public Vector3 DiffuseColor { get; set; }
    public Vector3 SpecularColor { get; set; }
    public Vector3 AmbientColor { get; set; }
    public Vector3 Color { get; set; } = Vector3.One;

    public Vector3 TextureScale { get; set; } = new Vector3(10f);

    public float Shininess { get; set; }

    public Dictionary<TextureType, Texture> Textures;

    public string Name { get; } = nameof(Material);

    public Material(string name)
    {
        Textures = new Dictionary<TextureType, Texture>();
        Name = name;
    }

    public Material(Material material)
    {
        DiffuseColor = material.DiffuseColor;
        SpecularColor = material.SpecularColor;
        AmbientColor = material.AmbientColor;
        Shininess = material.Shininess;
        Textures = material.Textures;

        Name = material.Name;
    }

    public Material(Vector3 diffuseColor, Vector3 specularColor, Vector3 ambientColor, float shininess, string name)
    {
        DiffuseColor = diffuseColor;
        SpecularColor = specularColor;
        AmbientColor = ambientColor;
        Shininess = shininess;
        Name = name;

        Textures = new Dictionary<TextureType, Texture>();
    }

    public void Draw(Renderer renderer)
    {
        int textureUnit = 0;

        renderer.Shader.Use();

        renderer.Shader.SetVector3("material.diffuseColor", DiffuseColor);
        renderer.Shader.SetVector3("material.specularColor", SpecularColor);
        renderer.Shader.SetVector3("material.ambientColor", AmbientColor);
        renderer.Shader.SetVector3("material.color", Color);
        renderer.Shader.SetVector3("material.textureScale", TextureScale);

        renderer.Shader.SetFloat("material.shininess", Shininess);

        foreach (var texture in Textures)
        {
            var textureValue = texture.Value;

            renderer.Shader.SetInt($"material.{texture.Key}", textureUnit);
            renderer.Shader.SetInt($"material.use{texture.Key}Map", 1);

            textureValue.Use(TextureUnit.Texture0 + textureUnit);

            textureUnit++;
        }
    }

    public void UnbindTexture(Renderer renderer)
    {
        for (int i = 0; i < Textures.Count; i++)
        {
            GL.ActiveTexture(TextureUnit.Texture0 + i);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            renderer.Shader.SetInt($"material.use{Textures.Keys.ToArray()[i]}Map", 0);
        }
    }

    public static Material Default => new Material(new Vector3(0.714f, 0.4284f, 0.18144f), new Vector3(0.393548f, 0.271906f, 0.166721f), new Vector3(0.2125f, 0.1275f, 0.054f), 25.6f, "Bronze");
}

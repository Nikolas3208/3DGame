using _3DGame.Core.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace _3DGame.Core;

public class Material
{
    public Vector3 Diffuse { get; set; }
    public Vector3 Specular { get; set; }
    public Vector3 Ambient { get; set; }
    public Vector3 Color { get; set; } = Vector3.One;

    public Vector3 TextureScale { get; set; } = new Vector3(1f);
    public Vector3 TextureOffset { get; set; } = Vector3.Zero;

    public float NormalStrength { get; set; } = 1.0f;
    public float HeightScale { get; set; } = 0.1f;
    public float Shininess { get; set; }

    public Dictionary<TextureType, Texture> Textures;

    public string Name { get; } = nameof(Material);

    public int useTBN = 1;

    public Material(string name)
    {
        Textures = new Dictionary<TextureType, Texture>();
        Name = name;
    }

    public Material(Material material)
    {
        Diffuse = material.Diffuse;
        Specular = material.Specular;
        Ambient = material.Ambient;
        Shininess = material.Shininess;
        Textures = material.Textures;

        Name = material.Name;
    }

    public Material(Vector3 diffuseColor, Vector3 specularColor, Vector3 ambientColor, float shininess, string name)
    {
        Diffuse = diffuseColor;
        Specular = specularColor;
        Ambient = ambientColor;
        Shininess = shininess;
        Name = name;

        Textures = new Dictionary<TextureType, Texture>();
    }

    public void AddTexture(TextureType type, Texture texture)
    {
        if (Textures.ContainsKey(type))
        {
            Textures[type] = texture;
        }
        else
        {
            Textures.Add(type, texture);
        }
    }

    public void RemoveTexture(TextureType type)
    {
        if (Textures.ContainsKey(type))
        {
            Textures.Remove(type);
        }
    }

    public void Draw(Renderer renderer)
    {
        int textureUnit = 0;

        renderer.Shader.Use();

        renderer.Shader.SetVector3("material.diffuseColor", Diffuse);
        renderer.Shader.SetVector3("material.specularColor", Specular);
        renderer.Shader.SetVector3("material.ambientColor", Ambient);
        renderer.Shader.SetVector3("material.color", Color);
        renderer.Shader.SetVector3("material.textureScale", TextureScale);
        renderer.Shader.SetVector3("material.textureOffset", TextureOffset);
        renderer.Shader.SetFloat("material.normalStrength", NormalStrength);
        renderer.Shader.SetFloat("material.heightScale", HeightScale);
        renderer.Shader.SetInt("material.useTBN", useTBN);

        renderer.Shader.SetFloat("material.shininess", Shininess);

        foreach (var texture in Textures)
        {
            var textureValue = texture.Value;

            renderer.Shader.SetInt($"material.use{texture.Key}Map", 1);
            renderer.Shader.SetInt($"material.{texture.Key}", textureUnit);

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

    public static Material BaseMaterial => new Material(new Vector3(1f), new Vector3(0.0f), new Vector3(0.5f), 10f, "Base Material");
}

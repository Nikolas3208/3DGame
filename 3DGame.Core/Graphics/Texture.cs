using System.Runtime.CompilerServices;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using StbImageSharp;

namespace _3DGame.Core;

public enum TextureType
{
    Deffuse,
    Normal,
    Specular,
    CubeMap
}

public class Texture
{
    private bool smooth;

    public readonly int Handle;

    public bool Smooth { get => smooth;  set { smooth = value; UpdateSmooth(); } }

    public Vector2 Size { get; }

    private Texture(int handle, Vector2 size, bool smooth)
    {
        Handle = handle;
        Size = size;
        this.smooth = smooth;
    }

    private void UpdateSmooth()
    {
        Bind();
        if (smooth)
        {
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
        }
        else
        {
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
        }
        Unbind();
    }

    public void Use(TextureUnit textureUnit)
    {
        GL.ActiveTexture(textureUnit);
        GL.BindTexture(TextureTarget.Texture2D, Handle);
    }

    public void Bind()
    {
        GL.BindTexture(TextureTarget.Texture2D, Handle);
    }

    public void Unbind()
    {
        GL.BindTexture(TextureTarget.Texture2D, 0);
    }

    public static Texture LoadFromFile(string path, bool smooth = false)
    {
        Vector2 size = new Vector2(0);

        int handle = GL.GenTexture();

        GL.ActiveTexture(TextureUnit.Texture0);
        GL.BindTexture(TextureTarget.Texture2D, handle);

        StbImage.stbi_set_flip_vertically_on_load(1);

        using (Stream stream = File.OpenRead(path))
        {
            ImageResult image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);
            size = new Vector2(image.Width, image.Height);
        }

        if (smooth)
        {
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
        }
        else
        {
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
        }

        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
        GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

        return new Texture(handle, size, smooth);
    }
}

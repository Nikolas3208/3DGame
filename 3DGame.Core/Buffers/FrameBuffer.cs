using _3DGame.Core.Ecs.Components;
using OpenTK.Graphics.OpenGL4;
using static System.Net.Mime.MediaTypeNames;

namespace _3DGame.Core.Buffers
{
    public class FrameBuffer
    {
        public int Handle { get; private set; }
        public int TextureHandle { get; private set; }

        public int Width { get; private set; }
        public int Height { get; private set; }

        public FrameBuffer(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public void Init()
        {
            Handle = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, Handle);
            GL.CreateTextures(TextureTarget.Texture2D, 1, out int mTexId);
            GL.BindTexture(TextureTarget.Texture2D, mTexId);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, Width, Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Linear);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapR, (uint)All.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (uint)All.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (uint)All.ClampToEdge);

            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, mTexId, 0);

            GL.CreateTextures(TextureTarget.Texture2D, 1, out int mDepthId);
            GL.BindTexture(TextureTarget.Texture2D, mDepthId);
            GL.TexStorage2D(TextureTarget2d.Texture2D, 1, SizedInternalFormat.Depth24Stencil8, Width, Height);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, Width, Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Linear);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapR, (uint)All.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (uint)All.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (uint)All.ClampToEdge);

            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment, TextureTarget.Texture2D, mDepthId, 0);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            TextureHandle = mTexId;
        }

        public void Bind()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, Handle);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        }

        public void Bind(int width, int height)
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, Handle);
            GL.Viewport(0, 0, width, height);
        }

        public void Unbind()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }
    }
}

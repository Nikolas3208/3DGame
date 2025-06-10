using _3DGame.Core.Graphics;
using OpenTK.Graphics.OpenGL4;
using System.Runtime.InteropServices;

namespace _3DGame.Core.Buffers
{
    public class VertexBuffer
    {
        private Vertex[] vertices;

        public int Handle { get; private set; }

        public VertexBuffer(Vertex[] vertices)
        {
            this.vertices = vertices;
        }

        public void Init()
        {
            Handle = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, Handle);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * Marshal.SizeOf(typeof(Vertex)), vertices, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        public void Bind()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, Handle);
        }

        public void Unbind()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        public int GetCount() => vertices.Length;
    }
}

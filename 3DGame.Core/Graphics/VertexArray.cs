using _3DGame.Core.Buffers;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.Runtime.InteropServices;

namespace _3DGame.Core.Graphics
{
    public class VertexArray
    {
        private VertexBuffer vertexBuffer;
        private IndexBuffer? indexBuffer;

        private int vertexLocation;
        private int normalLocation;
        private int texCoordsLocation;
        private int colorLocation;

        public int Handle;

        public VertexArray(VertexBuffer vertexBuffer)
        {
            this.vertexBuffer = vertexBuffer;
        }

        public VertexArray(VertexBuffer vertexBuffer, IndexBuffer indexBuffer)
        {
            this.vertexBuffer = vertexBuffer;
            this.indexBuffer = indexBuffer;
        }

        public void Init()
        {
            int size = Marshal.SizeOf(typeof(Vertex));
            int v3Size = Marshal.SizeOf(typeof(Vector3));
            int v2Size = Marshal.SizeOf(typeof(Vector2));
            int color4Size = Marshal.SizeOf(typeof(Color4));

            Handle = GL.GenVertexArray();

            Bind();

            GL.EnableVertexAttribArray(vertexLocation);
            GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, size, 0);

            GL.EnableVertexAttribArray(normalLocation);
            GL.VertexAttribPointer(normalLocation, 3, VertexAttribPointerType.Float, false, size, v3Size);

            GL.EnableVertexAttribArray(texCoordsLocation);
            GL.VertexAttribPointer(texCoordsLocation, 2, VertexAttribPointerType.Float, false, size, 3 * v2Size);

            GL.EnableVertexAttribArray(colorLocation);
            GL.VertexAttribPointer(colorLocation, 4, VertexAttribPointerType.Float, false, size, 4 * color4Size);

            Unbind();

            GL.DisableVertexAttribArray(vertexLocation);
            GL.DisableVertexAttribArray(normalLocation);
            GL.DisableVertexAttribArray(texCoordsLocation);
            GL.DisableVertexAttribArray(colorLocation);

        }

        public void Bind()
        {
            GL.BindVertexArray(Handle);
            vertexBuffer.Bind();
            indexBuffer?.Bind();
        }

        public void Unbind()
        {
            indexBuffer?.Unbind();
            vertexBuffer.Unbind();
            GL.BindVertexArray(0);
        }
    }
}

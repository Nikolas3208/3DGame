using _3DGame.Core.Buffers;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.Runtime.InteropServices;

namespace _3DGame.Core.Graphics
{
    public enum VertexArrayDrawType
    {
        ArrayDraw,
        DrawElements
    }

    public class VertexArray
    {
        private VertexBuffer vertexBuffer;
        private IndexBuffer? indexBuffer;

        private int vertexLocation = 0;
        private int normalLocation = 1;
        private int texCoordsLocation = 2;
        private int colorLocation = 3;

        public int Handle;

        public VertexArrayDrawType DrawType { get; set; }

        public VertexArray(VertexBuffer vertexBuffer)
        {
            this.vertexBuffer = vertexBuffer;

            DrawType = VertexArrayDrawType.ArrayDraw;
        }

        public VertexArray(VertexBuffer vertexBuffer, IndexBuffer indexBuffer)
        {
            this.vertexBuffer = vertexBuffer;
            this.indexBuffer = indexBuffer;

            DrawType = VertexArrayDrawType.DrawElements;
        }

        public void Init()
        {
            int size = Marshal.SizeOf(typeof(Vertex));

            Handle = GL.GenVertexArray();

            Bind();

            GL.EnableVertexAttribArray(vertexLocation);
            GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, size, 0);

            GL.EnableVertexAttribArray(normalLocation);
            GL.VertexAttribPointer(normalLocation, 3, VertexAttribPointerType.Float, false, size, 12);

            GL.EnableVertexAttribArray(texCoordsLocation);
            GL.VertexAttribPointer(texCoordsLocation, 2, VertexAttribPointerType.Float, false, size, 24);

            GL.EnableVertexAttribArray(colorLocation);
            GL.VertexAttribPointer(colorLocation, 4, VertexAttribPointerType.Float, false, size, 32);

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

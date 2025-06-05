using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3DGame.Core.Buffers
{
    public class IndexBuffer
    {
        private uint[] indices;

        public int Hanlde { get; private set; }

        public IndexBuffer(uint[] indices)
        {
            this.indices = indices;
        }

        public void Init()
        {
            Hanlde = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, Hanlde);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }

        public void Bind()
        {
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, Hanlde);
        }

        public void Unbind()
        {
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }

        public int GetCount() => indices.Length;
    }
}

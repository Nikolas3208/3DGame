using _3DGame.Core.Buffers;
using _3DGame.Core.Graphics;
using OpenTK.Graphics.ES11;

namespace _3DGame.Core;

public class Mesh
{
    private VertexArray vertexArray;

    private int vertexCount;
    private int indicesCount;

    public string Name { get; set; } = "Mesh";

    public Mesh(Vertex[] vertices)
    {
        vertexCount = vertices.Length;

        var vertexBuffer = new VertexBuffer(vertices);
        vertexBuffer.Init();

        vertexArray = new VertexArray(vertexBuffer);
        vertexArray.Init();
    }

    public Mesh(Vertex[] vertices, uint[] indices)
    {
        vertexCount = vertices.Length;
        indicesCount = indices.Length;

        var vertexBuffer = new VertexBuffer(vertices);
        vertexBuffer.Init();

        var indexBuffer = new IndexBuffer(indices);
        indexBuffer.Init();

        vertexArray = new VertexArray(vertexBuffer, indexBuffer);
        vertexArray.Init();
    }

    public void Draw(Renderer renderer)
    {
        renderer.Shader.Use();

        vertexArray.Bind();

        switch (vertexArray.DrawType)
        {
            case VertexArrayDrawType.ArrayDraw:
                GL.DrawArrays(PrimitiveType.Triangles, 0, vertexCount);
                break;
            case VertexArrayDrawType.ElementDraw:
                GL.DrawElements(PrimitiveType.Triangles, indicesCount, DrawElementsType.UnsignedInt, 0);
                break;
        }

        vertexArray.Unbind();
    }
}

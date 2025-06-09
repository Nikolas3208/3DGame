using _3DGame.Core.Buffers;
using _3DGame.Core.Graphics;
using OpenTK.Graphics.ES11;

namespace _3DGame.Core;

public class Mesh
{
    private VertexArray? vertexArray;

    private Vertex[] vertices;
    private uint[] indices;

    private int vertexCount;
    private int indicesCount;

    public string Name { get; set; } = "Mesh";

    public Material Material { get; set; }

    public Mesh()
    {
        Material = Material.Default;
    }

    public Mesh(Vertex[] vertices)
    {
        this.vertices = vertices;
        vertexCount = vertices.Length;

        var vertexBuffer = new VertexBuffer(vertices);
        vertexBuffer.Init();

        vertexArray = new VertexArray(vertexBuffer);
        vertexArray.Init();

        Material = Material.Default;
    }

    public Mesh(Vertex[] vertices, uint[] indices)
    {
        this.vertices = vertices;
        this.indices = indices;

        vertexCount = vertices.Length;
        indicesCount = indices.Length;

        var vertexBuffer = new VertexBuffer(vertices);
        vertexBuffer.Init();

        var indexBuffer = new IndexBuffer(indices);
        indexBuffer.Init();

        vertexArray = new VertexArray(vertexBuffer, indexBuffer);
        vertexArray.Init();

        Material = Material.Default;
    }

    public void Draw(Renderer renderer)
    {
        if (vertexArray == null)
            return;


        renderer.Shader.Use();
        Material.Draw(renderer);

        vertexArray.Bind();

        switch (vertexArray.DrawType)
        {
            case VertexArrayDrawType.ArrayDraw:
                GL.DrawArrays(PrimitiveType.Triangles, 0, vertexCount);
                break;
            case VertexArrayDrawType.DrawElements:
                GL.DrawElements(PrimitiveType.Triangles, indicesCount, DrawElementsType.UnsignedInt, 0);
                break;
        }

        vertexArray.Unbind();
        Material.UnbindTexture(renderer);
    }

    public int GetVerticesCount() => vertexCount;

    public Vertex[] GetVertices() => vertices;

    public int GetIndicesCount() => indicesCount;

    public uint[] GetIndices() => indices;
}

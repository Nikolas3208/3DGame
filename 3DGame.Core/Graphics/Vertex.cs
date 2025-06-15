using OpenTK.Mathematics;

namespace _3DGame.Core.Graphics
{
    public struct Vertex
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector2 TexCoords;
        public Color4 Color;
        public Vector3 Tangent;
        public Vector3 Bitangent;


        public Vertex(Vector3 position, Vector2 texCoords, Vector3 normal)
        {
            Position = position;
            TexCoords = texCoords;
            Normal = normal;
        }

        public Vertex(Vector3 position, Vector2 texCoords, Vector3 normal, Vector3 tangent, Vector3 bitngent)
        {
            Position = position;
            TexCoords = texCoords;
            Normal = normal;
            Tangent = tangent;
            Bitangent = bitngent;
        }
    }
}

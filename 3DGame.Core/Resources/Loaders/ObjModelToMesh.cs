// Полный и правильный .obj-парсер с расчётом тангента
using System;
using System.Collections.Generic;
using OpenTK.Mathematics;
using _3DGame.Core.Graphics;

namespace _3DGame.Core.Resources.Loaders
{
    public class ObjModelToMesh
    {
        public static List<Mesh> Convert(ObjModel objModel)
        {
            var uniqueVertexMap = new Dictionary<(int, int, int), int>();
            var vertices = new List<Vertex>();
            var indices = new List<uint>();

            foreach (var face in objModel.Faces)
            {
                for (int i = 0; i < face.Vertices.Count - 2; i++)
                {
                    int[] idx = new int[] { 0, i + 1, i + 2 };

                    foreach (int j in idx)
                    {
                        var fv = face.Vertices[j];
                        var key = (fv.VertexIndex, fv.TexCoordIndex, fv.NormalIndex);

                        if (!uniqueVertexMap.TryGetValue(key, out int index))
                        {
                            var pos = objModel.Vertices[fv.VertexIndex];
                            var uv = fv.TexCoordIndex >= 0 ? objModel.TexCoords[fv.TexCoordIndex] : Vector2.Zero;
                            var normal = fv.NormalIndex >= 0 ? objModel.Normals[fv.NormalIndex] : Vector3.UnitY;

                            index = vertices.Count;
                            uniqueVertexMap[key] = index;
                            vertices.Add(new Vertex(pos, uv, normal));
                        }

                        indices.Add((uint)index);
                    }
                }
            }

            // Расчёт тангента
            Vector3[] tangents = new Vector3[vertices.Count];
            Vector3[] bitangents = new Vector3[vertices.Count];

            for (int i = 0; i < indices.Count; i += 3)
            {
                int i0 = (int)indices[i];
                int i1 = (int)indices[i + 1];
                int i2 = (int)indices[i + 2];

                var pos0 = vertices[i0].Position;
                var pos1 = vertices[i1].Position;
                var pos2 = vertices[i2].Position;

                var uv0 = vertices[i0].TexCoords;
                var uv1 = vertices[i1].TexCoords;
                var uv2 = vertices[i2].TexCoords;

                var deltaPos1 = pos1 - pos0;
                var deltaPos2 = pos2 - pos0;

                var deltaUV1 = uv1 - uv0;
                var deltaUV2 = uv2 - uv0;

                float denom = deltaUV1.X * deltaUV2.Y - deltaUV1.Y * deltaUV2.X;
                if (Math.Abs(denom) < 1e-6f) continue;
                float r = 1.0f / denom;

                var tangent = (deltaPos1 * deltaUV2.Y - deltaPos2 * deltaUV1.Y) * r;
                var bitangent = (deltaPos2 * deltaUV1.X - deltaPos1 * deltaUV2.X) * r;

                tangents[i0] += tangent;
                tangents[i1] += tangent;
                tangents[i2] += tangent;

                bitangents[i0] += bitangent;
                bitangents[i1] += bitangent;
                bitangents[i2] += bitangent;
            }

            for (int i = 0; i < vertices.Count; i++)
            {
                var normal = vertices[i].Normal;
                var tangent = Vector3.Normalize(tangents[i] - normal * Vector3.Dot(normal, tangents[i]));
                var bitangent = Vector3.Normalize(Vector3.Cross(normal, tangent));

                vertices[i] = new Vertex(vertices[i].Position, vertices[i].TexCoords, normal, tangent, bitangent);
            }

            var mesh = new Mesh(vertices.ToArray(), indices.ToArray())
            {
                Name = objModel.Name
            };

            return new List<Mesh> { mesh };
        }
    }
}
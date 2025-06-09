using _3DGame.Core.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace _3DGame.Core
{
    public static class DebugRender
    {
        private static int vao;
        private static int vbo;
        private static int _triVao, _triVbo;
        private static Shader? lineShader;
        private static bool initialized = false;

        private static List<(Vector3 min, Vector3 max, Vector4 color)> cubeQueue = new();
        private static List<(Vector3 start, Vector3 end, Vector4 color)> lineQueue = new();
        private static List<Vertex> triVertices = new();

        public static bool Enable = false;

        public static void Init(Shader shader)
        {
            lineShader = shader;

            vao = GL.GenVertexArray();
            vbo = GL.GenBuffer();

            GL.BindVertexArray(vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, 1000 * sizeof(float), IntPtr.Zero, BufferUsageHint.DynamicDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            // Треугольники
            _triVao = GL.GenVertexArray();
            _triVbo = GL.GenBuffer();
            GL.BindVertexArray(_triVao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _triVbo);
            GL.BufferData(BufferTarget.ArrayBuffer, 1000 * sizeof(float), IntPtr.Zero, BufferUsageHint.DynamicDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Marshal.SizeOf(typeof(Vertex)), 0);
            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);

            initialized = true;
        }

        public static void AddTriangle(Vector3 a, Vector3 b, Vector3 c, Color4 color)
        {
            triVertices.Add(new Vertex { Position = a, Color = color });
            triVertices.Add(new Vertex { Position = b, Color = color });
            triVertices.Add(new Vertex { Position = c, Color = color });
        }

        public static void AddCube(Vector3 min, Vector3 max, Vector4 color)
        {
            if (!initialized) return;
            cubeQueue.Add((min, max, color));
        }

        public static void AddLine(Vector3 start, Vector3 end, Vector4 color)
        {
            if (!initialized) return;
            lineQueue.Add((start, end, color));
        }

        public static void AddSphere(Vector3 center, float radius, Vector4 color, int segments = 16)
        {
            for (int i = 0; i < segments; i++)
            {
                float theta1 = (float)(i * MathF.PI * 2 / segments);
                float theta2 = (float)((i + 1) * MathF.PI * 2 / segments);

                for (int j = 1; j < segments; j++)
                {
                    float phi = (float)(j * MathF.PI / segments);

                    Vector3 p1 = new Vector3(
                        radius * MathF.Cos(theta1) * MathF.Sin(phi),
                        radius * MathF.Cos(phi),
                        radius * MathF.Sin(theta1) * MathF.Sin(phi)) + center;

                    Vector3 p2 = new Vector3(
                        radius * MathF.Cos(theta2) * MathF.Sin(phi),
                        radius * MathF.Cos(phi),
                        radius * MathF.Sin(theta2) * MathF.Sin(phi)) + center;

                    AddLine(p1, p2, color);
                }
            }

            // горизонтальные кольца
            for (int j = 1; j < segments; j++)
            {
                float phi = (float)(j * MathF.PI / segments);

                for (int i = 0; i < segments; i++)
                {
                    float theta1 = (float)(i * MathF.PI * 2 / segments);
                    float theta2 = (float)((i + 1) * MathF.PI * 2 / segments);

                    Vector3 p1 = new Vector3(
                        radius * MathF.Cos(theta1) * MathF.Sin(phi),
                        radius * MathF.Cos(phi),
                        radius * MathF.Sin(theta1) * MathF.Sin(phi)) + center;

                    Vector3 p2 = new Vector3(
                        radius * MathF.Cos(theta2) * MathF.Sin(phi),
                        radius * MathF.Cos(phi),
                        radius * MathF.Sin(theta2) * MathF.Sin(phi)) + center;

                    DrawLine(p1, p2, color);
                }
            }
        }

        public static void AddPolygonMesh(Vector3[] verts, (int, int, int)[] tris, Matrix4 transform, Vector4 color)
        {
            foreach (var t in tris)
            {
                var a = verts[t.Item1];
                var b = verts[t.Item2];
                var c = verts[t.Item3];

                //AddTriangle(a, b, c, Color4.Red);
                AddLine(a, b, color);
                AddLine(b, c, color);
                AddLine(c, a, color);
            }
        }

        public static void DrawAll(Matrix4 view, Matrix4 projection)
        {
            if(!Enable)
            {
                cubeQueue.Clear();
                lineQueue.Clear();

                return;
            }

            if (!initialized || lineShader == null) return;

            GL.Disable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            lineShader.Use();
            lineShader.SetMatrix4("view", view);
            lineShader.SetMatrix4("projection", projection);

            GL.BindVertexArray(vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);

            foreach (var (min, max, color) in cubeQueue)
            {
                DrawCube(min, max, color);
            }

            foreach (var (start, end, color) in lineQueue)
            {
                DrawLine(start, end, color);
            }


            GL.Enable(EnableCap.DepthTest);

            lineShader.SetVector4("color", new Vector4(1, 0, 0, 1));

            if (triVertices.Count > 0)
            {
                GL.BindVertexArray(_triVao);
                GL.BindBuffer(BufferTarget.ArrayBuffer, _triVbo);
                GL.BufferData(BufferTarget.ArrayBuffer, triVertices.Count * Marshal.SizeOf(typeof(Vertex)), triVertices.ToArray(), BufferUsageHint.DynamicDraw);
                GL.DrawArrays(PrimitiveType.Triangles, 0, triVertices.Count);
            }

            GL.Disable(EnableCap.Blend);

            cubeQueue.Clear();
            lineQueue.Clear();
            triVertices.Clear();


        }

        private static void DrawCube(Vector3 min, Vector3 max, Vector4 color)
        {
            var vertices = new List<Vector3>
            {
                new Vector3(min.X, min.Y, min.Z), new Vector3(max.X, min.Y, min.Z),
                new Vector3(max.X, min.Y, min.Z), new Vector3(max.X, min.Y, max.Z),
                new Vector3(max.X, min.Y, max.Z), new Vector3(min.X, min.Y, max.Z),
                new Vector3(min.X, min.Y, max.Z), new Vector3(min.X, min.Y, min.Z),

                new Vector3(min.X, max.Y, min.Z), new Vector3(max.X, max.Y, min.Z),
                new Vector3(max.X, max.Y, min.Z), new Vector3(max.X, max.Y, max.Z),
                new Vector3(max.X, max.Y, max.Z), new Vector3(min.X, max.Y, max.Z),
                new Vector3(min.X, max.Y, max.Z), new Vector3(min.X, max.Y, min.Z),

                new Vector3(min.X, min.Y, min.Z), new Vector3(min.X, max.Y, min.Z),
                new Vector3(max.X, min.Y, min.Z), new Vector3(max.X, max.Y, min.Z),
                new Vector3(max.X, min.Y, max.Z), new Vector3(max.X, max.Y, max.Z),
                new Vector3(min.X, min.Y, max.Z), new Vector3(min.X, max.Y, max.Z)
            };

            float[] verts = new float[vertices.Count * 3];
            for (int i = 0; i < vertices.Count; i++)
            {
                verts[i * 3 + 0] = vertices[i].X;
                verts[i * 3 + 1] = vertices[i].Y;
                verts[i * 3 + 2] = vertices[i].Z;
            }

            GL.BufferData(BufferTarget.ArrayBuffer, verts.Length * sizeof(float), verts, BufferUsageHint.DynamicDraw);
            lineShader!.SetMatrix4("model", Matrix4.Identity);
            lineShader.SetVector4("color", color);
            GL.DrawArrays(PrimitiveType.Lines, 0, vertices.Count);
        }

        private static void DrawLine(Vector3 start, Vector3 end, Vector4 color)
        {
            float[] lineVerts = {
                    start.X, start.Y, start.Z,
                    end.X, end.Y, end.Z
                };

            GL.BindVertexArray(vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, lineVerts.Length * sizeof(float), lineVerts, BufferUsageHint.DynamicDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            lineShader?.SetMatrix4("model", Matrix4.Identity);
            lineShader?.SetVector4("color", color);
            GL.DrawArrays(PrimitiveType.Lines, 0, 2);
        }
    }
}

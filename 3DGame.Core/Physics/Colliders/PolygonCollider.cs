using _3DGame.Core.Ecs.Components;
using _3DGame.Core.Graphics;
using OpenTK.Mathematics;

namespace _3DGame.Core.Physics.Colliders
{
    public class PolygonCollider : ColliderShape
    {
        public Matrix4 TransformMatrix { get; set; } = Matrix4.Identity;

        public Vector3[] Vertices { get; set; }

        public (int, int, int)[] Triangles { get; set; }

        public PolygonCollider() : base(ColliderShapeType.PolygonCollider)
        {
            Vertices = [];
            Triangles = [];
        }

        public PolygonCollider(Vector3[] vertices) : base(ColliderShapeType.PolygonCollider)
        {
            Vertices = vertices;
            Triangles = [];
        }

        public void SetMesh(params Mesh[] mesh)
        {
            foreach (var meshItem in mesh)
            {
                Vertices = new Vector3[meshItem.GetVerticesCount()];
                Triangles = new (int, int, int)[meshItem.GetIndicesCount() / 3];

                for (int i = 0; i < Vertices.Length; i++)
                {
                    Vertices[i] = meshItem.GetVertices()[i].Position;
                }

                int j = 0;

                var indices = meshItem.GetIndices();

                for (int i = 0; i < Triangles.Length && j < meshItem.GetIndicesCount(); i++)
                {
                    Triangles[i] = ((int)meshItem.GetIndices()[j + 0],
                                    (int)meshItem.GetIndices()[j + 1],
                                    (int)meshItem.GetIndices()[j + 2]);
                    j += 3;
                }
            }

            ComputeSize();
        }

        private void ComputeSize()
        {
            if (Vertices.Length == 0) return;

            Vector3 min = Vertices[0], max = Vertices[0];
            foreach (var v in Vertices)
            {
                min = Vector3.ComponentMin(min, v);
                max = Vector3.ComponentMax(max, v);
            }

            Size = max - min;
            OffsetPosition = -(min + Size / 2);
        }

        public override PolygonCollider Transform(Vector3 position)
        {
            var matrix = Matrix4.CreateTranslation(position + OffsetPosition);

            return new PolygonCollider()
            {
                Vertices = [.. Vertices],
                Triangles = [.. Triangles],
                TransformMatrix = matrix * this.TransformMatrix
            };
        }

        public override PolygonCollider Transform(Vector3 position, Vector3 rotation, Vector3 scale)
        {
            var matrix =
                Matrix4.CreateScale(scale) *
                Matrix4.CreateRotationX(MathHelper.DegreesToRadians(rotation.X)) *
                Matrix4.CreateRotationY(MathHelper.DegreesToRadians(rotation.Y)) *
                Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(rotation.Z)) *
                Matrix4.CreateTranslation(position + OffsetPosition);

            return new PolygonCollider()
            {
                Vertices = [.. Vertices],
                Triangles = [.. Triangles],
                TransformMatrix = matrix * this.TransformMatrix
            };
        }

        public override float GetVolume()
        {
            float volume = 0;
            foreach (var t in Triangles)
            {
                var a = Vertices[t.Item1];
                var b = Vertices[t.Item2];
                var c = Vertices[t.Item3];
                volume += Vector3.Dot(a, Vector3.Cross(b, c)) / 6f;
            }
            return MathF.Abs(volume) / 10;
        }

        public List<Vector3> GetWorldVertices()
        {
            var list = new List<Vector3>(Vertices.Length);
            foreach (var v in Vertices)
                list.Add(Vector3.TransformPosition(v, TransformMatrix));
            return list;
        }

        private List<Vector3> GetTriangleNormals(List<Vector3> verts)
        {
            var list = new List<Vector3>();
            foreach (var t in Triangles)
            {
                var a = verts[t.Item1];
                var b = verts[t.Item2];
                var c = verts[t.Item3];
                var normal = Vector3.Normalize(Vector3.Cross(b - a, c - a));
                list.Add(normal);
            }
            return list;
        }

        public override bool Intersects(ColliderShape other)
        {
            return Intersects(other, out _, out _);
        }

        public override bool Intersects(ColliderShape other, out Vector3 normal, out float depth)
        {
            normal = Vector3.Zero;
            depth = float.MaxValue;

            if (other is PolygonCollider otherPolygon)
            {
                var vertsA = GetWorldVertices();
                var vertsB = otherPolygon.GetWorldVertices();

                var axes = new List<Vector3>();
                axes.AddRange(GetTriangleNormals(vertsA));
                axes.AddRange(otherPolygon.GetTriangleNormals(vertsB));

                foreach (var axis in axes)
                {
                    if (axis == Vector3.Zero) continue;

                    ProjectVertices(vertsA, axis, out float minA, out float maxA);
                    ProjectVertices(vertsB, axis, out float minB, out float maxB);

                    if (maxA < minB || maxB < minA)
                    {
                        return false; // Separating axis found — no collision
                    }

                    float axisDepth = MathF.Min(maxA, maxB) - MathF.Max(minA, minB);
                    if (axisDepth < depth)
                    {
                        depth = axisDepth;
                        normal = axis;
                    }
                }

                // Гарантированно столкновение, normal направлен от A к B
                var centerA = Average(vertsA);
                var centerB = Average(vertsB);
                if (Vector3.Dot(centerB - centerA, normal) > 0)
                    normal = -normal;

                return true;
            }
            else if (other is BoxCollider box)
            {
                var vertsA = GetWorldVertices();
                var vertsB = GetBoxCorners(box.Min, box.Max);

                var axes = new List<Vector3>();
                axes.AddRange(GetTriangleNormals(vertsA));
                axes.AddRange(new[] { Vector3.UnitX, Vector3.UnitY, Vector3.UnitZ });

                foreach (var axis in axes)
                {
                    if (axis == Vector3.Zero) continue;

                    var axisN = Vector3.Normalize(axis);

                    ProjectVertices(vertsA, axisN, out float minA, out float maxA);
                    ProjectVertices(vertsB, axisN, out float minB, out float maxB);

                    if (maxA < minB || maxB < minA)
                    {
                        return false; // Separating axis found — no collision
                    }

                    float axisDepth = MathF.Min(maxA, maxB) - MathF.Max(minA, minB);
                    if (axisDepth < depth)
                    {
                        depth = axisDepth;
                        normal = axisN;
                    }
                }

                // Гарантированно столкновение, normal направлен от A к B
                var centerA = Average(vertsA);
                var centerB = Average(vertsB);
                if (Vector3.Dot(centerB - centerA, normal) > 0)
                    normal = -normal;

                return true;
            }
            else if(other is SphereCollider sphere)
            {
                var vertsA = GetWorldVertices();

                Vector3 closest = ClosestPointOnPolygon(sphere.Center, vertsA);

                Vector3 dir = closest - sphere.Center;
                float distSq = dir.LengthSquared;

                if (distSq > sphere.Radius * sphere.Radius)
                    return false;

                float dist = MathF.Sqrt(distSq);
                depth = sphere.Radius - dist;

                // если центр внутри полигона, инвертируем нормаль
                if (dist == 0)
                    normal = Vector3.UnitY;
                else
                    normal = dir.Normalized();

                return true;
            }

            return false;
        }

        private void ProjectVertices(List<Vector3> vertices, Vector3 axis, out float min, out float max)
        {
            min = Vector3.Dot(vertices[0], axis);
            max = min;

            for (int i = 1; i < vertices.Count; i++)
            {
                float proj = Vector3.Dot(vertices[i], axis);
                if (proj < min) min = proj;
                if (proj > max) max = proj;
            }
        }

        private Vector3 Average(List<Vector3> verts)
        {
            Vector3 sum = Vector3.Zero;
            foreach (var v in verts) sum += v;
            return sum / verts.Count;
        }

        private List<Vector3> GetBoxCorners(Vector3 min, Vector3 max)
        {
            return new List<Vector3>
            {
                new(min.X, min.Y, min.Z),
                new(min.X, min.Y, max.Z),
                new(min.X, max.Y, min.Z),
                new(min.X, max.Y, max.Z),
                new(max.X, min.Y, min.Z),
                new(max.X, min.Y, max.Z),
                new(max.X, max.Y, min.Z),
                new(max.X, max.Y, max.Z),
            };
        }

        private Vector3 ClosestPointOnPolygon(Vector3 point, List<Vector3> polygon)
        {
            float minDist = float.MaxValue;
            Vector3 closest = Vector3.Zero;

            for (int i = 0; i < polygon.Count; i++)
            {
                Vector3 a = polygon[i];
                Vector3 b = polygon[(i + 1) % polygon.Count];

                Vector3 closestOnEdge = ClosestPointOnSegment(point, a, b);
                float distSq = (point - closestOnEdge).LengthSquared;

                if (distSq < minDist)
                {
                    minDist = distSq;
                    closest = closestOnEdge;
                }
            }

            return closest;
        }

        private Vector3 ClosestPointOnSegment(Vector3 p, Vector3 a, Vector3 b)
        {
            Vector3 ab = b - a;
            float t = Vector3.Dot(p - a, ab) / ab.LengthSquared;
            t = Math.Clamp(t, 0, 1);
            return a + ab * t;
        }
    }
}

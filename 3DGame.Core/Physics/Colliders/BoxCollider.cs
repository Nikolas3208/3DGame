using _3DGame.Core.Ecs.Components;
using OpenTK.Mathematics;

namespace _3DGame.Core.Physics.Colliders
{
    public class BoxCollider : ColliderShape
    {
        public Vector3 Min {  get; set; }
        public Vector3 Max { get; set; }

        public BoxCollider() : base(ColliderShapeType.BoxCollider)
        {

        }
        public BoxCollider(Vector3 min, Vector3 max) : base(ColliderShapeType.BoxCollider)
        {
            Min = min; 
            Max = max;

            Size = max - min;
            OffsetPosition = -Size / 2;
        }

        public override void Start()
        {
            if(Min == Max)
            {
                var meshRander = GameObject?.GetComponent<MeshRender>()!;
            }
        }

        public override BoxCollider Transform(Vector3 position)
        {
            return new BoxCollider(Min + OffsetPosition + position, Max + OffsetPosition + position) { GameObject = this.GameObject };
        }

        public override BoxCollider Transform(Vector3 position, Vector3 rotation, Vector3 scale)
        {
            Matrix4 rot =
                    Matrix4.CreateScale(scale) *
                    Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(rotation.Z)) *
                    Matrix4.CreateRotationX(MathHelper.DegreesToRadians(rotation.X)) *
                    Matrix4.CreateRotationY(MathHelper.DegreesToRadians(rotation.Y));

            // Вершины локального куба
            Vector3[] corners = new Vector3[8];
            corners[0] = Min + OffsetPosition;
            corners[1] = new Vector3(Max.X, Min.Y, Min.Z) + OffsetPosition;
            corners[2] = new Vector3(Min.X, Max.Y, Min.Z) + OffsetPosition;
            corners[3] = new Vector3(Max.X, Max.Y, Min.Z) + OffsetPosition;
            corners[4] = new Vector3(Min.X, Min.Y, Max.Z) + OffsetPosition;
            corners[5] = new Vector3(Max.X, Min.Y, Max.Z) + OffsetPosition;
            corners[6] = new Vector3(Min.X, Max.Y, Max.Z) + OffsetPosition;
            corners[7] = Max + OffsetPosition;

            // Повернуть все вершины и сместить в мировую позицию
            for (int i = 0; i < 8; i++)
            {
                corners[i] = (Vector3.TransformVector(corners[i], rot) + position);
            }

            // Найти новый AABB
            Vector3 newMin = corners[0];
            Vector3 newMax = corners[0];

            for (int i = 1; i < 8; i++)
            {
                newMin = Vector3.ComponentMin(newMin, corners[i]);
                newMax = Vector3.ComponentMax(newMax, corners[i]);
            }
            
            return new BoxCollider(newMin, newMax);
        }

        public override float GetVolume()
        {
            return Size.X * Size.Y * Size.Z / 10;
        }

        public override bool Intersects(ColliderShape other)
        {
            if (other is BoxCollider box)
            {
                return (Min.X <= box.Max.X && Max.X >= box.Min.X) &&
                       (Min.Y <= box.Max.Y && Max.Y >= box.Min.Y) &&
                       (Min.Z <= box.Max.Z && Max.Z >= box.Min.Z);
            }
            else if(other is SphereCollider otherSphere)
            {
                Vector3 clamped = Vector3.Clamp(otherSphere.Center, Min, Max);
                Vector3 delta = otherSphere.Center - clamped;
                float distSq = delta.LengthSquared;

                if (distSq > otherSphere.Radius * otherSphere.Radius)
                    return false;

                float dist = MathF.Sqrt(distSq);

                return true;
            }
            else if(other is PolygonCollider otherPolygon)
            {
                return otherPolygon.Intersects(this);
            }

                return false;
        }

        public override bool Intersects(ColliderShape other, out Vector3 normal, out float depth)
        {
            normal = Vector3.Zero;
            depth = float.MaxValue;

            if (other is BoxCollider box)
            {
                if (!Intersects(box))
                    return false;

                // Найдём перекрытие по каждой оси
                float dx = MathF.Min(Max.X, box.Max.X) - MathF.Max(Min.X, box.Min.X);
                float dy = MathF.Min(Max.Y, box.Max.Y) - MathF.Max(Min.Y, box.Min.Y);
                float dz = MathF.Min(Max.Z, box.Max.Z) - MathF.Max(Min.Z, box.Min.Z);

                // Найдём минимальную глубину столкновения
                depth = MathF.Min(dx, MathF.Min(dy, dz));

                // Определим нормаль по минимальной глубине
                if (depth == dx)
                    normal = new Vector3(Min.X < box.Min.X ? -1 : 1, 0, 0);
                else if (depth == dy)
                    normal = new Vector3(0, Min.Y < box.Min.Y ? -1 : 1, 0);
                else if (depth == dz)
                    normal = new Vector3(0, 0, Min.Z < box.Min.Z ? -1 : 1);

                return true;
            }
            else if(other is SphereCollider otherSphere)
            {
                Vector3 clamped = Vector3.Clamp(otherSphere.Center, Min, Max);
                Vector3 delta = otherSphere.Center - clamped;
                float distSq = delta.LengthSquared;

                if (distSq > otherSphere.Radius * otherSphere.Radius)
                    return false;

                float dist = MathF.Sqrt(distSq);
                depth = otherSphere.Radius - dist;

                normal = dist > 0 ? delta / dist : Vector3.UnitX;

                return true;
            }
            else if (other is PolygonCollider otherPolygon)
            {
                bool result = otherPolygon.Intersects(this, out normal, out depth);
                normal = -normal;

                return result;
            }

            return false;
        }
    }
}

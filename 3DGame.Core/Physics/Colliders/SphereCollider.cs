using OpenTK.Mathematics;

namespace _3DGame.Core.Physics.Colliders
{
    public class SphereCollider : ColliderShape
    {
        public Vector3 Center { get; set; }

        public float Radius { get; set; }

        public SphereCollider(float radius, Vector3 center = new Vector3()) : base(ColliderShapeType.SphereCollider)
        {
            Radius = radius;
            Center = center;

            Size = new Vector3(Radius * 2);
        }

        public override SphereCollider Transform(Vector3 position)
        {
            return new SphereCollider(Radius, Center + position + OffsetPosition) { GameObject = this.GameObject };
        }

        public override SphereCollider Transform(Vector3 position, Vector3 rotation, Vector3 scale)
        {
            return new SphereCollider(Radius, (Center + OffsetPosition) * scale + position) { GameObject = this.GameObject };
        }

        public override float GetVolume()
        {
            return (4f / 3f) * MathF.PI * MathF.Pow(Radius, 3) / 10;
        }

        public override bool Intersects(ColliderShape other)
        {
            if (other is SphereCollider otherSphere)
            {
                float distance = Vector3.DistanceSquared(Center, otherSphere.Center);
                float radii = Radius + otherSphere.Radius;

                return distance < radii * radii;
            }
            else if(other is BoxCollider otherBox)
            {
                return otherBox.Intersects(this);
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

            if (other is SphereCollider otherSphere)
            {
                float distance = Vector3.DistanceSquared(Center, otherSphere.Center);
                float radii = Radius + otherSphere.Radius;

                normal = Vector3.Normalize(Center - otherSphere.Center);
                depth = radii - distance;

                return distance < radii;
            }
            else if(other is BoxCollider otherBox)
            {
                return otherBox.Intersects(this, out normal, out depth);
            }
            else if (other is PolygonCollider otherPolygon)
            {
                var result = otherPolygon.Intersects(this, out normal, out depth);
                //normal = -normal;

                return result;
            }

            return false;
        }
    }
}

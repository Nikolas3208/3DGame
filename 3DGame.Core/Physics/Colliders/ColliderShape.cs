using _3DGame.Core.Ecs;
using OpenTK.Mathematics;

namespace _3DGame.Core.Physics.Colliders
{
    public enum ColliderShapeType
    {
        BoxCollider,
        SphereCollider,
        PolygonCollider
    }

    public abstract class ColliderShape : Component
    {
        public Vector3 Size {  get; set; }

        public Vector3 OffsetPosition { get; set; }
        public Vector3 OffsetRotation { get; set; }

        public ColliderShapeType Type { get; set; }

        public ColliderShape(ColliderShapeType type)
        {
            Type = type;
        }

        public abstract ColliderShape Transform(Vector3 position);
        public abstract ColliderShape Transform(Vector3 position, Vector3 rotation, Vector3 scale);
        public abstract float GetVolume();
        public abstract bool Intersects(ColliderShape other);
        public abstract bool Intersects(ColliderShape other, out Vector3 normal, out float depth);
    }
}

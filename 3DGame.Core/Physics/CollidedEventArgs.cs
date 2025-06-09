using _3DGame.Core.Ecs;
using _3DGame.Core.Physics.Colliders;
using OpenTK.Mathematics;

namespace _3DGame.Core.Physics
{
    public struct CollidedEventArgs
    {
        public GameObject OtherObject { get; set; }
        public RigidBody OtherBody { get; set; }
        public ColliderShape OtherCollider { get; set; }

        public Vector3 Normal { get; set; }
        public float Depth { get; set; }

        public CollidedEventArgs(GameObject otherObject, RigidBody otherBody, ColliderShape otherCollider, Vector3 normal, float depth)
        {
            OtherObject = otherObject;
            OtherBody = otherBody;
            OtherCollider = otherCollider;
            Normal = normal;
            Depth = depth;
        }
    }
}

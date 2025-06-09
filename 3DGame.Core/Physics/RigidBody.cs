using _3DGame.Core.Ecs;
using _3DGame.Core.Ecs.Components;
using _3DGame.Core.Physics.Colliders;
using OpenTK.Mathematics;

namespace _3DGame.Core.Physics
{
    public enum RigidBodyType
    {
        Static,
        Dynamic,
        Kinematic
    }
    public class RigidBody : Component
    {
        private Transformable transformable;
        private ColliderShape colliderShape;

        public Vector3 Position { get => transformable.Position; set => transformable.Position = value; }

        public Vector3 Rotation { get => transformable.Rotation; set => transformable.Rotation = value; }
        public Vector3 Scale { get => transformable.Scale; set => transformable.Scale = value; }

        public Vector3 LinearVelocity { get; set; }

        public RigidBodyType Type { get; set; }

        public MassData MassData { get; set; }
        public PhysicsMaterial Material { get; set; }

        public bool IsTriger { get; set; } = false;

        public RigidBody(RigidBodyType type = RigidBodyType.Dynamic)
        {
            Type = type;

            transformable = new Transformable();
            colliderShape = null!;

            MassData = new MassData(40);

            Material = PhysicsMaterial.Default;
        }

        public override void Start()
        {
            transformable = GameObject?.GetComponent<Transformable>()!;

            colliderShape = GameObject?.GetComponent<ColliderShape>()!;

            if (colliderShape != null)
            {
                float mass = colliderShape.GetVolume() * Material.Density;

                MassData = new MassData(mass);
            }
        }

        public override void Update(float deltaTime)
        {
            if (GetCollider() is BoxCollider box)
            {
                DebugRender.AddCube(box.Min, box.Max, new Vector4(0, 1, 0, 1));
            }
            else if (GetCollider() is SphereCollider sphere)
            {
                DebugRender.AddSphere(sphere.Center, sphere.Radius, new Vector4(0, 1, 0, 1));
            }
            else if(GetCollider() is PolygonCollider polygon)
            {
                DebugRender.AddPolygonMesh(polygon.GetWorldVertices().ToArray(), polygon.Triangles, polygon.TransformMatrix, new Vector4(0, 1, 0, 1));
            }

            if (Position.Y <= -30)
            {
                Position = new Vector3(0, 10, 0);
                LinearVelocity = new Vector3();
            }

        }

        public void Step(float deltaTime, Vector3 gravity, int iteration)
        {
            if (Type == RigidBodyType.Static) return;

            float time = deltaTime / iteration;

            LinearVelocity += time * gravity * MassData.Mass;

            Position += LinearVelocity * time;
        }

        public void Move(Vector3 offset)
        {
            if (Type == RigidBodyType.Static) return;

            Position += offset;
        }

        public void AddLinearVelosity(Vector3 v)
        {
            if (Type != RigidBodyType.Static)
                LinearVelocity += v;
        }

        public void AddImpuls(Vector3 v)
        {
            if (Type == RigidBodyType.Static)
                return;

            AddLinearVelosity(v * MassData.InvMass);
        }

        public void OnCollided(CollidedEventArgs args)
        {
            GameObject?.GetComponent<ScriptComponent>()?.OnCollided(args);
        }

        public ColliderShape? GetCollider() => colliderShape?.Transform(Position, Rotation, Scale);
    }
}

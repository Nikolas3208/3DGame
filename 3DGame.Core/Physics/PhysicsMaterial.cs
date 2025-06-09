namespace _3DGame.Core.Physics
{
    public struct PhysicsMaterial
    {
        public float Density { get; }
        public float Restitution { get; }
        public float StaticFriction { get; }
        public float DynamicFriction { get; }

        public PhysicsMaterial(float density, float restitution, float staticFriction, float dynamicFriction)
        {
            Density = density;
            Restitution = restitution;
            StaticFriction = staticFriction;
            DynamicFriction = dynamicFriction;
        }

        public static PhysicsMaterial Default => new PhysicsMaterial(2, 0.0f, 0.6f, 0.3f);
    }
}

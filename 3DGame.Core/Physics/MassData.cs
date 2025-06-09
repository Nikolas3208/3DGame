namespace _3DGame.Core.Physics
{
    public struct MassData
    {
        public float Mass { get; set; }
        public float InvMass { get; set; }

        public float Inertia { get; set; }
        public float InvInertia { get; set; }

        public MassData(float mass)
        {
            Mass = mass;

            InvMass = Mass > 0 ? 1f / Mass : 0f;
        }

        public MassData(float mass, float inertia)
        {
            Mass = mass;
            Inertia = inertia;

            InvMass = Mass > 0 ? 1f / Mass : 0f;
            InvInertia = Inertia > 0 ? 1f / Inertia : 0f;
        }
    }
}

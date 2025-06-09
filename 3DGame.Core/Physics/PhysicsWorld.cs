using _3DGame.Core.Physics.Colliders;
using OpenTK.Mathematics;

namespace _3DGame.Core.Physics
{
    public class PhysicsWorld
    {
        private Scene scene;

        private List<(RigidBody, RigidBody)> contactPairs = [];
        private Vector3 gravity = new Vector3(0, -9.8f, 0);

        private int iteration = 4;

        public PhysicsWorld(Scene scene)
        {
            this.scene = scene;
        }

        public void FixedUpdate(float deltaTime)
        {
            var rigidBodys = scene.GetComponents<RigidBody>();

            for (int i = 0; i <= iteration; i++)
            {
                Step(deltaTime, iteration, rigidBodys);
            }
        }

        private void Step(float deltaTime, int iteration, List<RigidBody> bodies)
        {
            contactPairs.Clear();

            BodyStep(deltaTime, bodies.Where(r => r.Type != RigidBodyType.Static).ToList(), iteration);
            BroadPhase(bodies);
            NarrowPhase();
        }

        private void BroadPhase(List<RigidBody> bodies)
        {
            for (int i = 0; i < bodies.Count; i++)
            {
                var body = bodies[i];
                var collider = body.GetCollider();

                if (collider == null)
                    continue;


                for (int j = i + 1; j < bodies.Count; j++)
                {
                    var otherBody = bodies[j];
                    var otherCollider = otherBody.GetCollider();

                    if (otherCollider == null)
                        continue;

                    if (body.Type == RigidBodyType.Static && otherBody.Type == RigidBodyType.Static)
                        continue;

                    if (!collider.Intersects(otherCollider))
                        continue;

                    contactPairs.Add((body, otherBody));
                }
            }
        }

        private void NarrowPhase()
        {
            for (int i = 0; i < contactPairs.Count; i++)
            {
                var pair = contactPairs[i];

                var body = pair.Item1;
                var collider = body.GetCollider();

                var otherBody = pair.Item2;
                var otherCollider = otherBody.GetCollider();

                if (collider!.Intersects(otherCollider!, out Vector3 normal, out float depth))
                {
                    body.OnCollided(new CollidedEventArgs(otherBody.GameObject!, otherBody, otherCollider!, normal, depth));
                    otherBody.OnCollided(new CollidedEventArgs(body.GameObject!, body, collider, normal, depth));

                    SeparateBodies(body, otherBody, normal, depth);

                    PositionalCorrection(body, otherBody, normal, depth);
                    ResolveCollision(body, otherBody, normal, depth);
                }
            }
        }

        private void SeparateBodies(RigidBody body, RigidBody otherBody, Vector3 normal, float depth)
        {
            if (body.Type == RigidBodyType.Static)
            {
                otherBody.Move(-normal * depth);
            }
            else if (otherBody.Type == RigidBodyType.Static)
            {
                body.Move(normal * depth);
            }
            else
            {
                body.Move(normal * depth / 2);
                otherBody.Move(-normal * depth / 2);
            }
        }

        private void BodyStep(float deltaTime, List<RigidBody> bodies, int iteration)
        {
            foreach (var body in bodies)
            {
                if (body.Type != RigidBodyType.Static)
                    body.Step(deltaTime, gravity, iteration);
            }
        }

        private void PositionalCorrection(RigidBody body, RigidBody otherBody, Vector3 normal, float depth)
        {
            const float percent = 0.8f; // степень коррекции
            const float slop = 0.01f; // минимальное проникновение, которое игнорируем

            float correctionMagnitude = MathF.Max(depth - slop, 0f) / (body.MassData.InvMass + otherBody.MassData.InvMass);
            Vector3 correction = correctionMagnitude * percent * normal;

            body.Move(-correction * body.MassData.InvMass);
            otherBody.Move(correction * otherBody.MassData.InvMass);
        }

        private void ResolveCollision(RigidBody bodyA, RigidBody bodyB, Vector3 normal, float depth)
        {
            Vector3 relativeVelocity = bodyA.LinearVelocity - bodyB.LinearVelocity;

            Vector3.Dot(in relativeVelocity, in normal, out float contacVelosityMag);

            if (contacVelosityMag > 0f)
            {
                return;
            }

            float e = MathF.Min(bodyA.Material.Restitution, bodyB.Material.Restitution);

            Vector3 impulse = GetBodyMoment(normal, relativeVelocity, e, bodyA.MassData.InvMass, bodyB.MassData.InvMass);

            bodyA.AddImpuls(impulse);
            bodyB.AddImpuls(-impulse);
        }

        private Vector3 GetBodyMoment(Vector3 normal, Vector3 relativeVeloity, float e, float invMassA, float invMassB)
        {
            float j = -(1f + e) * Vector3.Dot(relativeVeloity, normal);
            j /= invMassA + invMassB;

            return j * normal;
        }
    }
}

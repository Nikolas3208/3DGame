using OpenTK.Mathematics;

namespace _3DGame.Core.Ecs.Components
{
    public class Transformable : Component
    {
        private Vector3 position;
        private Vector3 rotation;
        private Vector3 scale;

        private Matrix4 transform = Matrix4.Identity;

        public Transformable()
        {
            position = new Vector3(0);
            rotation = new Vector3(0);
            scale = new Vector3(1);
        }

        public Vector3 Position { get => position; set { position = value; UpdateTransform(); } }
        public Vector3 Rotation { get => rotation; set { rotation = value; UpdateTransform(); } }
        public Vector3 Scale { get => scale; set { scale = value; UpdateTransform(); } }

        public Matrix4 Transform => transform;

        private void UpdateTransform()
        {
            transform = Matrix4.Identity;
            transform *= Matrix4.CreateScale(Scale);
            transform *= Matrix4.CreateRotationX(MathHelper.DegreesToRadians(Rotation.X));
            transform *= Matrix4.CreateRotationY(MathHelper.DegreesToRadians(Rotation.Y));
            transform *= Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(Rotation.Z));
            transform *= Matrix4.CreateTranslation(Position);
        }
    }
}

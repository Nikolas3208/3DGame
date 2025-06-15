using OpenTK.Mathematics;

namespace _3DGame.Core.Ecs.Components
{
    public class Transform : Component
    {
        private Vector3 position;
        private Vector3 rotation;
        private Vector3 scale;

        private Matrix4 transformModel = Matrix4.Identity;

        public Transform()
        {
            position = new Vector3(0);
            rotation = new Vector3(0);
            scale = new Vector3(1);
        }

        public Vector3 Position { get => position; set { position = value; UpdateTransform(); } }
        public Vector3 Rotation { get => rotation; set { rotation = value; UpdateTransform(); } }
        public Vector3 Scale { get => scale; set { scale = value; UpdateTransform(); } }

        public Matrix4 TransformModel => transformModel;

        private void UpdateTransform()
        {
            transformModel = Matrix4.Identity;
            transformModel *= Matrix4.CreateScale(Scale);
            transformModel *= Matrix4.CreateRotationX(MathHelper.DegreesToRadians(Rotation.X));
            transformModel *= Matrix4.CreateRotationY(MathHelper.DegreesToRadians(Rotation.Y));
            transformModel *= Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(Rotation.Z));
            transformModel *= Matrix4.CreateTranslation(Position);
        }
    }
}

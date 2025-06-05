using _3DGame.Core.Graphics;
using OpenTK.Mathematics;

namespace _3DGame.Core.Ecs.Components
{
    public class Transformable : Component
    {
        private Vector3 position;
        private Quaternion rotation;
        private Vector3 scale;

        private Matrix4 transform = Matrix4.Identity;

        public string Name { get; } = nameof(Transformable);
        public GameObject Perent { get; }

        public Transformable(GameObject parent)
        {
            Perent = parent;
        }

        public Vector3 Position { get => position; set { position = value; UpdateTransform(); } }
        public Quaternion Rotation { get => rotation; set { rotation = value; UpdateTransform(); } }
        public Vector3 Scale { get => scale; set { scale = value; UpdateTransform(); } }

        public Matrix4 Transform => transform;

        private void UpdateTransform()
        {
            transform = Matrix4.Identity;
            transform *= Matrix4.CreateScale(Scale);
            transform *= Matrix4.CreateFromQuaternion(Rotation);
            transform *= Matrix4.CreateTranslation(Position);
        }

        public void Start()
        {
            position = new Vector3();
            rotation = new Quaternion(0, 0, 0);
            scale = new Vector3(1);
        }

        public void Update(float deltaTime)
        {

        }

        public void Draw(Renderer renderer)
        {

        }
    }
}

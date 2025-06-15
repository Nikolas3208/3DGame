using _3DGame.Core;
using _3DGame.Core.Ecs;
using _3DGame.Core.Ecs.Components;
using _3DGame.Core.Graphics;
using _3DGame.Core.Physics;
using _3DGame.Core.Physics.Colliders;
using _3DGame.Core.Resources.Loaders;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace _3DGame
{
    public class MyScene : Scene
    {
        private GameObject mainCamera;

        public MyScene(Vector2i size, Game game) : base(size, game)
        {
            game.GetWindow().UpdateFrequency = 60;

            GL.Enable(EnableCap.CullFace);
            GL.CullFace(TriangleFace.Back);

            mainCamera = new GameObject(this, "Main camera");
            mainCamera.AddComponent(new Camera(size.X / (float)size.Y));
            mainCamera.GetComponent<Transform>()!.Position = new Vector3(0, 0, 10);

            AddGameObject(mainCamera);

            SetCamera(mainCamera.GetComponent<Camera>()!);

            var light = new GameObject(this, "light");
            light.AddComponent(Light.Directional);
            light.AddComponent(new MeshRender(MeshLoader.LoadMesh("Assets\\GreenCube.obj")));

            light.GetComponent<Transform>()!.Position = new Vector3(2.5f, -2.5f, 0f);
            light.GetComponent<Transform>()!.Scale = new Vector3(0.2f);

            AddGameObject(light);

            var cube = new GameObject(this, "cube");
            cube.AddComponent(new MeshRender(MeshLoader.LoadMesh("Assets\\Cube.obj")));
            cube.AddComponent(new RigidBody(RigidBodyType.Dynamic));
            cube.AddComponent(new BoxCollider(new Vector3(), new Vector3(2)));
            cube.GetComponent<Transform>()!.Position = new Vector3(0, 0, 0);
            cube.GetComponent<Transform>()!.Scale = new Vector3(0.5f);

            AddGameObject(cube);

            var plain = new GameObject(this, "plain");
            plain.AddComponent(new MeshRender(MeshLoader.LoadMesh("Assets\\Cube.obj")));
            plain.AddComponent(new RigidBody(RigidBodyType.Static));
            plain.GetComponent<Transform>()!.Position = new Vector3(0, -10, 0);
            plain.GetComponent<Transform>()!.Scale = new Vector3(10, 0.2f, 10);

            var polygonCollider = new PolygonCollider();
            polygonCollider.SetMesh(MeshLoader.LoadMesh("Assets\\Cube.obj"));

            plain.AddComponent(polygonCollider);

            AddGameObject(plain);

            var cube2 = new GameObject(this, "plain");
            cube2.AddComponent(new MeshRender(MeshLoader.LoadMesh("Assets\\Cube.obj")));
            cube2.AddComponent(new RigidBody(RigidBodyType.Dynamic));
            cube2.GetComponent<Transform>()!.Position = new Vector3(3, -9, 0);

            polygonCollider = new PolygonCollider();
            polygonCollider.SetMesh(MeshLoader.LoadMesh("Assets\\Cube.obj"));

            cube2.AddComponent(polygonCollider);

            AddGameObject(cube2);

            cube2.AddComponent(new PlayerController());

            DebugRender.Init(new Shader("Shaders\\line.vert", "Shaders\\line.frag"));
        }

        public override void Start()
        {
            base.Start();
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
        }

        public override void FixedUpdate(float fixedDeltaTime)
        {
            base.FixedUpdate(fixedDeltaTime);
        }

        public override void Draw(Renderer renderer)
        {
            base.Draw(renderer);

            DebugRender.DrawAll(activeCamera!.GetViewMatrix(), activeCamera!.GetProjectionMatrix());
        }

        public override void MouseWheel(Vector2 offset)
        {
            base.MouseWheel(offset);

            mainCamera.GetComponent<Camera>()!.Rotation -= new Vector3(0, offset.Y, 0);
        }
    }
}

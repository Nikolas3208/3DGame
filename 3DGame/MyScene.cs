using _3DGame.Core;
using _3DGame.Core.Ecs;
using _3DGame.Core.Ecs.Components;
using _3DGame.Core.Graphics;
using _3DGame.Core.Physics;
using _3DGame.Core.Physics.Colliders;
using _3DGame.Core.Resources;
using _3DGame.Core.Resources.Loaders;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace _3DGame
{
    public class MyScene : Scene
    {
        private GameObject mainCamera;

        private bool cursoreGrabbed = false;

        private float cameraSpeed = 1.5f;
        private float sensitivity = 0.2f;

        public MyScene(Vector2i size, Game game) : base(size, game)
        {
            game.GetWindow().UpdateFrequency = 60;

            GL.Enable(EnableCap.CullFace);
            GL.CullFace(TriangleFace.Back);

            mainCamera = new GameObject(this, "Main camera");
            mainCamera.AddComponent(new Camera(new Vector3(), size.X / (float)size.Y));
            mainCamera.GetComponent<Transformable>()!.Position = new Vector3(0, 0, 10);

            AddGameObject(mainCamera);

            SetCamera(mainCamera.GetComponent<Camera>()!);

            var light = new GameObject(this, "light");
            light.AddComponent(Light.Directional);
            light.AddComponent(new MeshRender(MeshLoader.LoadMesh("Assets\\GreenCube.obj")));

            light.GetComponent<Transformable>()!.Position = new Vector3(2.5f, -2.5f, 0f);
            light.GetComponent<Transformable>()!.Scale = new Vector3(0.2f);

            AddGameObject(light);

            var cube = new GameObject(this, "cube");
            cube.AddComponent(new MeshRender(MeshLoader.LoadMesh("Assets\\Cube.obj")));
            cube.AddComponent(new RigidBody(RigidBodyType.Dynamic));
            cube.AddComponent(new BoxCollider(new Vector3(), new Vector3(2)));
            cube.GetComponent<Transformable>()!.Position = new Vector3(0, 0, 0);
            cube.GetComponent<Transformable>()!.Scale = new Vector3(0.5f);

            AddGameObject(cube);

            var plain = new GameObject(this, "plain");
            plain.AddComponent(new MeshRender(MeshLoader.LoadMesh("Assets\\Cube.obj")));
            plain.AddComponent(new RigidBody(RigidBodyType.Static));
            plain.GetComponent<Transformable>()!.Position = new Vector3(0, -10, 0);
            plain.GetComponent<Transformable>()!.Scale = new Vector3(10, 0.2f, 10);

            var polygonCollider = new PolygonCollider();
            polygonCollider.SetMesh(MeshLoader.LoadMesh("Assets\\Cube.obj"));

            plain.AddComponent(polygonCollider);

            AddGameObject(plain);

            var cube2 = new GameObject(this, "plain");
            cube2.AddComponent(new MeshRender(MeshLoader.LoadMesh("Assets\\Cube.obj")));
            cube2.AddComponent(new RigidBody(RigidBodyType.Dynamic));
            cube2.GetComponent<Transformable>()!.Position = new Vector3(3, -9, 0);

            polygonCollider = new PolygonCollider();
            polygonCollider.SetMesh(MeshLoader.LoadMesh("Assets\\Cube.obj"));

            cube2.AddComponent(polygonCollider);

            AddGameObject(cube2);

            DebugRender.Init(new Shader("Shaders\\line.vert", "Shaders\\line.frag"));

            ScriptsManager.LoadScripts("Assets");

            cube.AddComponent(ScriptsManager.GetScript("PlayerController")!);
        }

        public override void Start()
        {
            base.Start();
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);            

            var transform = mainCamera.GetComponent<Transformable>();
            var camera = mainCamera.GetComponent<Camera>();

            if(Keyboard.IsKeyReleased(Keys.C))
            {
                if (!cursoreGrabbed)
                {
                    game.GetWindow().CursorState = CursorState.Grabbed;

                    cursoreGrabbed = true;
                }
                else
                {
                    game.GetWindow().CursorState = CursorState.Normal;

                    cursoreGrabbed = false;
                }
            }

            if (camera != null && transform != null)
            {
                if (false)
                {
                    if (Keyboard.IsKeyDown(Keys.W))
                    {
                        transform.Position += camera.Front * cameraSpeed * deltaTime;
                    }
                    if (Keyboard.IsKeyDown(Keys.S))
                    {
                        transform.Position -= camera.Front * cameraSpeed * deltaTime;
                    }
                    if (Keyboard.IsKeyDown(Keys.A))
                    {
                        transform.Position -= camera.Right * cameraSpeed * deltaTime;
                    }
                    if (Keyboard.IsKeyDown(Keys.D))
                    {
                        transform.Position += camera.Right * cameraSpeed * deltaTime;
                    }
                    if (Keyboard.IsKeyDown(Keys.LeftShift))
                    {
                        transform.Position -= camera.Up * cameraSpeed * deltaTime;
                    }
                }

                var deltaX = Mouse.GetDeltaPosition().X;
                var deltaY = Mouse.GetDeltaPosition().Y;

                camera.Yaw += deltaX * sensitivity;
                //camera.Pitch -= deltaY * sensitivity;
            }
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

            mainCamera.GetComponent<Camera>()!.Fov -= offset.Y;
        }
    }
}

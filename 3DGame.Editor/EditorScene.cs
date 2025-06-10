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

namespace _3DGame.Editor
{
    public class EditorScene : Scene
    {
        private GameObject mainCamera;

        private bool cursoreGrabbed = false;

        private float cameraSpeed = 1.5f;
        private float sensitivity = 0.2f;

        public EditorScene(Vector2i size, Game game) : base(size, game)
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


            AddGameObject(light);

            var cube = new GameObject(this, "cube");
            cube.AddComponent(new MeshRender(MeshLoader.LoadMesh("Assets\\Cube.obj")));
            cube.GetComponent<Transformable>()!.Position = new Vector3(0, 1, -1);

            AddGameObject(cube);
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            activeCamera?.UpdateAspectRatio(EditorUI.SceneWindowSize.X / (float)EditorUI.SceneWindowSize.Y);

            var transform = mainCamera.GetComponent<Transformable>();
            var camera = mainCamera.GetComponent<Camera>();

            if (Keyboard.IsKeyReleased(Keys.C))
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

            if (camera != null && transform != null && Mouse.IsButtonDown(MouseButton.Right))
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

                var deltaX = Mouse.GetDeltaPosition().X;
                var deltaY = Mouse.GetDeltaPosition().Y;

                camera.Yaw += deltaX * sensitivity;
                camera.Pitch -= deltaY * sensitivity;
            }
        }
    }
}

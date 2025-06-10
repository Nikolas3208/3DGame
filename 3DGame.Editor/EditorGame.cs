using _3DGame.Core;
using _3DGame.Core.Buffers;
using _3DGame.Core.Graphics;
using ImGuiNET;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace _3DGame.Editor
{
    public class EditorGame : Game
    {
        private ImGuiController imGuiController;
        private FrameBuffer frameBuffer;
        public EditorGame(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
        {
            imGuiController = new ImGuiController(nativeWindowSettings.ClientSize.X, nativeWindowSettings.ClientSize.Y);
            frameBuffer = new FrameBuffer(nativeWindowSettings.ClientSize.X, nativeWindowSettings.ClientSize.Y);
            frameBuffer.Init();

            var editorScene = new EditorScene(nativeWindowSettings.ClientSize, this);

            AddScene(editorScene);
            SetActiveScene(editorScene);
        }

        protected override void Follows(Window window)
        {
            base.Follows(window);

            window.TextInput += (TextInputEventArgs args) => { imGuiController.PressChar((char)args.Unicode); };
        }

        protected override void Start()
        {
            base.Start();
        }

        protected override void Update(float deltaTime)
        {
            EditorUI.SetScene(activeScene!);

            imGuiController.Update(window, deltaTime);

            base.Update(deltaTime);
        }

        protected override void Draw(Renderer renderer)
        {
            frameBuffer.Bind();
            activeScene?.Draw(renderer);
            frameBuffer.Unbind();
            GL.Viewport(0, 0, window.ClientSize.X, window.ClientSize.Y);

            EditorUI.SetSceneImage(frameBuffer.TextureHandle);

            EditorUI.Draw();

            imGuiController.Render();

            window.SwapBuffers();
        }

        protected override void Resize(ResizeEventArgs obj)
        {
            base.Resize(obj);

            frameBuffer = new FrameBuffer(obj.Width, obj.Height);
            frameBuffer.Init();

            imGuiController.WindowResized(obj.Width, obj.Height);
        }

        protected override void MouseWheel(Vector2 offset)
        {
            base.MouseWheel(offset);

            imGuiController.MouseScroll(offset);
        }
    }
}

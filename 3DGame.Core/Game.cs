using _3DGame.Core.Graphics;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace _3DGame.Core
{
    public abstract class Game
    {
        protected List<Scene> scenes;
        protected Scene? activeScene;
        protected Window window;
        protected Renderer renderer;

        public Game(Window window)
        {
            this.window = window;

            window.Load += () => { Start(); };
            window.UpdateFrame += (FrameEventArgs e) => { Update((float)e.Time); };
            window.RenderFrame += (FrameEventArgs e) => { Draw(renderer); };
            window.Resize += Resize;

            scenes = new List<Scene>();
        }

        public Game(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
        {
            window = new Window(gameWindowSettings, nativeWindowSettings);
            window.Load += () => { Start(); };
            window.UpdateFrame += (FrameEventArgs e) => { Update((float)e.Time); };
            window.RenderFrame += (FrameEventArgs e) => 
            { Draw(renderer); };
            window.Resize += Resize;
            window.MouseWheel += (MouseWheelEventArgs e) => { MouseWheel(e.Offset); };

            scenes = new List<Scene>();
        }

        protected virtual void Start()
        {
            renderer = Renderer.Default;

            activeScene?.Start();
        }

        protected virtual void Update(float deltaTime)
        {
            Keyboard.Update(window.KeyboardState);
            Mouse.Update(window.MouseState);

            window.Title = (1f / deltaTime).ToString();

            activeScene?.Update(deltaTime);
        }

        protected virtual void Draw(Renderer renderer)
        {
            activeScene?.Draw(renderer);

            window.SwapBuffers();
        }

        protected virtual void Resize(ResizeEventArgs obj)
        {
            activeScene?.Resize(obj.Size);
        }

        protected virtual void MouseWheel(Vector2 offset)
        {
            activeScene?.MouseWheel(offset);
        }

        public bool AddScene(Scene scene)
        {
            if (!scenes.Contains(scene))
            {
                scenes.Add(scene);

                return true;
            }

            return false;
        }

        public Scene? GetSceneAt(int index)
        {
            if (index < 0 || index >= scenes.Count)
                return null;

            return scenes[index];
        }

        public bool RemoveScene(Scene scene)
        {
            return scenes.Remove(scene);
        }

        public bool SetActiveScene(Scene scene)
        {
            if(scene == null)
                return false;

            if (activeScene != null)
                activeScene.IsActive = false;

            activeScene = scene;
            activeScene.IsActive = true;

            return true;
        }

        public Scene? GetAciveScene() => activeScene;

        public Window GetWindow() => window;
    }
}

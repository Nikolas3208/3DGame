using _3DGame.Core.Graphics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace _3DGame
{
    public class Game
    {
        private Window window;

        public Game(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
        {
            window = new Window(gameWindowSettings, nativeWindowSettings);

            window.Load += Start;
            window.UpdateFrame += Update;
            window.RenderFrame += Draw;
        }

        public void Run() => window.Run();


        private void Start()
        {

        }

        private void Update(FrameEventArgs obj)
        {
        }

        private void Draw(FrameEventArgs obj)
        {

        }
    }
}

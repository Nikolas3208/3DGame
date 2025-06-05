using OpenTK.Windowing.Desktop;

namespace _3DGame.Core.Graphics
{
    public class Window : GameWindow
    {
        private Camera camera;

        public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
        {
            
        }
    }
}

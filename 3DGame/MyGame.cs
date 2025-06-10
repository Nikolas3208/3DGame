using _3DGame.Core;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;

namespace _3DGame
{
    public class MyGame : Game
    {
        public MyGame(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
        {
            AddScene(new MyScene(nativeWindowSettings.ClientSize, this));

            SetActiveScene(GetSceneAt(0)!);
        }

        protected override void Start()
        {
            base.Start();
        }
    }
}

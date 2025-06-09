using _3DGame.Core.Graphics;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;

namespace _3DGame
{
    public class Program
    {
        public static void Main(string[] args)
        {
            MyGame game = new MyGame(GameWindowSettings.Default, new NativeWindowSettings() { Title = "3D Game", Size = new Vector2i(1920, 1080) });

            game.Run();
        }
    }
}
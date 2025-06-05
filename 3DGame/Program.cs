using _3DGame.Core.Graphics;
using OpenTK.Windowing.Desktop;

namespace _3DGame
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Game game = new Game(GameWindowSettings.Default, NativeWindowSettings.Default);

            game.Run();
        }
    }
}
using _3DGame.Core;
using _3DGame.Editor;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;

public class Program
{
    public static void Main(string[] args)
    {
        var game = new EditorGame(GameWindowSettings.Default, new NativeWindowSettings() { Title = "3D Game", ClientSize = new Vector2i(1920, 1080) });

        game.Run();
    }
}
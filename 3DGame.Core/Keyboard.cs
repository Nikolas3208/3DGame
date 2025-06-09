using OpenTK.Windowing.GraphicsLibraryFramework;

namespace _3DGame.Core
{
    public static class Keyboard
    {
        private static KeyboardState currentState;

        public static void Update(KeyboardState state)
        {
            currentState = state;
        }

        public static bool IsKeyPressed(Keys key)
        {
            return currentState.IsKeyPressed(key);
        }

        public static bool IsKeyDown(Keys key)
        {
            return currentState.IsKeyDown(key);
        }

        public static bool IsKeyReleased(Keys key)
        {
            return currentState.IsKeyReleased(key);
        }
    }
}

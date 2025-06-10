using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace _3DGame.Core
{

    public static class Mouse
    {
        private static MouseState _currentState;

        public static void Update(MouseState state)
        {
            _currentState = state;
        }

        public static bool IsButtonPressed(MouseButton button)
        {
            return _currentState.IsButtonPressed(button);
        }

        public static bool IsButtonDown(MouseButton button)
        {
            return _currentState.IsButtonDown(button);
        }

        public static bool IsButtonReleased(MouseButton button)
        {
            return _currentState.IsButtonReleased(button);
        }

        public static Vector2 GetPosition() => _currentState.Position;
        public static Vector2 GetPreviousPosition() => _currentState.PreviousPosition;
        public static Vector2 GetDeltaPosition() => GetPosition() - GetPreviousPosition();
        public static float ScrollDelta => _currentState.Scroll.Y - _currentState.PreviousScroll.Y;
    }
}
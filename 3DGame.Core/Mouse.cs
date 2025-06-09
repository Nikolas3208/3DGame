using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace _3DGame.Core
{

    public static class Mouse
    {
        private static MouseState _currentState;
        private static MouseState _previousState;

        public static void Update(MouseState state)
        {
            _previousState = _currentState;
            _currentState = state;
        }

        public static bool IsButtonPressed(MouseButton button)
        {
            return _currentState.IsButtonDown(button) && !_previousState.IsButtonDown(button);
        }

        public static bool IsButtonDown(MouseButton button)
        {
            return _currentState.IsButtonDown(button);
        }

        public static bool IsButtonReleased(MouseButton button)
        {
            return !_currentState.IsButtonDown(button) && _previousState.IsButtonDown(button);
        }

        public static Vector2 GetPosition() => _currentState.Position;
        public static Vector2 GetPreviousPosition() => _currentState.PreviousPosition;
        public static Vector2 GetDeltaPosition() => GetPosition() - GetPreviousPosition();
        public static float ScrollDelta => _currentState.Scroll.Y - _previousState.Scroll.Y;
    }
}
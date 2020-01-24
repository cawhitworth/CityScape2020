using System;
using SharpDX;
using SharpDX.DirectInput;

namespace CityScape2020
{
    class Input : IInput, IDisposable
    {
        private readonly DirectInput input = new DirectInput();
        private readonly Mouse mouse;
        private readonly Keyboard keyboard;
        private KeyboardState keyboardState;
        private MouseState mouseState;
        private Vector2 mousePosition;

        public Input(IntPtr windowHandle)
        {
            mouse = new Mouse(input);
            keyboard = new Keyboard(input);
            keyboard.SetCooperativeLevel(windowHandle, CooperativeLevel.Foreground | CooperativeLevel.Exclusive);

            mouse.Properties.AxisMode = DeviceAxisMode.Relative;
            mouse.SetCooperativeLevel(windowHandle, CooperativeLevel.Foreground | CooperativeLevel.Exclusive);

        }

        public void Update()
        {
            try
            {
                keyboardState = new KeyboardState();
                keyboard.GetCurrentState(ref keyboardState);
            }
            catch
            {
                keyboardState = null;
            }

            if (keyboardState == null)
                TryAcquireKeyboard();

            try
            {
                mouseState = new MouseState();
                mouse.GetCurrentState(ref mouseState);
                mousePosition.X = mouseState.X;
                mousePosition.Y = mouseState.Y;
            }
            catch
            {
                mouseState = null;
            }

            if (mouseState == null)
                TryAcquireMouse();
        }

        private void TryAcquireKeyboard()
        {
            try
            {
                keyboard.Acquire();
            }
            catch
            {
                keyboardState = null;
            }
        }

        private void TryAcquireMouse()
        {
            try
            {
                mouse.Acquire();
            }
            catch
            {
                mouseState = null;
            }
        }

        public bool IsKeyDown(Key key)
        {
            return keyboardState != null && keyboardState.IsPressed(key);
        }

        public bool IsKeyUp(Key key)
        {
            return keyboardState != null && !keyboardState.IsPressed(key);
        }

        public Vector2 MousePosition()
        {
            return mousePosition;
        }

        public void Dispose()
        {
            mouse.Unacquire();
            mouse.Dispose();
            
            keyboard.Unacquire();
            keyboard.Dispose();

            input.Dispose();
        }
    }
}
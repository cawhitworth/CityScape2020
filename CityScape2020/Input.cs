// <copyright file="Input.cs" company="Chris Whitworth">
// Copyright (c) Chris Whitworth. All rights reserved.
// </copyright>

namespace CityScape2020
{
    using System;
    using SharpDX;
    using SharpDX.DirectInput;

    internal class Input : IInput, IDisposable
    {
        private readonly DirectInput input = new DirectInput();
        private readonly Mouse mouse;
        private readonly Keyboard keyboard;
        private KeyboardState keyboardState;
        private MouseState mouseState;
        private Vector2 mousePosition;

        public Input(IntPtr windowHandle)
        {
            this.mouse = new Mouse(this.input);
            this.keyboard = new Keyboard(this.input);
            this.keyboard.SetCooperativeLevel(windowHandle, CooperativeLevel.Foreground | CooperativeLevel.Exclusive);

            this.mouse.Properties.AxisMode = DeviceAxisMode.Relative;
            this.mouse.SetCooperativeLevel(windowHandle, CooperativeLevel.Foreground | CooperativeLevel.Exclusive);
        }

        public void Update()
        {
            try
            {
                this.keyboardState = new KeyboardState();
                this.keyboard.GetCurrentState(ref this.keyboardState);
            }
            catch
            {
                this.keyboardState = null;
            }

            if (this.keyboardState == null)
            {
                this.TryAcquireKeyboard();
            }

            try
            {
                this.mouseState = new MouseState();
                this.mouse.GetCurrentState(ref this.mouseState);
                this.mousePosition.X = this.mouseState.X;
                this.mousePosition.Y = this.mouseState.Y;
            }
            catch
            {
                this.mouseState = null;
            }

            if (this.mouseState == null)
            {
                this.TryAcquireMouse();
            }
        }

        public bool IsKeyDown(Key key)
        {
            return this.keyboardState != null && this.keyboardState.IsPressed(key);
        }

        public bool IsKeyUp(Key key)
        {
            return this.keyboardState != null && !this.keyboardState.IsPressed(key);
        }

        public Vector2 MousePosition()
        {
            return this.mousePosition;
        }

        public void Dispose()
        {
            this.mouse.Unacquire();
            this.mouse.Dispose();

            this.keyboard.Unacquire();
            this.keyboard.Dispose();

            this.input.Dispose();
        }

        private void TryAcquireKeyboard()
        {
            try
            {
                this.keyboard.Acquire();
            }
            catch
            {
                this.keyboardState = null;
            }
        }

        private void TryAcquireMouse()
        {
            try
            {
                this.mouse.Acquire();
            }
            catch
            {
                this.mouseState = null;
            }
        }
    }
}
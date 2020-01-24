// <copyright file="IInput.cs" company="Chris Whitworth">
// Copyright (c) Chris Whitworth. All rights reserved.
// </copyright>

namespace CityScape2020
{
    using SharpDX;
    using SharpDX.DirectInput;

    internal interface IInput
    {
        bool IsKeyDown(Key key);

        bool IsKeyUp(Key key);

        Vector2 MousePosition();
    }
}
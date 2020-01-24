// <copyright file="Program.cs" company="Chris Whitworth">
// Copyright (c) Chris Whitworth. All rights reserved.
// </copyright>

namespace CityScape2020
{
    using System;

    internal static class Program
    {
        [STAThread]
        public static void Main()
        {
            var app = new App();
            app.Run();
        }
    }
}

// <copyright file="StoryCalculator.cs" company="Chris Whitworth">
// Copyright (c) Chris Whitworth. All rights reserved.
// </copyright>

namespace CityScape2020.Buildings
{
    using System;
    using SharpDX;

    internal class StoryCalculator
    {
        private readonly Size2 textureSize;
        private readonly Size2 windowSize;
        private readonly Random random;

        public StoryCalculator(Size2 textureSize, Size2 windowSize, float storySize)
        {
            this.textureSize = textureSize;
            this.windowSize = windowSize;
            this.StorySize = storySize;
            this.random = new Random();
        }

        public float StorySize { get; }

        private int StoriesX => this.textureSize.Width / this.windowSize.Width;

        private int StoriesY => this.textureSize.Height / this.windowSize.Height;

        public Vector2 RandomPosition()
        {
            return new Vector2(this.random.Next(this.StoriesX), this.random.Next(this.StoriesY));
        }

        public Vector2 ToTexture(Vector2 p)
        {
            return new Vector2(this.ToTextureX((int)p.X), this.ToTextureY((int)p.Y));
        }

        private float ToTextureX(int stories)
        {
            return stories * ((float)this.windowSize.Width / this.textureSize.Width);
        }

        private float ToTextureY(int stories)
        {
            return stories * ((float)this.windowSize.Height / this.textureSize.Height);
        }
    }
}
using System;
using SharpDX;

namespace CityScape2020.Buildings
{
    class StoryCalculator
    {
        private readonly Size2 textureSize;
        private readonly Size2 windowSize;
        private readonly Random random;

        public StoryCalculator(Size2 textureSize, Size2 windowSize, float storySize)
        {
            this.textureSize = textureSize;
            this.windowSize = windowSize;
            StorySize = storySize;
            random = new Random();
        }

        public float StorySize { get; }

        public int StoriesX => textureSize.Width / windowSize.Width;
        public int StoriesY => textureSize.Height / windowSize.Height;

        public Vector2 RandomPosition()
        {
            return new Vector2(random.Next(StoriesX), random.Next(StoriesY));
        }

        public float ToTextureX(int stories)
        {
            return stories*((float)windowSize.Width/textureSize.Width);
        }

        public float ToTextureY(int stories)
        {
            return stories*((float)windowSize.Height/textureSize.Height);
        }

        public Vector2 ToTexture(Vector2 p)
        {
            return new Vector2(ToTextureX((int)p.X), ToTextureY((int)p.Y));
        }
    }
}
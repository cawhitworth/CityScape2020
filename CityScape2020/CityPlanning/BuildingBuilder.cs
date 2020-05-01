// <copyright file="BuildingBuilder.cs" company="Chris Whitworth">
// Copyright (c) Chris Whitworth. All rights reserved.
// </copyright>

namespace CityScape2020.CityPlanning
{
    using System;
    using CityScape2020.Buildings;
    using CityScape2020.Geometry;
    using SharpDX;

    internal class BuildingBuilder
    {
        private readonly StoryCalculator storyCalculator;
        private readonly BuildingBlockBuilder buildingBuilder;
        private readonly ColumnedBuildingBlockBuilder columnedBuildingBuilder;
        private readonly Random random;

        public BuildingBuilder()
        {
            this.random = new Random();

            var windowSize = new Size2(8, 8);
            var textureSize = new Size2(512, 512);

            this.storyCalculator = new StoryCalculator(textureSize, windowSize, 0.05f);
            this.buildingBuilder = new BuildingBlockBuilder(this.storyCalculator);
            this.columnedBuildingBuilder = new ColumnedBuildingBlockBuilder(this.storyCalculator);
        }

        public IGeometry MakeBuilding(int x, int y, int xStories, int yStories, int zStories)
        {
            var c1 = new Vector3(x * this.storyCalculator.StorySize, 0, y * this.storyCalculator.StorySize);

            IGeometry building;

            int buildingPick = this.random.Next(10);

            if (yStories > 15)
            {
                building = buildingPick > 7
                    ? this.buildingBuilder.Build(c1, xStories, yStories, zStories)
                    : new ClassicBuilding(c1, xStories, yStories, zStories, this.storyCalculator);
            }
            else
            {
                building = buildingPick > 7
                    ? this.buildingBuilder.Build(c1, xStories, yStories, zStories)
                    : this.columnedBuildingBuilder.Build(c1, xStories, yStories, zStories);
            }

            return building;
        }

        public IGeometry MakeBase(int x, int y, int w, int h)
        {
            var c1 = new Vector3(x * this.storyCalculator.StorySize, -this.storyCalculator.StorySize, y * this.storyCalculator.StorySize);
            var c2 = new Vector3(c1.X + (this.storyCalculator.StorySize * w), 0.0f, c1.Z + (this.storyCalculator.StorySize * h));

            return new Box(c1, c2);
        }

        private void PickBuildingSize(out int xSize, out int ySize, out int zSize)
        {
            var sizeGroup = this.random.Next(10);
            if (sizeGroup < 3)
            {
                ySize = this.random.Next(10);
                xSize = this.random.Next(10) + 10;
                zSize = this.random.Next(10) + 10;
            }
            else if (sizeGroup < 9)
            {
                ySize = this.random.Next(30);
                xSize = this.random.Next(18) + 2;
                zSize = this.random.Next(18) + 2;
            }
            else
            {
                ySize = this.random.Next(60);
                xSize = this.random.Next(15) + 5;
                zSize = this.random.Next(15) + 5;
            }

            ySize += 1;
        }
    }
}

// <copyright file="ColumnedBuildingBlockBuilder.cs" company="Chris Whitworth">
// Copyright (c) Chris Whitworth. All rights reserved.
// </copyright>

namespace CityScape2020.Buildings
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using CityScape2020.Geometry;
    using SharpDX;

    internal class ColumnedBuildingBlockBuilder
    {
        private readonly StoryCalculator storyCalculator;
        private readonly ModColor modColor;
        private readonly Random random;
        private Func<int, IEnumerable<int>> generateColumns;

        public ColumnedBuildingBlockBuilder(StoryCalculator storyCalculator, Func<int, IEnumerable<int>> generateColumns = null)
        {
            this.storyCalculator = storyCalculator;
            this.random = new Random();
            this.modColor = new ModColor(this.random);

            this.generateColumns = generateColumns ?? this.GenerateColumns;
        }

        public IGeometry Build(Vector3 c1, int xStories, int yStories, int zStories)
        {
            var c2 = new Vector3(
                c1.X + (xStories * this.storyCalculator.StorySize),
                c1.Y + (yStories * this.storyCalculator.StorySize),
                c1.Z + (zStories * this.storyCalculator.StorySize));

            var mod = this.modColor.Pick();

            var storyWidthsX = this.generateColumns(xStories).ToArray();
            var storyWidthsZ = this.generateColumns(zStories).ToArray();

            var front = new ColumnedPanel(c1, yStories, storyWidthsX, Panel.Plane.XY, Panel.Facing.Out, mod, this.storyCalculator);
            var back = new ColumnedPanel(c2, yStories, storyWidthsX, Panel.Plane.XY, Panel.Facing.In, mod, this.storyCalculator);

            var right = new ColumnedPanel(new Vector3(c2.X, c1.Y, c1.Z), yStories, storyWidthsZ, Panel.Plane.YZ, Panel.Facing.Out, mod, this.storyCalculator);
            var left = new ColumnedPanel(new Vector3(c1.X, c2.Y, c2.Z), yStories, storyWidthsZ, Panel.Plane.YZ, Panel.Facing.In, mod, this.storyCalculator);

            var tx1 = new Vector2(0, 0);

            var top = new Panel(
                new Vector3(c1.X, c2.Y, c1.Z),
                new Vector2(c2.X - c1.X, c2.Z - c1.Z),
                Panel.Plane.XZ,
                Panel.Facing.Out,
                tx1,
                tx1,
                mod);

            var aggregate = new AggregateGeometry(front, back, right, left, top);

            return aggregate;
        }

        private IEnumerable<int> GenerateColumns(int width)
        {
            var spacers = new bool[width];
            var offset = 0;
            while (offset < width / 2)
            {
                offset += this.random.Next(width / 3) + 2;
                if (offset >= width / 2)
                {
                    break;
                }

                spacers[offset] = true;
                spacers[(width - 1) - offset] = true;

                if (this.random.Next(2) == 1)
                {
                    spacers[offset + 1] = true;
                    spacers[(width - 1) - (offset + 1)] = true;
                    offset += 1;
                }
            }

            var w = 0;
            var window = true;
            foreach (var isSpacer in spacers)
            {
                if (window)
                {
                    if (isSpacer)
                    {
                        yield return w;
                        window = false;
                        w = 1;
                    }
                    else
                    {
                        w += 1;
                    }
                }
                else
                {
                    if (isSpacer)
                    {
                        w += 1;
                    }
                    else
                    {
                        yield return w;
                        window = true;
                        w = 1;
                    }
                }
            }

            yield return w;
        }
    }
}
// <copyright file="ClassicBuilding.cs" company="Chris Whitworth">
// Copyright (c) Chris Whitworth. All rights reserved.
// </copyright>

namespace CityScape2020.Buildings
{
    using System;
    using System.Collections.Generic;
    using CityScape2020.Geometry;
    using CityScape2020.Rendering;
    using SharpDX;

    internal class ClassicBuilding : IGeometry
    {
        private AggregateGeometry aggregateGeometry;

        public ClassicBuilding(Vector3 corner, int widthStories, int heightStories, int depthStories, StoryCalculator storyCalc)
        {
            var builder = new ColumnedBuildingBlockBuilder(storyCalc);

            var random = new Random();
            int totalHeight = 0;

            var geometry = new List<IGeometry>();

            var oppCorner = new Vector3(
                corner.X + (widthStories * storyCalc.StorySize),
                corner.Y - 0.5f,
                corner.Z + (depthStories * storyCalc.StorySize));

            Vector3 center = (oppCorner + corner) / 2;
            center.Y = 0.0f;

            // Base of the building
            geometry.Add(new Box(corner, oppCorner));

            var tierScale = 0.6 + (random.NextDouble() * 0.4);

            widthStories -= 1;
            depthStories -= 1;
            while (totalHeight < heightStories)
            {
                corner.X = center.X - ((widthStories / 2.0f) * storyCalc.StorySize);
                corner.Z = center.Z - ((depthStories / 2.0f) * storyCalc.StorySize);
                corner.Y = center.Y + (totalHeight * storyCalc.StorySize);

                int tierHeight;
                if (heightStories - totalHeight < 5)
                {
                    tierHeight = heightStories - totalHeight;
                }
                else
                {
                    tierHeight = heightStories * 2;

                    while (totalHeight + tierHeight > heightStories && tierHeight != 0)
                    {
                        tierHeight = heightStories - totalHeight > totalHeight / 3
                            ? random.Next((heightStories * 5) / 6) + (heightStories / 6)
                            : heightStories - totalHeight;
                    }
                }

                geometry.Add(builder.Build(corner, widthStories, tierHeight, depthStories));

                totalHeight += tierHeight;

                widthStories = (int)(tierScale * widthStories);
                depthStories = (int)(tierScale * depthStories);
                if (widthStories < 1)
                {
                    widthStories = 1;
                }

                if (depthStories < 1)
                {
                    depthStories = 1;
                }
            }

            this.aggregateGeometry = new AggregateGeometry(geometry);
        }

        public IEnumerable<ushort> Indices => this.aggregateGeometry.Indices;

        public IEnumerable<VertexPosNormalTextureMod> Vertices => this.aggregateGeometry.Vertices;
    }
}

// <copyright file="City.cs" company="Chris Whitworth">
// Copyright (c) Chris Whitworth. All rights reserved.
// </copyright>

namespace CityScape2020
{
    using System;
    using System.Collections.Generic;
    using CityScape2020.Buildings;
    using CityScape2020.Geometry;
    using CityScape2020.Rendering;
    using SharpDX;
    using SharpDX.Direct3D11;
    using Device = SharpDX.Direct3D11.Device;

    internal class City : Component
    {
        private readonly DeviceContext deviceContext;
        private readonly BatchedGeometryRenderer batchedRenderer;
        private readonly VertexPosNormalTextureModShader vertexShader;
        private readonly PixelTextureLightShader pixelShader;
        private readonly Random random;
        private readonly StoryCalculator storyCalculator;
        private readonly BuildingBlockBuilder buildingBuilder;
        private readonly ColumnedBuildingBlockBuilder columnedBuildingBuilder;

        public City(Device device, DeviceContext context)
        {
            this.deviceContext = context;

            var windowSize = new Size2(8, 8);
            var textureSize = new Size2(512, 512);

            this.storyCalculator = new StoryCalculator(textureSize, windowSize, 0.05f);
            this.buildingBuilder = new BuildingBlockBuilder(this.storyCalculator);
            this.columnedBuildingBuilder = new ColumnedBuildingBlockBuilder(this.storyCalculator);

            var buildingTexture = new BuildingTexture(device, context, textureSize, windowSize);

            var texture = Texture.FromTexture2D(buildingTexture.Texture, device);

            this.pixelShader = new PixelTextureLightShader(device, texture);
            this.vertexShader = new VertexPosNormalTextureModShader(device);

            var boxes = new List<IGeometry>();
            this.random = new Random();

            for (int x = -40; x < 41; x++)
            {
                for (int y = -40; y < 41; y++)
                {
                    boxes.Add(this.MakeBuilding(x, y));
                }
            }

            var geometryBatcher = new GeometryBatcher(boxes, 3000);

            var vertexSize = (Utilities.SizeOf<Vector3>() * 3) + Utilities.SizeOf<Vector2>();

            this.batchedRenderer = new BatchedGeometryRenderer(geometryBatcher, device, vertexSize, this.vertexShader.Layout);
        }

        public int Draw(long elapsed, Matrix view, Matrix proj)
        {
            // float fElapsed = elapsed / 1000.0f;
            // var world = Matrix.RotationY(fElapsed * 0.2f);// * Matrix.RotationX(fElapsed * 0.7f);
            var world = Matrix.Identity;
            world.Transpose();

            this.vertexShader.Bind(this.deviceContext, world, view, proj);
            this.pixelShader.Bind(this.deviceContext);

            return this.batchedRenderer.Render(this.deviceContext);
        }

        private IGeometry MakeBuilding(int x, int y)
        {
            int yStories, xStories, zStories;
            this.PickBuildingSize(out xStories, out yStories, out zStories);

            var c1 = new Vector3(x - 0.5f, 0, y - 0.5f);

            c1.X += (20 - xStories) * this.storyCalculator.StorySize / 2.0f;
            c1.Z += (20 - zStories) * this.storyCalculator.StorySize / 2.0f;

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

            var buildingBase = new Box(new Vector3(x - 0.5f, -0.5f, y - 0.5f), new Vector3(x + 0.5f, 0.0f, y + 0.5f));

            return new AggregateGeometry(buildingBase, building);
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
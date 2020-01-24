using System;
using System.Collections.Generic;
using CityScape2020.Buildings;
using CityScape2020.Geometry;
using CityScape2020.Rendering;
using SharpDX;
using SharpDX.Direct3D11;
using Device = SharpDX.Direct3D11.Device;

namespace CityScape2020
{
    class City : Component
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
            deviceContext = context;

            var windowSize = new Size2(8,8);
            var textureSize = new Size2(512,512);

            storyCalculator = new StoryCalculator(textureSize, windowSize, 0.05f);
            buildingBuilder = new BuildingBlockBuilder(storyCalculator);
            columnedBuildingBuilder = new ColumnedBuildingBlockBuilder(storyCalculator);

            var buildingTexture = new BuildingTexture(device, context, textureSize, windowSize);

            var texture = Texture.FromTexture2D(buildingTexture.Texture, device);

            pixelShader = new PixelTextureLightShader(device, texture);
            vertexShader = new VertexPosNormalTextureModShader(device);

            var boxes = new List<IGeometry>();
            random = new Random();

            for (int x = -40; x < 41; x++)
            {
                for (int y = -40; y < 41; y++)
                {
                    boxes.Add(MakeBuilding(x, y));
                }
            }
            var geometryBatcher = new GeometryBatcher(boxes, 3000);

            var vertexSize = Utilities.SizeOf<Vector3>()*3 + Utilities.SizeOf<Vector2>();

            batchedRenderer = new BatchedGeometryRenderer(geometryBatcher, device, vertexSize, vertexShader.Layout);

        }

        private IGeometry MakeBuilding(int x, int y)
        {
            int yStories, xStories, zStories;
            PickBuildingSize(out xStories, out yStories, out zStories);

            var c1 = new Vector3(x - 0.5f, 0, y - 0.5f);

            c1.X += ((20 - xStories)*storyCalculator.StorySize)/2.0f;
            c1.Z += ((20 - zStories)*storyCalculator.StorySize)/2.0f;

            IGeometry building;

            var buildingPick = random.Next(10);

            if (yStories > 15)
            {
                if (buildingPick > 7)
                {
                    building = buildingBuilder.Build(c1, xStories, yStories, zStories);
                }
                else
                {
                    building = new ClassicBuilding(c1, xStories, yStories, zStories, storyCalculator);
                }
            }
            else
            {
                if (buildingPick > 7)
                {
                    building = buildingBuilder.Build(c1, xStories, yStories, zStories);
                }
                else 
                {
                    building = columnedBuildingBuilder.Build(c1, xStories, yStories, zStories);
                }
            }

            var buildingBase = new Box(new Vector3(x - 0.5f, -0.5f, y - 0.5f), new Vector3(x + 0.5f, 0.0f, y + 0.5f));

            return new AggregateGeometry(buildingBase, building);
        }

        private void PickBuildingSize(out int xSize, out int ySize, out int zSize)
        {
            var sizeGroup = random.Next(10);
            if (sizeGroup < 3)
            {
                ySize = random.Next(10);
                xSize = random.Next(10) + 10;
                zSize = random.Next(10) + 10;
            }
            else if (sizeGroup < 9)
            {
                ySize = random.Next(30);
                xSize = random.Next(18) + 2;
                zSize = random.Next(18) + 2;
            }
            else
            {
                ySize = random.Next(60);
                xSize = random.Next(15) + 5;
                zSize = random.Next(15) + 5;
            }

            ySize += 1;
        }


        public int Draw(long elapsed, Matrix view, Matrix proj)
        {
            //float fElapsed = elapsed / 1000.0f;
            //var world = Matrix.RotationY(fElapsed * 0.2f);// * Matrix.RotationX(fElapsed * 0.7f);

            var world = Matrix.Identity;
            world.Transpose();

            vertexShader.Bind(deviceContext, world, view, proj);
            pixelShader.Bind(deviceContext);

            return batchedRenderer.Render(deviceContext);

        }
    }
}
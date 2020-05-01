// <copyright file="City.cs" company="Chris Whitworth">
// Copyright (c) Chris Whitworth. All rights reserved.
// </copyright>

namespace CityScape2020
{
    using System;
    using System.Collections.Generic;
    using CityScape2020.Buildings;
    using CityScape2020.CityPlanning;
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

        public City(Device device, DeviceContext context)
        {
            this.deviceContext = context;

            var windowSize = new Size2(8, 8);
            var textureSize = new Size2(512, 512);

            var buildingTexture = new BuildingTexture(device, context, textureSize, windowSize);

            var texture = Texture.FromTexture2D(buildingTexture.Texture, device);

            this.pixelShader = new PixelTextureLightShader(device, texture);
            this.vertexShader = new VertexPosNormalTextureModShader(device);

            this.random = new Random();

            var cityPlanner = new CityPlanner();
            var boxes = cityPlanner.BuildCity();

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
    }
}
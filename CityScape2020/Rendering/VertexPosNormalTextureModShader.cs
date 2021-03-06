// <copyright file="VertexPosNormalTextureModShader.cs" company="Chris Whitworth">
// Copyright (c) Chris Whitworth. All rights reserved.
// </copyright>

namespace CityScape2020.Rendering
{
    using System.IO;
    using SharpDX;
    using SharpDX.Direct3D11;
    using SharpDX.DXGI;
    using Device = SharpDX.Direct3D11.Device;
    using MapFlags = SharpDX.Direct3D11.MapFlags;

    internal class VertexPosNormalTextureModShader : Component
    {
        private readonly Buffer constantBuffer;
        private readonly VertexShader vertexShader;

        public VertexPosNormalTextureModShader(Device device)
        {
            var vertShaderBytecode = File.ReadAllBytes("VertexShader.cso");
            this.vertexShader = this.ToDispose(new VertexShader(device, vertShaderBytecode));

            InputElement[] elements = new[]
            {
                new InputElement("POSITION", 0, Format.R32G32B32_Float, 0, 0),
                new InputElement("NORMAL", 0, Format.R32G32B32_Float, 12, 0),
                new InputElement("TEXCOORD", 0, Format.R32G32_Float, 24, 0),
                new InputElement("TEXCOORD", 1, Format.R32G32B32_Float, 32, 0),
            };

            this.Layout = this.ToDispose(new InputLayout(device, vertShaderBytecode, elements));

            this.constantBuffer =
                this.ToDispose(new Buffer(
                    device,
                    Utilities.SizeOf<Matrix>() * 3,
                    ResourceUsage.Dynamic,
                    BindFlags.ConstantBuffer,
                    CpuAccessFlags.Write,
                    ResourceOptionFlags.None,
                    0));
        }

        public InputLayout Layout { get; }

        public void Bind(DeviceContext context, Matrix world, Matrix view, Matrix proj)
        {
            context.VertexShader.Set(this.vertexShader);
            context.VertexShader.SetConstantBuffer(0, this.constantBuffer);

            DataStream mappedResource;
            context.MapSubresource(this.constantBuffer, MapMode.WriteDiscard, MapFlags.None, out mappedResource);
            mappedResource.Write(world);
            mappedResource.Write(view);
            mappedResource.Write(proj);
            context.UnmapSubresource(this.constantBuffer, 0);
        }
    }
}
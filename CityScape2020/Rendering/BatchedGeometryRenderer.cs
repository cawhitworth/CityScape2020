// <copyright file="BatchedGeometryRenderer.cs" company="Chris Whitworth">
// Copyright (c) Chris Whitworth. All rights reserved.
// </copyright>

namespace CityScape2020.Rendering
{
    using System.Linq;
    using CityScape2020.Geometry;
    using SharpDX;
    using SharpDX.Direct3D;
    using SharpDX.Direct3D11;
    using SharpDX.DXGI;
    using Device = SharpDX.Direct3D11.Device;
    using MapFlags = SharpDX.Direct3D11.MapFlags;

    internal class BatchedGeometryRenderer : Component
    {
        private readonly Buffer vertices;
        private readonly Buffer indices;
        private readonly IGeometryBatcher geometryBatcher;
        private readonly int vertexSize;
        private readonly InputLayout inputLayout;

        public BatchedGeometryRenderer(IGeometryBatcher batcher, Device device, int vertexSize, InputLayout layout)
        {
            this.vertices =
                this.ToDispose(new Buffer(
                    device,
                    vertexSize * batcher.MaxVertexBatchSize,
                    ResourceUsage.Dynamic,
                    BindFlags.VertexBuffer,
                    CpuAccessFlags.Write,
                    ResourceOptionFlags.None,
                    0));

            this.indices =
                this.ToDispose(new Buffer(
                    device,
                    batcher.MaxIndexBatchSize * 2,
                    ResourceUsage.Dynamic,
                    BindFlags.IndexBuffer,
                    CpuAccessFlags.Write,
                    ResourceOptionFlags.None,
                    0));

            this.geometryBatcher = batcher;
            this.vertexSize = vertexSize;
            this.inputLayout = layout;
        }

        public int Render(DeviceContext context)
        {
            context.InputAssembler.InputLayout = this.inputLayout;
            context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(this.vertices, this.vertexSize, 0));
            context.InputAssembler.SetIndexBuffer(this.indices, Format.R16_UInt, 0);

            return this.geometryBatcher.VertexBatches.Zip(this.geometryBatcher.IndexBatches, (vertices, indices) =>
            {
                DataStream mappedResource;
                context.MapSubresource(this.indices, MapMode.WriteDiscard, MapFlags.None, out mappedResource);
                mappedResource.WriteRange(indices);
                context.UnmapSubresource(this.indices, 0);

                context.MapSubresource(this.vertices, MapMode.WriteDiscard, MapFlags.None, out mappedResource);
                mappedResource.WriteRange(vertices);
                context.UnmapSubresource(this.vertices, 0);

                context.DrawIndexed(indices.Count(), 0, 0);

                return indices.Count();
            }).Aggregate(0, (a, b) => a + b) / 3;
        }
    }
}
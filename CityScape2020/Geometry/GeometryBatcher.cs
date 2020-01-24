// <copyright file="GeometryBatcher.cs" company="Chris Whitworth">
// Copyright (c) Chris Whitworth. All rights reserved.
// </copyright>

namespace CityScape2020.Geometry
{
    using System.Collections.Generic;
    using System.Linq;
    using CityScape2020.Rendering;

    internal class GeometryBatcher : IGeometryBatcher
    {
        private readonly List<ushort[]> indexBatches = new List<ushort[]>();
        private readonly List<VertexPosNormalTextureMod[]> vertexBatches = new List<VertexPosNormalTextureMod[]>();

        public GeometryBatcher(IEnumerable<IGeometry> geometries, int desiredBatchSize)
        {
            var currentVertexBatch = new List<VertexPosNormalTextureMod>();
            var currentIndexBatch = new List<ushort>();

            this.MaxIndexBatchSize = 0;
            var indexBase = 0;
            foreach (var geometry in geometries)
            {
                if (currentIndexBatch.Count + geometry.Indices.Count() > desiredBatchSize * 3)
                {
                    if (currentIndexBatch.Count > this.MaxIndexBatchSize)
                    {
                        this.MaxIndexBatchSize = currentIndexBatch.Count;
                    }

                    if (currentVertexBatch.Count > this.MaxVertexBatchSize)
                    {
                        this.MaxVertexBatchSize = currentVertexBatch.Count;
                    }

                    this.indexBatches.Add(currentIndexBatch.ToArray());
                    this.vertexBatches.Add(currentVertexBatch.ToArray());
                    currentIndexBatch.Clear();
                    currentVertexBatch.Clear();
                    indexBase = 0;
                }

                currentVertexBatch.AddRange(geometry.Vertices);
                currentIndexBatch.AddRange(geometry.Indices.Select(i => (ushort)(i + indexBase)));
                indexBase += geometry.Vertices.Count();
            }

            if (currentIndexBatch.Count > this.MaxIndexBatchSize)
            {
                this.MaxIndexBatchSize = currentIndexBatch.Count;
            }

            if (currentVertexBatch.Count > this.MaxVertexBatchSize)
            {
                this.MaxVertexBatchSize = currentVertexBatch.Count;
            }

            this.indexBatches.Add(currentIndexBatch.ToArray());
            this.vertexBatches.Add(currentVertexBatch.ToArray());
        }

        public int MaxVertexBatchSize { get; }

        public int MaxIndexBatchSize { get; }

        public IEnumerable<ushort[]> IndexBatches => this.indexBatches;

        public IEnumerable<VertexPosNormalTextureMod[]> VertexBatches => this.vertexBatches;
    }
}
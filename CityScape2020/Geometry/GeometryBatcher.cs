using System.Collections.Generic;
using System.Linq;
using CityScape2020.Rendering;

namespace CityScape2020.Geometry
{
    class GeometryBatcher : IGeometryBatcher
    {
        private readonly List<ushort[]> indexBatches = new List<ushort[]>();
        private readonly List<VertexPosNormalTextureMod[]> vertexBatches = new List<VertexPosNormalTextureMod[]>();

        public GeometryBatcher(IEnumerable<IGeometry> geometries, int desiredBatchSize)
        {
            var currentVertexBatch = new List<VertexPosNormalTextureMod>();
            var currentIndexBatch = new List<ushort>();

            MaxIndexBatchSize = 0;
            var indexBase = 0;
            foreach (var geometry in geometries)
            {
                if (currentIndexBatch.Count + geometry.Indices.Count() > desiredBatchSize*3)
                {
                    if (currentIndexBatch.Count > MaxIndexBatchSize)
                        MaxIndexBatchSize = currentIndexBatch.Count;
                    if (currentVertexBatch.Count > MaxVertexBatchSize)
                        MaxVertexBatchSize = currentVertexBatch.Count;

                    indexBatches.Add(currentIndexBatch.ToArray());
                    vertexBatches.Add(currentVertexBatch.ToArray());
                    currentIndexBatch.Clear();
                    currentVertexBatch.Clear();
                    indexBase = 0;
                }

                currentVertexBatch.AddRange(geometry.Vertices);
                currentIndexBatch.AddRange(geometry.Indices.Select(i => (ushort)(i + indexBase)));
                indexBase += geometry.Vertices.Count();
            }

            if (currentIndexBatch.Count > MaxIndexBatchSize)
            {
                MaxIndexBatchSize = currentIndexBatch.Count;
            }

            if (currentVertexBatch.Count > MaxVertexBatchSize)
            {
                MaxVertexBatchSize = currentVertexBatch.Count;
            }

            indexBatches.Add(currentIndexBatch.ToArray());
            vertexBatches.Add(currentVertexBatch.ToArray());
        }

        public int MaxVertexBatchSize { get; }

        public int MaxIndexBatchSize { get; }

        public IEnumerable<ushort[]> IndexBatches => indexBatches;

        public IEnumerable<VertexPosNormalTextureMod[]> VertexBatches => vertexBatches;
    }
}
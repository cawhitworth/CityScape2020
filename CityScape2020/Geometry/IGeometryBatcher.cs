using System.Collections.Generic;
using CityScape2020.Rendering;

namespace CityScape2020.Geometry
{
    internal interface IGeometryBatcher
    {
        int MaxVertexBatchSize { get; }
        int MaxIndexBatchSize { get; }
        IEnumerable<ushort[]> IndexBatches { get; }
        IEnumerable<VertexPosNormalTextureMod[]> VertexBatches { get; }
    }
}
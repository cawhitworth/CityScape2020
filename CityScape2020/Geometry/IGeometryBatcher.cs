// <copyright file="IGeometryBatcher.cs" company="Chris Whitworth">
// Copyright (c) Chris Whitworth. All rights reserved.
// </copyright>

namespace CityScape2020.Geometry
{
    using System.Collections.Generic;
    using CityScape2020.Rendering;

    internal interface IGeometryBatcher
    {
        int MaxVertexBatchSize { get; }

        int MaxIndexBatchSize { get; }

        IEnumerable<ushort[]> IndexBatches { get; }

        IEnumerable<VertexPosNormalTextureMod[]> VertexBatches { get; }
    }
}
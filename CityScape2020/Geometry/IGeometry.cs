// <copyright file="IGeometry.cs" company="Chris Whitworth">
// Copyright (c) Chris Whitworth. All rights reserved.
// </copyright>

namespace CityScape2020.Geometry
{
    using System.Collections.Generic;
    using CityScape2020.Rendering;

    internal interface IGeometry
    {
        IEnumerable<ushort> Indices { get; }

        IEnumerable<VertexPosNormalTextureMod> Vertices { get; }
    }
}
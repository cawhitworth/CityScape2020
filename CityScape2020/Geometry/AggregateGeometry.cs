// <copyright file="AggregateGeometry.cs" company="Chris Whitworth">
// Copyright (c) Chris Whitworth. All rights reserved.
// </copyright>

namespace CityScape2020.Geometry
{
    using System.Collections.Generic;
    using System.Linq;
    using CityScape2020.Rendering;

    internal class AggregateGeometry : IGeometry
    {
        private ushort[] indices;
        private VertexPosNormalTextureMod[] vertices;

        public AggregateGeometry(params IGeometry[] geometries)
        {
            this.Aggregate(geometries);
        }

        public AggregateGeometry(IEnumerable<IGeometry> geometries)
        {
            this.Aggregate(geometries);
        }

        public IEnumerable<ushort> Indices => this.indices;

        public IEnumerable<VertexPosNormalTextureMod> Vertices => this.vertices;

        private void Aggregate(IEnumerable<IGeometry> geometries)
        {
            var allIndices = new List<ushort>();
            var allVertices = new List<VertexPosNormalTextureMod>();
            ushort baseIndex = 0;

            foreach (var geometry in geometries)
            {
                allIndices.AddRange(geometry.Indices.Select(i => (ushort)(i + baseIndex)));
                allVertices.AddRange(geometry.Vertices);
                baseIndex += (ushort)geometry.Vertices.Count();
            }

            this.indices = allIndices.ToArray();
            this.vertices = allVertices.ToArray();
        }
    }
}
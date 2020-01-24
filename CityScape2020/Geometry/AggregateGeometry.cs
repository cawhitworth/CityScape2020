using System.Collections.Generic;
using System.Linq;
using CityScape2020.Rendering;

namespace CityScape2020.Geometry
{
    class AggregateGeometry : IGeometry
    {
        private ushort[] indices;
        private VertexPosNormalTextureMod[] vertices;

        public AggregateGeometry(params IGeometry[] geometries)
        {
            Aggregate(geometries);
        }

        public AggregateGeometry(IEnumerable<IGeometry> geometries)
        {
            Aggregate(geometries);
        }

        private void Aggregate(IEnumerable<IGeometry> geometries)
        {
            var allIndices = new List<ushort>();
            var allVertices = new List<VertexPosNormalTextureMod>();
            ushort baseIndex = 0;

            foreach (var geometry in geometries)
            {
                allIndices.AddRange(geometry.Indices.Select(i => (ushort) (i + baseIndex)));
                allVertices.AddRange(geometry.Vertices);
                baseIndex += (ushort)geometry.Vertices.Count();
            }
            indices = allIndices.ToArray();
            vertices = allVertices.ToArray();
        }

        public IEnumerable<ushort> Indices => indices;
        public IEnumerable<VertexPosNormalTextureMod> Vertices => vertices;
    }
}
using System.Collections.Generic;
using CityScape2020.Rendering;

namespace CityScape2020.Geometry
{
    internal interface IGeometry
    {
        IEnumerable<ushort> Indices { get; }
        IEnumerable<VertexPosNormalTextureMod> Vertices { get; }
    }
}
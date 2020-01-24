using System;
using System.Collections.Generic;
using CityScape2020.Rendering;
using SharpDX;

namespace CityScape2020.Geometry
{
    public class Panel : IGeometry
    {
        public enum Plane
        {
            XY,
            YZ,
            XZ
        };

        public enum Facing
        {
            In, Out
        }

        private readonly Vector3 position;
        private readonly Vector2 size;
        private readonly Plane plane;
        private readonly Facing facing;
        private readonly Vector2 texCoord1;
        private readonly Vector2 texCoord2;
        private readonly ushort[] indices;
        private readonly VertexPosNormalTextureMod[] vertices;
        private readonly Vector3 colorMod;

        public Panel(Vector3 position, Vector2 size, Plane plane, Facing facing, Vector2 tex1, Vector2 tex2, Color mod)
        {
            this.position = position;
            this.size = size;
            this.plane = plane;
            this.facing = facing;
            texCoord1 = tex1;
            texCoord2 = tex2;
            colorMod = new Vector3(mod.R / 255.0f, mod.G / 255.0f, mod.B / 255.0f);

            if (this.facing == Facing.Out)
            {
                this.indices = new ushort[]
                {
                    0, 1, 2, 1, 3, 2
                };
            }
            else
            {
                this.indices = new ushort[]
                {
                    0, 2, 1, 3, 1, 2
                };
            }
            vertices = CalculateVertices();
        }

        public IEnumerable<VertexPosNormalTextureMod> Vertices => vertices;

        public IEnumerable<ushort> Indices => indices;

        private VertexPosNormalTextureMod[] CalculateVertices()
        {
            Vector3 normal;
            Vector3 offsetVector, oppositeCorner, topLeft, bottomRight;
            switch (plane)
            {
                case Plane.XY:
                    offsetVector = new Vector3(size.X, size.Y, 0.0f);
                    oppositeCorner = position + offsetVector;
                    topLeft = new Vector3(position.X, oppositeCorner.Y, position.Z);
                    bottomRight = new Vector3(oppositeCorner.X, position.Y, position.Z);
                    normal = new Vector3(0, 0, -1);
                    break;
                case Plane.YZ:
                    offsetVector = new Vector3(0.0f, size.Y, size.X);
                    oppositeCorner = position + offsetVector;
                    topLeft = new Vector3(position.X, oppositeCorner.Y, position.Z);
                    bottomRight = new Vector3(position.X, position.Y, oppositeCorner.Z);
                    normal = new Vector3(1, 0, 0);
                    break;
                case Plane.XZ:
                    offsetVector = new Vector3(size.X, 0.0f, size.Y);
                    oppositeCorner = position + offsetVector;
                    topLeft = new Vector3(position.X, position.Y, oppositeCorner.Z);
                    bottomRight = new Vector3(oppositeCorner.X, position.Y, position.Z);
                    normal = new Vector3(0, 1, 0);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (facing == Facing.In)
                normal *= -1;

            var texTopLeft = new Vector2(texCoord1.X, texCoord2.Y);
            var texBottomRight = new Vector2(texCoord2.X, texCoord1.Y);

            return new[]
            {
                new VertexPosNormalTextureMod(position, normal, texCoord1, colorMod),
                new VertexPosNormalTextureMod(topLeft, normal, texTopLeft, colorMod),
                new VertexPosNormalTextureMod(bottomRight, normal, texBottomRight, colorMod),
                new VertexPosNormalTextureMod(oppositeCorner, normal, texCoord2, colorMod)
            };
        }
    }
}
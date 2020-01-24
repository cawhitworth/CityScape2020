// <copyright file="Panel.cs" company="Chris Whitworth">
// Copyright (c) Chris Whitworth. All rights reserved.
// </copyright>

namespace CityScape2020.Geometry
{
    using System;
    using System.Collections.Generic;
    using CityScape2020.Rendering;
    using SharpDX;

    public class Panel : IGeometry
    {
        private readonly Vector3 position;
        private readonly Vector2 size;
        private readonly Plane plane;
        private readonly Facing facing;
        private readonly Vector2 texBottomLeft;
        private readonly Vector2 texTopRight;
        private readonly ushort[] indices;
        private readonly VertexPosNormalTextureMod[] vertices;
        private readonly Vector3 colorMod;

        public Panel(Vector3 position, Vector2 size, Plane plane, Facing facing, Vector2 texBottomLeft, Vector2 texTopRight, Color mod)
        {
            this.position = position;
            this.size = size;
            this.plane = plane;
            this.facing = facing;
            this.texBottomLeft = texBottomLeft;
            this.texTopRight = texTopRight;
            this.colorMod = new Vector3(mod.R / 255.0f, mod.G / 255.0f, mod.B / 255.0f);

            if (this.facing == Facing.Out)
            {
                this.indices = new ushort[]
                {
                    0, 1, 2, 1, 3, 2,
                };
            }
            else
            {
                this.indices = new ushort[]
                {
                    0, 2, 1, 3, 1, 2,
                };
            }

            this.vertices = this.CalculateVertices();
        }

        public enum Plane
        {
            XY,
            YZ,
            XZ,
        }

        public enum Facing
        {
            In,
            Out,
        }

        public IEnumerable<VertexPosNormalTextureMod> Vertices => this.vertices;

        public IEnumerable<ushort> Indices => this.indices;

        private VertexPosNormalTextureMod[] CalculateVertices()
        {
            Vector3 normal;
            Vector3 offsetVector, oppositeCorner, topLeft, bottomRight;

            switch (this.plane)
            {
                case Plane.XY:
                    offsetVector = new Vector3(this.size.X, this.size.Y, 0.0f);
                    oppositeCorner = this.position + offsetVector;
                    topLeft = new Vector3(this.position.X, oppositeCorner.Y, this.position.Z);
                    bottomRight = new Vector3(oppositeCorner.X, this.position.Y, this.position.Z);
                    normal = new Vector3(0, 0, -1);
                    break;

                case Plane.YZ:
                    offsetVector = new Vector3(0.0f, this.size.Y, this.size.X);
                    oppositeCorner = this.position + offsetVector;
                    topLeft = new Vector3(this.position.X, oppositeCorner.Y, this.position.Z);
                    bottomRight = new Vector3(this.position.X, this.position.Y, oppositeCorner.Z);
                    normal = new Vector3(1, 0, 0);
                    break;

                case Plane.XZ:
                    offsetVector = new Vector3(this.size.X, 0.0f, this.size.Y);
                    oppositeCorner = this.position + offsetVector;
                    topLeft = new Vector3(this.position.X, this.position.Y, oppositeCorner.Z);
                    bottomRight = new Vector3(oppositeCorner.X, this.position.Y, this.position.Z);
                    normal = new Vector3(0, 1, 0);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (this.facing == Facing.In)
            {
                normal *= -1;
            }

            var texTopLeft = new Vector2(this.texBottomLeft.X, this.texTopRight.Y);
            var texBottomRight = new Vector2(this.texTopRight.X, this.texBottomLeft.Y);

            return new[]
            {
                new VertexPosNormalTextureMod(this.position, normal, this.texBottomLeft, this.colorMod),
                new VertexPosNormalTextureMod(topLeft, normal, texTopLeft, this.colorMod),
                new VertexPosNormalTextureMod(bottomRight, normal, texBottomRight, this.colorMod),
                new VertexPosNormalTextureMod(oppositeCorner, normal, this.texTopRight, this.colorMod),
            };
        }
    }
}
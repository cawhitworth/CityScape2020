// <copyright file="VertexPosNormalTextureMod.cs" company="Chris Whitworth">
// Copyright (c) Chris Whitworth. All rights reserved.
// </copyright>

namespace CityScape2020.Rendering
{
    using SharpDX;

    public struct VertexPosNormalTextureMod
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector2 Tex;
        public Vector3 Mod;

        public VertexPosNormalTextureMod(Vector3 pos, Vector3 norm, Vector2 tex, Vector3 mod)
        {
            this.Mod = mod;
            this.Position = pos;
            this.Normal = norm;
            this.Tex = tex;
        }
    }
}
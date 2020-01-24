// <copyright file="PixelTextureLightShader.cs" company="Chris Whitworth">
// Copyright (c) Chris Whitworth. All rights reserved.
// </copyright>

namespace CityScape2020.Rendering
{
    using System.IO;
    using SharpDX.Direct3D11;

    internal class PixelTextureLightShader : Component
    {
        private readonly Texture texture;
        private readonly PixelShader pixelShader;

        public PixelTextureLightShader(Device device, Texture texture)
        {
            this.texture = texture;
            var pixelShaderBytecode = File.ReadAllBytes("PixelShader.cso");
            this.pixelShader = this.ToDispose(new PixelShader(device, pixelShaderBytecode));
        }

        public void Bind(DeviceContext context)
        {
            context.PixelShader.Set(this.pixelShader);
            this.texture.Bind(context, 0);
        }
    }
}
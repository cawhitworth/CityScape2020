// <copyright file="Texture.cs" company="Chris Whitworth">
// Copyright (c) Chris Whitworth. All rights reserved.
// </copyright>

namespace CityScape2020.Rendering
{
    using SharpDX;
    using SharpDX.Direct3D11;

    internal class Texture : Component
    {
        private readonly ShaderResourceView textureView;
        private readonly SamplerState samplerState;

        private Texture(Texture2D t2D, Device device)
        {
            Texture2D texture = this.ToDispose(t2D);
            this.textureView = this.ToDispose(new ShaderResourceView(device, texture));
            this.samplerState = this.ToDispose(new SamplerState(device, new SamplerStateDescription
            {
                Filter = Filter.MinMagMipLinear,
                AddressU = TextureAddressMode.Wrap,
                AddressV = TextureAddressMode.Wrap,
                AddressW = TextureAddressMode.Wrap,
                BorderColor = Color.Black,
                ComparisonFunction = Comparison.Never,
                MaximumAnisotropy = 16,
                MipLodBias = 0,
                MinimumLod = 0,
                MaximumLod = float.MaxValue,
            }));
        }

        public static Texture FromTexture2D(Texture2D t2D, Device device)
        {
            return new Texture(t2D, device);
        }

        public void Bind(DeviceContext context, int slot)
        {
            context.PixelShader.SetSampler(slot, this.samplerState);
            context.PixelShader.SetShaderResource(slot, this.textureView);
        }
    }
}
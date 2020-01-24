using System.IO;
using SharpDX;
using SharpDX.Direct3D11;

namespace CityScape2020.Rendering
{
    class PixelTextureLightShader : Component
    {
        private readonly Texture texture;
        private readonly PixelShader pixelShader;

        public PixelTextureLightShader(Device device, Texture texture)
        {
            this.texture = texture;
            var pixelShaderBytecode = File.ReadAllBytes("PixelShader.cso");
            pixelShader = ToDispose(new PixelShader(device, pixelShaderBytecode));
        }

        public void Bind(DeviceContext context)
        {
            context.PixelShader.Set(pixelShader);
            texture.Bind(context, 0);
        }
    }
}
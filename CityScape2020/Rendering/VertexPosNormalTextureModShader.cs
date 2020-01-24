using System.IO;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Device = SharpDX.Direct3D11.Device;
using MapFlags = SharpDX.Direct3D11.MapFlags;

namespace CityScape2020.Rendering
{
    class VertexPosNormalTextureModShader : Component
    {
        private readonly Buffer constantBuffer;
        private readonly VertexShader vertexShader;

        public VertexPosNormalTextureModShader(Device device)
        {
            var vertShaderBytecode = File.ReadAllBytes("VertexShader.cso");
            vertexShader = ToDispose(new VertexShader(device, vertShaderBytecode));

            Layout = ToDispose(new InputLayout(device, vertShaderBytecode, new[]
            {
                new InputElement("POSITION", 0, Format.R32G32B32_Float, 0,0),
                new InputElement("NORMAL", 0, Format.R32G32B32_Float, 12, 0),
                new InputElement("TEXCOORD", 0, Format.R32G32_Float, 24, 0), 
                new InputElement("TEXCOORD", 1, Format.R32G32B32_Float, 32, 0) 
            }));

            constantBuffer =
                ToDispose(new Buffer(device, Utilities.SizeOf<Matrix>() * 3, ResourceUsage.Dynamic,
                    BindFlags.ConstantBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0));
        }

        public InputLayout Layout { get; }

        public void Bind(DeviceContext context, Matrix world, Matrix view, Matrix proj)
        {
            context.VertexShader.Set(vertexShader);
            context.VertexShader.SetConstantBuffer(0, constantBuffer);

            DataStream mappedResource;
            context.MapSubresource(constantBuffer, MapMode.WriteDiscard, MapFlags.None, out mappedResource);
            mappedResource.Write(world);
            mappedResource.Write(view);
            mappedResource.Write(proj);
            context.UnmapSubresource(constantBuffer, 0);
        }
    }
}
using System.Diagnostics;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DirectInput;
using SharpDX.DXGI;
using SharpDX.Windows;
using Color = SharpDX.Color;
using Device = SharpDX.Direct3D11.Device;
using Resource = SharpDX.Direct3D11.Resource;

namespace CityScape2020
{
    internal class App : Component
    {
        private RenderForm form;
        private Device device;
        private SwapChain swapChain;
        private DeviceContext deviceContext;
        private Texture2D backBuffer;
        private RenderTargetView renderTargetView;
        private Texture2D depthBuffer;
        private DepthStencilView depthStencilView;
        private Factory dxgiFactory;

        public void Run()
        {
            CreateDeviceAndSwapChain();

            var recreate = true;
            form.Resize += (sender, args) => recreate = true;

            RecreateBuffers();

            var input = new Input(form.Handle);
            var camera = new Camera(input, Width, Height);

            var city = new City(device, deviceContext);

            var proj = Matrix.Identity;

            var clock = new Stopwatch();
            clock.Start();
            var overlay = new Overlay(clock.ElapsedMilliseconds);

            var clearColor = new Color(0.1f, 0.1f, 0.2f, 0.0f);

            RenderLoop.Run(form, () =>
            {   

                input.Update();
                if (input.IsKeyDown(Key.Escape))
                    form.Close();

                camera.Update(clock.ElapsedMilliseconds);
                var view = camera.View;
                view.Transpose();


                if (recreate)
                {
                    RecreateBuffers();
                    camera.SetProjection(Width, Height);
                    proj = camera.Projection;
                    proj.Transpose();
                    recreate = false;
                }

                deviceContext.ClearDepthStencilView(depthStencilView, DepthStencilClearFlags.Depth, 1.0f, 0);
                deviceContext.ClearRenderTargetView(renderTargetView, clearColor);

                var polys = city.Draw(clock.ElapsedMilliseconds, view, proj);
                overlay.Draw(clock.ElapsedMilliseconds, polys);

                swapChain.Present(0, PresentFlags.None);
            });

            input.Dispose();
            DisposeBuffers();
            Dispose();
        }

        private int Width => form.ClientSize.Width;

        private int Height => form.ClientSize.Height;

        private void CreateDeviceAndSwapChain()
        {
            form = ToDispose(new RenderForm());

            form.Width = 1280;
            form.Height = 800;

            var desc = new SwapChainDescription
            {
                BufferCount = 1,
                IsWindowed = true,
                ModeDescription =
                    new ModeDescription(Width, Height, new Rational(60, 1), Format.R8G8B8A8_UNorm),
                OutputHandle = form.Handle,
                SampleDescription = new SampleDescription(1, 0),
                SwapEffect = SwapEffect.Discard,
                Usage = Usage.RenderTargetOutput
            };

            // Create device + swapchain
            Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.Debug, desc, out device, out swapChain);
            deviceContext = ToDispose(device.ImmediateContext);
            device = ToDispose(device);
            swapChain = ToDispose(swapChain);

            // Ignore Windows events
            dxgiFactory = ToDispose(swapChain.GetParent<Factory>());
            dxgiFactory.MakeWindowAssociation(form.Handle, WindowAssociationFlags.IgnoreAll);
        }

        private void DisposeBuffers()
        {
            depthStencilView.Dispose();
            depthBuffer.Dispose();
            renderTargetView.Dispose();
            backBuffer.Dispose();
        }

        private void RecreateBuffers()
        {
            Utilities.Dispose(ref backBuffer);
            Utilities.Dispose(ref renderTargetView);
            Utilities.Dispose(ref depthBuffer);
            Utilities.Dispose(ref depthStencilView);

            backBuffer = Resource.FromSwapChain<Texture2D>(swapChain, 0);
            renderTargetView = new RenderTargetView(device, backBuffer);
            depthBuffer = new Texture2D(device, new Texture2DDescription
            {
                Format = Format.D24_UNorm_S8_UInt,
                ArraySize = 1,
                MipLevels = 1,
                Width = Width,
                Height = Height,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default,
                BindFlags = BindFlags.DepthStencil,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None
            });
            depthStencilView = new DepthStencilView(device, depthBuffer);

            deviceContext.Rasterizer.State = new RasterizerState(device, new RasterizerStateDescription
            {
                FillMode = FillMode.Solid,
                CullMode = CullMode.Back,
                IsFrontCounterClockwise = false,
                DepthBias = 0,
                DepthBiasClamp = 0,
                SlopeScaledDepthBias = 0,
                IsDepthClipEnabled = true,
                IsScissorEnabled = false,
                IsMultisampleEnabled = false,
                IsAntialiasedLineEnabled = false
            });

            deviceContext.Rasterizer.SetViewport(new Viewport(0, 0, Width, Height, 0.0f, 1.0f));
            deviceContext.OutputMerger.SetTargets(depthStencilView, renderTargetView);
        }

    }
}
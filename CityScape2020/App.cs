// <copyright file="App.cs" company="Chris Whitworth">
// Copyright (c) Chris Whitworth. All rights reserved.
// </copyright>

namespace CityScape2020
{
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

        private int Width => this.form.ClientSize.Width;

        private int Height => this.form.ClientSize.Height;

        public void Run()
        {
            this.CreateDeviceAndSwapChain();

            var recreate = true;
            this.form.Resize += (sender, args) => recreate = true;

            this.RecreateBuffers();

            var input = new Input(this.form.Handle);
            var camera = new Camera(input, this.Width, this.Height);

            var city = new City(this.device, this.deviceContext);

            var proj = Matrix.Identity;

            var clock = new Stopwatch();
            clock.Start();
            var overlay = new Overlay(clock.ElapsedMilliseconds);

            var clearColor = new Color(0.1f, 0.1f, 0.2f, 0.0f);

            RenderLoop.Run(this.form, () =>
            {
                input.Update();
                if (input.IsKeyDown(Key.Escape))
                {
                    this.form.Close();
                }

                camera.Update(clock.ElapsedMilliseconds);
                var view = camera.View;
                view.Transpose();

                if (recreate)
                {
                    this.RecreateBuffers();
                    camera.SetProjection(this.Width, this.Height);
                    proj = camera.Projection;
                    proj.Transpose();
                    recreate = false;
                }

                this.deviceContext.ClearDepthStencilView(this.depthStencilView, DepthStencilClearFlags.Depth, 1.0f, 0);
                this.deviceContext.ClearRenderTargetView(this.renderTargetView, clearColor);

                var polys = city.Draw(clock.ElapsedMilliseconds, view, proj);
                overlay.Draw(clock.ElapsedMilliseconds, polys);

                this.swapChain.Present(0, PresentFlags.None);
            });

            input.Dispose();
            this.DisposeBuffers();
            this.Dispose();
        }

        private void CreateDeviceAndSwapChain()
        {
            this.form = this.ToDispose(new RenderForm());

            this.form.Width = 1280;
            this.form.Height = 800;

            var desc = new SwapChainDescription
            {
                BufferCount = 1,
                IsWindowed = true,
                ModeDescription =
                    new ModeDescription(this.Width, this.Height, new Rational(60, 1), Format.R8G8B8A8_UNorm),
                OutputHandle = this.form.Handle,
                SampleDescription = new SampleDescription(1, 0),
                SwapEffect = SwapEffect.Discard,
                Usage = Usage.RenderTargetOutput,
            };

            // Create device + swapchain
            Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.Debug, desc, out this.device, out this.swapChain);
            this.deviceContext = this.ToDispose(this.device.ImmediateContext);
            this.device = this.ToDispose(this.device);
            this.swapChain = this.ToDispose(this.swapChain);

            // Ignore Windows events
            this.dxgiFactory = this.ToDispose(this.swapChain.GetParent<Factory>());
            this.dxgiFactory.MakeWindowAssociation(this.form.Handle, WindowAssociationFlags.IgnoreAll);
        }

        private void DisposeBuffers()
        {
            this.depthStencilView.Dispose();
            this.depthBuffer.Dispose();
            this.renderTargetView.Dispose();
            this.backBuffer.Dispose();
        }

        private void RecreateBuffers()
        {
            Utilities.Dispose(ref this.backBuffer);
            Utilities.Dispose(ref this.renderTargetView);
            Utilities.Dispose(ref this.depthBuffer);
            Utilities.Dispose(ref this.depthStencilView);

            this.backBuffer = Resource.FromSwapChain<Texture2D>(this.swapChain, 0);
            this.renderTargetView = new RenderTargetView(this.device, this.backBuffer);
            this.depthBuffer = new Texture2D(this.device, new Texture2DDescription
            {
                Format = Format.D24_UNorm_S8_UInt,
                ArraySize = 1,
                MipLevels = 1,
                Width = this.Width,
                Height = this.Height,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default,
                BindFlags = BindFlags.DepthStencil,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None,
            });
            this.depthStencilView = new DepthStencilView(this.device, this.depthBuffer);

            this.deviceContext.Rasterizer.State = new RasterizerState(this.device, new RasterizerStateDescription
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
                IsAntialiasedLineEnabled = false,
            });

            this.deviceContext.Rasterizer.SetViewport(new Viewport(0, 0, this.Width, this.Height, 0.0f, 1.0f));
            this.deviceContext.OutputMerger.SetTargets(this.depthStencilView, this.renderTargetView);
        }
    }
}
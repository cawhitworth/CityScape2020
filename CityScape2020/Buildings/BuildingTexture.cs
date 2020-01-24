// <copyright file="BuildingTexture.cs" company="Chris Whitworth">
// Copyright (c) Chris Whitworth. All rights reserved.
// </copyright>

namespace CityScape2020.Buildings
{
    using System;
    using SharpDX;
    using SharpDX.Direct3D11;
    using SharpDX.DXGI;
    using Device = SharpDX.Direct3D11.Device;

    internal class BuildingTexture : Component
    {
        private readonly int windowWidth;
        private readonly int windowHeight;
        private readonly int height;
        private readonly int width;
        private readonly Random random = new Random();

        public BuildingTexture(Device device, DeviceContext context, Size2 textureSize, Size2 windowSize)
        {
            this.height = textureSize.Height;
            this.width = textureSize.Width;
            this.windowWidth = windowSize.Width;
            this.windowHeight = windowSize.Height;

            var p = this.Pixels();

            var stream = new DataStream(this.width * this.height * 4, true, true);
            stream.WriteRange(p);
            var data = new DataBox(stream.DataPointer, this.width * 4, this.width * this.height * 4);

            this.Texture = new Texture2D(device, new Texture2DDescription
            {
                ArraySize = 1,
                BindFlags = BindFlags.ShaderResource | BindFlags.RenderTarget,
                CpuAccessFlags = CpuAccessFlags.None,
                Format = Format.R8G8B8A8_UNorm,
                Height = this.height,
                Width = this.width,
                MipLevels = 0,
                OptionFlags = ResourceOptionFlags.GenerateMipMaps,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default,
            });

            context.UpdateSubresource(data, this.Texture);
            var textureView = this.ToDispose(new ShaderResourceView(device, this.Texture));
            context.GenerateMips(textureView);
        }

        public Texture2D Texture { get; }

        public Color[] Pixels()
        {
            var pixels = new Color[this.width * this.height];
            var black = new Color(0x10, 0x10, 0x10, 0x00);
            for (int p = 0; p < this.width * this.height; p++)
            {
                pixels[p] = black;
            }

            // Random windows
            for (int x = 0; x < this.width / this.windowWidth; x++)
            {
                for (int y = 0; y < this.height / this.windowHeight; y++)
                {
                    bool light = this.random.NextDouble() > 0.9;
                    float shade = 0.15f + ((float)this.random.NextDouble() * 0.15f);
                    if (light)
                    {
                        shade += 0.75f;
                    }

                    this.DrawWindow(pixels, x, y, shade);
                }
            }

            // Clusters
            for (int streak = 0; streak < 10; streak++)
            {
                int startX = this.random.Next(this.width / this.windowWidth);
                int startY = this.random.Next(this.height / this.windowHeight);

                int width = this.random.Next(this.width / (4 * this.windowWidth)) + 1;
                if (width + startX > (this.width / this.windowWidth))
                {
                    width = (this.width / this.windowWidth) - startX;
                }

                int lines = this.random.Next(2) + 1;
                if (startY - lines < 0)
                {
                    startY = lines;
                }

                for (int line = 0; line < lines; line++)
                {
                    for (int xx = startX; xx < startX + width; xx++)
                    {
                        float shade = 0.65f + ((float)this.random.NextDouble() * 0.25f);
                        this.DrawWindow(pixels, xx, startY - line, shade);
                    }

                    startX += this.random.Next(6) - 3;
                    if (startX < 0)
                    {
                        startX = 0;
                    }

                    if (width + startX > (this.width / this.windowWidth))
                    {
                        width = (this.width / this.windowWidth) - startX;
                    }
                }
            }

            return pixels;
        }

        private void DrawWindow(Color[] pixels, int x, int y, float shade)
        {
            var col = new Color(shade, shade, shade, 1.0f);

            for (int xx = 1; xx < this.windowWidth - 1; xx++)
            {
                for (int yy = 1; yy < this.windowHeight - 1; yy++)
                {
                    pixels[this.OffsetFor(x, y, xx, yy)] = col;
                }
            }

            // Add noise
            int points = this.random.Next(16) + 16;
            for (int point = 0; point < points; point++)
            {
                int xx = this.random.Next(6) + 1;
                int yy = this.random.Next(4) + 1;

                Color c = pixels[this.OffsetFor(x, y, xx, yy)];
                c.R = this.ModColor(c.R, 2);
                c.G = this.ModColor(c.R, 4);
                c.B = this.ModColor(c.R, 4);

                pixels[this.OffsetFor(x, y, xx, yy)] = c;
            }
        }

        private int OffsetFor(int x, int y, int xx, int yy)
        {
            return (x * this.windowWidth) + xx + (((y * this.windowHeight) + yy) * this.width);
        }

        private byte ModColor(byte component, int divisor)
        {
            var comp = (int)component;
            comp -= this.random.Next(component / divisor);

            comp = Math.Min(comp, 255);
            comp = Math.Max(comp, 0);

            return (byte)comp;
        }
    }
}
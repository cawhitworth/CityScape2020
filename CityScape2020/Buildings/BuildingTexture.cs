using System;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Device = SharpDX.Direct3D11.Device;

namespace CityScape2020.Buildings
{
    class BuildingTexture : Component
    {
        private readonly int windowWidth;
        private readonly int windowHeight;
        private readonly int height;
        private readonly int width;
        private readonly Random random = new Random();

        public BuildingTexture(Device device, DeviceContext context, Size2 textureSize, Size2 windowSize)
        {
            height = textureSize.Height;
            width = textureSize.Width;
            windowWidth = windowSize.Width;
            windowHeight = windowSize.Height;

            var p = Pixels();

            var stream = new DataStream(width*height*4, true, true);
            stream.WriteRange(p);
            var data = new DataBox(stream.DataPointer, width * 4, width * height * 4);

            Texture = new Texture2D(device, new Texture2DDescription
            {
                ArraySize = 1,
                BindFlags = BindFlags.ShaderResource | BindFlags.RenderTarget,
                CpuAccessFlags = CpuAccessFlags.None,
                Format = Format.R8G8B8A8_UNorm,
                Height = height,
                Width = width,
                MipLevels = 0,
                OptionFlags = ResourceOptionFlags.GenerateMipMaps,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default
            });

            context.UpdateSubresource(data, Texture);
            var textureView = ToDispose(new ShaderResourceView(device, Texture));
            context.GenerateMips(textureView);
        }

        public Texture2D Texture { get; }

        public Color[] Pixels()
        {
            var pixels = new Color[width * height];
            var black = new Color(0);
            for (int p = 0; p < width * height; p++)
            {
                pixels[p] = black;
            }

            // Random windows

            for (int x = 0; x < width / windowWidth; x++)
            {
                for (int y = 0; y < height / windowHeight; y++)
                {
                    bool light = random.NextDouble() > 0.9;
                    float shade = (float)random.NextDouble() * 0.15f;
                    if (light)
                        shade += 0.75f;

                    DrawWindow(pixels, x, y, shade);
                }
            }

            // Clusters

            for (int streak = 0; streak < 10; streak++)
            {
                int startX = this.random.Next(this.width / this.windowWidth);
                int startY = random.Next(height / windowHeight);
                int width = this.random.Next(this.width / (4 * this.windowWidth)) + 1;
                if (width + startX > (this.width / this.windowWidth)) width = (this.width / this.windowWidth) - startX;
                int lines = random.Next(2) + 1;
                if (startY - lines < 0) startY = lines;
                for (int line = 0; line < lines; line++)
                {
                    for (int xx = startX; xx < startX + width; xx++)
                    {
                        float shade = 0.65f + (float)random.NextDouble() * 0.25f;
                        DrawWindow(pixels, xx, startY - line, shade);
                    }
                    startX += random.Next(6) - 3;
                    if (startX < 0) startX = 0;
                    if (width + startX > (this.width / this.windowWidth)) width = (this.width / this.windowWidth) - startX;
                }
            }

            return pixels;
        }

        private void DrawWindow(Color[] pixels, int x, int y, float shade)
        {
            var col = new Color(shade, shade, shade, 1.0f);

            for (int xx = 1; xx < windowWidth - 1; xx++)
            {
                for (int yy = 1; yy < windowHeight - 1; yy++)
                {
                    pixels[(x*windowWidth) + xx + ((y*windowHeight) + yy)*width] = col;
                }
            }

            // Add noise

            int points = random.Next(16) + 16;
            for (int point = 0; point < points; point++)
            {
                int xx = random.Next(6) + 1;
                int yy = random.Next(4) + 1;
                Color c = pixels[(x*windowWidth) + xx + ((y*windowHeight) + yy)*width];
                c.R = ModColor(c.R, 2);
                c.G = c.B = c.R;
                c.G = ModColor(c.G, 4);
                c.B = ModColor(c.B, 4);
                pixels[(x*windowWidth) + xx + ((y*windowHeight) + yy)*width] = c;

            }
        }

        byte ModColor(byte component, int divisor)
        {
            var comp = (int)component;
            comp -= random.Next( component / divisor );
            if (comp > 255) comp = 255;
            if (comp < 0) comp = 0;
            return (byte)comp;
        }
    }
}
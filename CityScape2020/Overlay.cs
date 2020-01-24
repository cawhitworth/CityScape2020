﻿using System;

namespace CityScape2020
{
    internal class Overlay
    {
        private long last;
        private int frames;
        private long elapsed;
        private float fps;

        public Overlay(long initial)
        {
            last = initial;
        }

        public void Draw(long elapsed, int polygons)
        {
            frames++;
            this.elapsed += elapsed - this.last;
            last = elapsed;

            if (this.elapsed > 1000)
            {
                this.fps = this.frames/(this.elapsed/1000.0f);
                this.frames = 0;
                this.elapsed = 0;
                Console.WriteLine("{0} fps, {1} polys/frame ({2} kpolys/sec)", this.fps, polygons, (polygons * this.fps) / 1000);
            }

        }
    }
}
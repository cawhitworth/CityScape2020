// <copyright file="Lot.cs" company="Chris Whitworth">
// Copyright (c) Chris Whitworth. All rights reserved.
// </copyright>

namespace CityScape2020.CityPlanning
{
    using System;

    public class Lot
    {
        private static Random rand = new Random();

        public Lot()
        {
        }

        public Lot(int x, int y, int w, int h)
        {
            this.X = x;
            this.Y = y;
            this.W = w;
            this.H = h;
        }

        public int X { get; set; }

        public int Y { get; set; }

        public int W { get; set; }

        public int H { get; set; }

        public (Lot, Road) Split(int roadWidth)
        {
            if (rand.NextDouble() < this.W / (double)(this.W + this.H))
            {
                return this.SplitH(roadWidth);
            }
            else
            {
                return this.SplitV(roadWidth);
            }
        }

        public (Lot, Road) SplitH(int roadWidth)
        {
            if (this.W < roadWidth * 3)
            {
                return (null, null);
            }

            int splitPoint;

            if (this.W == roadWidth * 3)
            {
                splitPoint = roadWidth;
            }
            else
            {
                splitPoint = rand.Next(this.W - (roadWidth * 3)) + roadWidth;
            }

            var lot = new Lot
            {
                X = this.X + splitPoint + roadWidth,
                Y = this.Y,
                W = this.W - (splitPoint + roadWidth),
                H = this.H,
            };

            var road = new Road
            {
                Orientation = Orientation.Vertical,
                X = this.X + splitPoint,
                Y = this.Y,
                Width = roadWidth,
                Length = this.H,
            };

            if (lot.X + lot.W > this.X + this.W)
            {
                Console.WriteLine("This seems wrong");
            }

            this.W = splitPoint;
            return (lot, road);
        }

        public (Lot, Road) SplitV(int roadWidth)
        {
            if (this.H < roadWidth * 3)
            {
                return (null, null);
            }

            int splitPoint;

            if (this.H == roadWidth * 3)
            {
                splitPoint = roadWidth;
            }
            else
            {
                splitPoint = rand.Next(this.H - (roadWidth * 3)) + roadWidth;
            }

            var lot = new Lot
            {
                X = this.X,
                Y = this.Y + splitPoint + roadWidth,
                H = this.H - (splitPoint + roadWidth),
                W = this.W,
            };

            var road = new Road
            {
                Orientation = Orientation.Horizontal,
                X = this.X,
                Y = this.Y + splitPoint,
                Width = roadWidth,
                Length = this.W,
            };

            if (lot.Y + lot.H > this.Y + this.H)
            {
                Console.WriteLine("This seems wrong");
            }

            this.H = splitPoint;

            return (lot, road);
        }

        public override string ToString() => $"{this.X},{this.Y} {this.W},{this.H}";
    }
}

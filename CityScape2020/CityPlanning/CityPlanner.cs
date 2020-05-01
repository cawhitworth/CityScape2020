// <copyright file="CityPlanner.cs" company="Chris Whitworth">
// Copyright (c) Chris Whitworth. All rights reserved.
// </copyright>

namespace CityScape2020.CityPlanning
{
    using System;
    using System.Collections.Generic;
    using CityScape2020.Geometry;
    using SharpDX;

    internal class CityPlanner
    {
        private const int Height = 1024;
        private const int Width = 1024;
        private const int MaxSize = 30;
        private const int MaxRatio = 3;

        private static Random rand = new Random();

        private static Color lot = Color.Blue;
        private static Dictionary<RoadType, Color> roadColors = null;

        public IEnumerable<IGeometry> BuildCity()
        {
            var textureData = new Color[Width * Height];
            for (int i = 0; i < Width * Height; i++)
            {
                textureData[i] = Color.Black;
            }

            var lots = new List<Lot>();
            var roads = new List<Road>();

            lots.Add(new Lot(0, 0, Width, Height));

            int count = 0, notSplit = 0, splitFail = 0;
            int roadWidth = 16;
            RoadType roadType = RoadType.DualCarriageway;
            for (int splits = 0; splits < 50; splits++)
            {
                switch (splits)
                {
                    case 5:
                        roadWidth = 8;
                        roadType = RoadType.Major;
                        break;
                    case 10:
                        roadWidth = 4;
                        roadType = RoadType.Minor;
                        break;
                    case 15:
                        roadWidth = 2;
                        break;
                    case 35:
                        roadType = RoadType.Path;
                        break;
                }

                var newLots = new List<Lot>();
                var newRoads = new List<Road>();

                foreach (Lot lot in lots)
                {
                    int midX = lot.X + (lot.W / 2);
                    int midY = lot.Y + (lot.H / 2);
                    int dx = (midX - (Width / 2)) * (midX - (Width / 2));
                    int dy = (midY - (Height / 2)) * (midY - (Height / 2));
                    int d = dx + dy;
                    double distFactor = Math.Sqrt(d) / Math.Sqrt((Width * Width) + (Height * Height));
                    double areaFactor = lot.W * lot.H / (double)(Width * Height);

                    if (rand.NextDouble() < distFactor + (areaFactor * 2) || splits < 2)
                    {
                        count++;
                        (Lot newLot, Road newRoad) = lot.Split(roadWidth);

                        if (newLot != null)
                        {
                            newRoad.Type = roadType;
                            newLots.Add(newLot);
                            newRoads.Add(newRoad);
                        }
                        else
                        {
                            splitFail++;
                        }
                    }
                    else
                    {
                        notSplit++;
                    }
                }

                lots.AddRange(newLots);
                roads.AddRange(newRoads);
            }

            bool done;
            do
            {
                done = true;
                var newLots = new List<Lot>();
                var newRoads = new List<Road>();
                foreach (Lot lot in lots)
                {
                    if (lot.W > MaxSize || lot.W / lot.H > MaxRatio)
                    {
                        (Lot newLot, Road newRoad) = lot.SplitH(2);

                        if (newLot != null)
                        {
                            newRoad.Type = RoadType.Path;
                            newLots.Add(newLot);
                            newRoads.Add(newRoad);
                        }

                        done = false;
                    }

                    if (lot.H > MaxSize || lot.H / lot.W > MaxRatio)
                    {
                        (Lot newLot, Road newRoad) = lot.SplitV(2);

                        if (newLot != null)
                        {
                            newRoad.Type = RoadType.Path;
                            newLots.Add(newLot);
                            newRoads.Add(newRoad);
                        }

                        done = false;
                    }
                }

                lots.AddRange(newLots);
                roads.AddRange(newRoads);
            }
            while (!done);

            foreach (Lot l in lots)
            {
                AddLot(ref textureData, l);
            }

            foreach (Road r in roads)
            {
                AddRoad(ref textureData, r);
            }

            var buildingBuilder = new BuildingBuilder();

            var buildings = new List<IGeometry>();

            var maxWidth = 0;
            var maxHeight = 0;
            foreach (Lot l in lots)
            {
                maxWidth = Math.Max(l.W, maxWidth);
                maxHeight = Math.Max(l.H, maxHeight);

                int stories = (int)(Math.Max(l.H, l.W) * (0.5 + rand.NextDouble()));
                buildings.Add(buildingBuilder.MakeBuilding(l.X, l.Y, l.W, stories, l.H));
            }

            buildings.Add(buildingBuilder.MakeBase(0, 0, Width, Height));

            return buildings;
        }

        private static Color Get(ref Color[] data, int x, int y)
        {
            return data[x + (y * Width)];
        }

        private static void Plot(ref Color[] data, int x, int y, Color pixel)
        {
            data[x + (y * Width)] = pixel;
        }

        private static void AddLot(ref Color[] data, Lot l)
        {
            AddRect(ref data, l.X, l.Y, l.W, l.H, lot);
        }

        private static void AddRect(ref Color[] data, int x, int y, int w, int h, Color color)
        {
            int px, py;
            for (px = x; px < x + w; px++)
            {
                for (py = y; py < y + h; py++)
                {
                    Plot(ref data, px, py, color);
                }
            }
        }

        private static void BuildRoadColors()
        {
            roadColors = new Dictionary<RoadType, Color>();
            roadColors[RoadType.DualCarriageway] = Color.DarkRed;
            roadColors[RoadType.Major] = Color.Yellow;
            roadColors[RoadType.Minor] = Color.Gray;
            roadColors[RoadType.Path] = Color.DarkBlue;
        }

        private static void AddRoad(ref Color[] data, Road r)
        {
            if (roadColors == null)
            {
                BuildRoadColors();
            }

            if (r.Orientation == Orientation.Horizontal)
            {
                AddRect(ref data, r.X, r.Y, r.Length, r.Width, roadColors[r.Type]);
            }
            else
            {
                AddRect(ref data, r.X, r.Y, r.Width, r.Length, roadColors[r.Type]);
            }
        }
    }
}

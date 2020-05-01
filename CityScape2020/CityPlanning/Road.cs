// <copyright file="Road.cs" company="Chris Whitworth">
// Copyright (c) Chris Whitworth. All rights reserved.
// </copyright>

namespace CityScape2020.CityPlanning
{
    public enum Orientation
    {
        Horizontal,
        Vertical,
    }

    public enum RoadType
    {
        DualCarriageway,
        Major,
        Minor,
        Path,
    }

    public class Road
    {
        public int X { get; set; }

        public int Y { get; set; }

        public int Width { get; set; }

        public int Length { get; set; }

        public Orientation Orientation { get; set; }

        public RoadType Type { get; set; }
    }
}

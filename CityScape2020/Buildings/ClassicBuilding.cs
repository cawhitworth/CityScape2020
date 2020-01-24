using System;
using System.Collections.Generic;
using System.Windows.Forms.VisualStyles;
using CityScape2020.Geometry;
using CityScape2020.Rendering;
using SharpDX;

namespace CityScape2020.Buildings
{
    class ClassicBuilding : IGeometry
    {
        private AggregateGeometry aggregateGeometry;
        private Random random;

        public ClassicBuilding(Vector3 corner, int widthStories, int heightStories, int depthStories, StoryCalculator storyCalc)
        {
            var builder = new ColumnedBuildingBlockBuilder(storyCalc);

            random = new Random();
            int totalHeight = 0;

            var geometry = new List<IGeometry>();

            var oppCorner = new Vector3(corner.X + widthStories*storyCalc.StorySize, 
                                        corner.Y - 0.5f,
                                        corner.Z + depthStories*storyCalc.StorySize);

            var center = (oppCorner + corner)/2;
            center.Y = 0.0f;

            // Base of the building

            geometry.Add(new Box(corner, oppCorner));

            var tierScale = 0.6 + (random.NextDouble() * 0.4);

            widthStories -= 1;
            depthStories -= 1;
            while (totalHeight < heightStories)
            {
                corner.X = center.X - (widthStories/2.0f) * storyCalc.StorySize;
                corner.Z = center.Z - (depthStories/2.0f) * storyCalc.StorySize;
                corner.Y = center.Y + (totalHeight*storyCalc.StorySize);

                var tierHeight = 0;

                if (heightStories - totalHeight < 5)
                {
                    tierHeight = heightStories - totalHeight;
                }
                else
                {
                    tierHeight = heightStories*2;

                    while (totalHeight + tierHeight > heightStories && tierHeight != 0)
                    {
                        if (heightStories - totalHeight > totalHeight/3)
                        {
                            tierHeight = random.Next((heightStories*5)/6) + (heightStories/6);
                        }
                        else
                        {
                            tierHeight = heightStories - totalHeight;
                        }
                    }
                }

                geometry.Add(builder.Build(corner, widthStories, tierHeight, depthStories));

                totalHeight += tierHeight;

                widthStories =(int) (tierScale*widthStories);
                depthStories =(int) (tierScale*depthStories);
                if (widthStories < 1) widthStories = 1;
                if (depthStories < 1) depthStories = 1;

            }


            aggregateGeometry = new AggregateGeometry(geometry);
        }

        public IEnumerable<ushort> Indices => aggregateGeometry.Indices;

        public IEnumerable<VertexPosNormalTextureMod> Vertices => aggregateGeometry.Vertices;
    }
}

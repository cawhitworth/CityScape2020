using System;
using SharpDX;

namespace CityScape2020.Buildings
{
    class ModColor
    {
        private readonly Random random;

        public ModColor(Random random)
        {
            this.random = random;
        }

        public Color Pick()
        {
            Color mod;
            switch (random.Next(3))
            {
                case 0:
                    mod = new Color(1.0f, 1.0f, 0.8f, 1.0f);
                    break;
                case 1:
                    mod = new Color(0.95f, 1.0f, 1.0f, 1.0f);
                    break;
                default:
                    mod = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                    break;
            }
            return mod;
        }
    }
}
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galaga.Utilities
{
    public class MyRandom : Random
    {
        /// <summary>
        /// Keep this around to optimize gaussian calculation performance.
        /// </summary>
        private double y2;
        private bool usePrevious { get; set; }

        /// <summary>
        /// Generates a random number in the range or [Min,Max]
        /// </summary>
        public float NextRange(float min, float max)
        {
            return MathHelper.Lerp(min, max, (float)this.NextDouble());
        }

        public Vector2 NextVector()
        {
            float x = this.NextRange(0, 1);
            float y = this.NextRange(0, 1);

            return new Vector2(x, y);
        }

        /// <summary>
        /// Generate a random vector about a unit circle
        /// </summary>
        public Vector2 NextCircleVector()
        {
            float angle = (float)(this.NextDouble() * 2.0 * Math.PI);
            float x = (float)Math.Cos(angle);
            float y = (float)Math.Sin(angle);

            return new Vector2(x, y);
        }

        /// <summary>
        /// Generate a normally distributed random number.  Derived from a Wiki reference on
        /// how to do this.
        /// </summary>
        public double NextGaussian(double mean, double stdDev)
        {
            if (this.usePrevious)
            {
                this.usePrevious = false;
                return mean + y2 * stdDev;
            }
            this.usePrevious = true;

            double x1 = 0.0;
            double x2 = 0.0;
            double y1 = 0.0;
            double z = 0.0;

            do
            {
                x1 = 2.0 * this.NextDouble() - 1.0;
                x2 = 2.0 * this.NextDouble() - 1.0;
                z = (x1 * x1) + (x2 * x2);
            }
            while (z >= 1.0);

            z = Math.Sqrt((-2.0 * Math.Log(z)) / z);
            y1 = x1 * z;
            y2 = x2 * z;

            return mean + y1 * stdDev;
        }


    }
}
